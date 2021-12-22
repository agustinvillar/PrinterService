using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Extensions;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
using Menoo.PrinterService.Infraestructure.Repository;
using Menoo.PrinterService.Infraestructure.Services;
using Rebus.Activation;
using Rebus.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;

namespace Menoo.PrinterService.Builder
{
    public partial class Builder : ServiceBase, ISubscriptionService
    {
        private readonly TicketRepository _ticketRepository;

        private readonly EventLog _generalWriter;

        private readonly BuiltinHandlerActivator _adapter;

        private Timer _timer;

        private readonly string _queueName;

        private readonly string _queueConnectionString;

        private readonly int _queueDelay;

        private long _tickCounter;

        public Builder()
        {
            InitializeService();
            _adapter = new BuiltinHandlerActivator();
            _queueName = GlobalConfig.ConfigurationManager.GetSetting("queueName");
            _queueConnectionString = GlobalConfig.ConfigurationManager.GetSetting("queueConnectionString");
            _queueDelay = int.Parse(GlobalConfig.ConfigurationManager.GetSetting("queueDelay"));
            _generalWriter = GlobalConfig.DependencyResolver.ResolveByName<EventLog>("builder");
            _ticketRepository = GlobalConfig.DependencyResolver.Resolve<TicketRepository>();
        }

        public async Task RecieveAsync(PrintMessage data, Dictionary<string, string> extras = null)
        {
            var builders = GlobalConfig.DependencyResolver.ResolveAll<ITicketBuilder>();
            foreach (var builder in builders)
            {
                if (builder.ToString() == data.Builder) 
                {
                    _generalWriter.WriteEntry($"Builder::RecieveAsync(). Activando el builder de: {builder}", EventLogEntryType.Information);
                    string type = !string.IsNullOrEmpty(data.SubTypeDocument) ? $"{data.TypeDocument}-{data.SubTypeDocument}" : $"{data.TypeDocument}";
                    string documentsId = data.DocumentsId != null && data.DocumentsId.Count > 0 ? string.Join(",", data.DocumentsId) : data.DocumentId;
                    _generalWriter.WriteEntry(
                        $"{builder}::BuildAsync(). Nuevo ticket de impresión recibido. {Environment.NewLine}" +
                        $"Evento: {data.PrintEvent}{Environment.NewLine}" +
                        $"Tipo: {type}{Environment.NewLine}" +
                        $"FirebaseId: {documentsId}{Environment.NewLine}", EventLogEntryType.Information);
                    try
                    {
                        await Task.Delay(_queueDelay);
                        var dataToPrint = await builder.BuildAsync(extras["id"], data);
                        if (dataToPrint == null || dataToPrint.Count == 0) 
                        {
                            break;
                        }
                        await PrintAsync(extras["id"], dataToPrint, data.PrintEvent);
                    }
                    catch (Exception e) 
                    {
                        _generalWriter.WriteEntry(
                            $"{builder}::RecieveAsync(). NO se imprimió el ticket de impresión recibido. {Environment.NewLine}" +
                            $"Evento: {data.PrintEvent}{Environment.NewLine}" +
                            $"Tipo: {type}{Environment.NewLine}" +
                            $"FirebaseId: {documentsId}{Environment.NewLine}" +
                            $"Excepción: {e}", EventLogEntryType.Error);
                    }
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            Debugger.Launch();
            _generalWriter.WriteEntry("Builder::OnStart(). Iniciando servicio.", EventLogEntryType.Information);
            ConfigureWorker();
        }

        protected override void OnShutdown()
        {
            if (_adapter != null)
            {
                _adapter.Dispose();
            }
            _generalWriter.WriteEntry("Builder::OnShutdown(). Apagando servicio.", EventLogEntryType.Warning);
            base.OnShutdown();
        }

        protected override void OnStop()
        {
            if (_adapter != null)
            {
                _adapter.Dispose();
            }
            _generalWriter.WriteEntry("Builder::OnStop(). Deteniendo servicio.", EventLogEntryType.Warning);
        }

        private void ConfigureWorker()
        {
            _adapter.Handle<PrintMessage>(async (bus, context, message) =>
            {
                await RecieveAsync(message, context.Headers);
            });
            Configure.With(_adapter)
                .Logging(l => l.Serilog())
                .Transport(t => t.UseRabbitMq(_queueConnectionString, _queueName))
                .Options(o => o.SetMaxParallelism(1))
                .Options(o => o.SetNumberOfWorkers(1))
                .Start();
            _adapter.Bus.Subscribe<PrintMessage>().GetAwaiter().GetResult();
        }

        #region private methods
        private void ConfigureTimer()
        {
            double.TryParse(GlobalConfig.ConfigurationManager.GetSetting("serviceInternal"), out double interval);
            if (interval == 0d)
            {
                interval = 10000d;
            }
            _tickCounter = 0L;
            _timer = new Timer(interval);
            _timer.Elapsed += ServiceTimer_Tick;
        }

        private void InitializeService()
        {
            try
            {
                AutoLog = false;
                CanHandlePowerEvent = true;
                CanPauseAndContinue = false;
                CanShutdown = true;
                ServiceName = GlobalConfig.ConfigurationManager.GetSetting("serviceBuilderName");
                ConfigureTimer();
            }
            catch (Exception ex)
            {
                EventLog eventLog = new EventLog
                {
                    Source = GlobalConfig.ConfigurationManager.GetSetting("defaultLog")
                };
                eventLog.WriteEntry("Builder::InitializeService()" + Environment.NewLine + ex, EventLogEntryType.Error);
                eventLog.Dispose();
            }
        }

        private async Task PrintAsync(string id, List<PrintInfo> data, string printEvent)
        {
            if (printEvent == PrintEvents.NEW_TAKE_AWAY)
            {
                //TODO: Mover código para este caso especial.
            }
            else
            {
                if (data == null || data.Count == 0)
                {
                    return;
                }
                foreach (var item in data)
                {
                    var sectors = item.Store.GetPrintSettings(printEvent);
                    foreach (var sector in sectors)
                    {
                        IFormaterService formatterService = FormaterFactory.Resolve(sector.IsHTML.GetValueOrDefault(), item.Content, item.Template);
                        string image = formatterService.Create();
                        //Se arma el objeto de ticket
                        var ticket = new Ticket
                        {
                            TicketType = printEvent,
                            PrintBefore = item.BeforeAt,
                            Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                            Copies = sector.Copies,
                            PrinterName = sector.Printer,
                            TicketImage = image,
                            StoreName = item.Store.Name,
                            StoreId = item.Store.Id
                        };
                        _generalWriter.WriteEntry($"Builder::PrintAsync(). Enviando a imprimir el ticket con la siguiente información." +
                            $"{Environment.NewLine}Detalles:{Environment.NewLine}" +
                            $"Nombre de la impresora: {sector.Printer}{Environment.NewLine}" +
                            $"Sector de impresión: {sector.Name}{Environment.NewLine}" +
                            $"Hora de impresión: {ticket.PrintBefore}{Environment.NewLine}" +
                            $"Restaurante: {ticket.StoreName}{Environment.NewLine}" +
                            $"Id en colección printEvents: {id}");
                        await _ticketRepository.SaveAsync(ticket);
                        await _ticketRepository.SetTicketImageAsync(id, image);
                    }
                }
            }
        }

        private void ServiceTimer_Tick(object sender, ElapsedEventArgs e)
        {
            try
            {
                long memSize = 0L;
                _tickCounter++;
                if (_tickCounter % 60L == 0L)
                {
                    memSize = GC.GetTotalMemory(true);
                    _generalWriter.WriteEntry($"Builder::ServiceTimer_Tick(). Se ha liberado {memSize / 1024 }." , EventLogEntryType.Warning);
                }
                if (_tickCounter % 3600L == 0L)
                {
                    _timer.Stop();
                    //Proceso de mantenimiento de .NET.
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.WaitForFullGCComplete();
                    memSize = GC.GetTotalMemory(true);
                    _generalWriter.WriteEntry($"Builder::ServiceTimer_Tick(). Se ha liberado {memSize / 1024 }.", EventLogEntryType.Warning);
                    _timer.Start();
                }
            }
            catch (Exception ex)
            {
                _generalWriter.WriteEntry("Builder::ServiceTimer_Tick()" + Environment.NewLine + ex, EventLogEntryType.Error);
            }
        }
        #endregion
    }
}
