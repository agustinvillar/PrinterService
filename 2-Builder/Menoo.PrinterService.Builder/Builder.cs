using Menoo.Backend.Integrations.Constants;
using Menoo.Backend.Integrations.Messages;
using Menoo.Printer.Builder.Orders.Repository;
using Menoo.PrinterService.Builder.Hub;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Entities;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Services;
using Microsoft.Owin.Hosting;
using Rebus.Activation;
using Rebus.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;

namespace Menoo.PrinterService.Builder
{
    public partial class Builder : ServiceBase, ISubscriptionService
    {
        private readonly BuiltinHandlerActivator _adapter;

        private readonly EventLog _generalWriter;

        private readonly PrintHub _hub;

        private readonly ItemRepository _itemRepository;

        private readonly string _queueConnectionString;

        private readonly string _queueName;

        private readonly string _signalR;

        private long _tickCounter;

        private System.Timers.Timer _timer;

        public Builder()
        {
            InitializeService();
            _adapter = new BuiltinHandlerActivator();
            _queueName = GlobalConfig.ConfigurationManager.GetSetting("QueuePrint");
            _queueConnectionString = GlobalConfig.ConfigurationManager.GetSetting("queueConnectionString");
            _generalWriter = GlobalConfig.DependencyResolver.ResolveByName<EventLog>("builder");
            _signalR = GlobalConfig.ConfigurationManager.GetSetting("signalR");
            _itemRepository = GlobalConfig.DependencyResolver.Resolve<ItemRepository>();
            _hub = new PrintHub();
        }

        public async Task RecieveAsync(PrintMessage data, Dictionary<string, string> extras = null)
        {
            var builders = GlobalConfig.DependencyResolver.ResolveAll<ITicketBuilder>();
            foreach (var builder in builders)
            {
                if (builder.ToString() == data.Builder)
                {
                    string type = !string.IsNullOrEmpty(data.SubTypeDocument) ? $"{data.TypeDocument}-{data.SubTypeDocument}" : $"{data.TypeDocument}";
                    string documentsId = data.DocumentsId != null && data.DocumentsId.Count > 0 ? string.Join(",", data.DocumentsId) : data.DocumentId;
                    _generalWriter.WriteEntry($"Builder::RecieveAsync(). Activando el builder de: {builder}", EventLogEntryType.Information);
                    _generalWriter.WriteEntry(
                        $"{builder}::BuildAsync(). Nuevo ticket de impresión recibido. {Environment.NewLine}" +
                        $"Evento: {data.PrintEvent}{Environment.NewLine}" +
                        $"Tipo: {type}{Environment.NewLine}" +
                        $"Id colleción printEvents: {documentsId}{Environment.NewLine}", EventLogEntryType.Information);
                    try
                    {
                        var dataToPrint = await builder.BuildAsync(data);
                        await SendToPrintAsync(dataToPrint, data.TypeDocument, data.PrintEvent);
                    }
                    catch (BadImageFormatException imageException)
                    {
                        _generalWriter.WriteEntry(
                            $"{builder}::RecieveAsync(). {imageException.Message}. {Environment.NewLine}" +
                            $"Evento: {data.PrintEvent}{Environment.NewLine}" +
                            $"Tipo: {type}{Environment.NewLine}" +
                            $"PrintEventId: {documentsId}{Environment.NewLine}", EventLogEntryType.Error);
                    }
                    catch (Exception e)
                    {
                        _generalWriter.WriteEntry(
                            $"{builder}::RecieveAsync(). NO se imprimió el ticket de impresión recibido. {Environment.NewLine}" +
                            $"Evento: {data.PrintEvent}{Environment.NewLine}" +
                            $"Tipo: {type}{Environment.NewLine}" +
                            $"PrintEventId: {documentsId}{Environment.NewLine}" +
                            $"Excepción: {e}", EventLogEntryType.Error);
                    }
                }
            }
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

        protected override void OnStart(string[] args)
        {
#if DEBUG
            Debugger.Launch();
#endif
            _generalWriter.WriteEntry("Builder::OnStart(). Iniciando servicio.", EventLogEntryType.Information);
            ConfigureWorker();
            _timer.Start();
            WebApp.Start<Startup>(_signalR);
        }

        protected override void OnStop()
        {
            _timer.Stop();
            if (_adapter != null)
            {
                _adapter.Dispose();
            }
            _generalWriter.WriteEntry("Builder::OnStop(). Deteniendo servicio.", EventLogEntryType.Warning);
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
            _timer = new System.Timers.Timer(interval);
            _timer.Elapsed += ServiceTimer_Tick;
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

        private async Task<List<PrinterConfiguration>> GetSectorsAsync(string storeId, string printEvent)
        {
            List<PrinterConfiguration> sectors = null;
            using (var sqlServerContext = new PrinterContext())
            {
                sectors = await sqlServerContext.GetPrinterConfigurationAsync(storeId, printEvent);
            }
            return sectors;
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

        private async Task PrintAsync(PrintInfo data, string printEvent, List<PrinterConfiguration> sectors, bool isTakeAway = false)
        {
            foreach (var sector in sectors)
            {
                if (isTakeAway)
                {
                    if (!data.Content.ContainsKey("printQR"))
                    {
                        data.Content.Add("printQR", sector.AllowPrintQR);
                    }
                    else
                    {
                        data.Content["printQR"] = sector.AllowPrintQR;
                    }
                }
                bool allowLogo = sector.AllowLogo;
                if (!data.Content.ContainsKey("allowLogo"))
                {
                    data.Content.Add("allowLogo", allowLogo);
                }
                else
                {
                    data.Content["allowLogo"] = allowLogo;
                }
                if (!data.Content.ContainsKey("sector"))
                {
                    data.Content.Add("sector", sector.Name);
                }
                else
                {
                    data.Content["sector"] = sector.Name;
                }

                IFormaterService formatterService = FormaterFactory.Resolve(sector.IsHtml, data.Content, data.Template);
                var ticketData = formatterService.Create();
                await SendTicketAsync(sector, data, printEvent, ticketData.Item2);
                _generalWriter.WriteEntry($"Builder::SendToPrint(). Enviando a imprimir el ticket con la siguiente información. {Environment.NewLine}" +
                    $"Detalles:{Environment.NewLine}" +
                    $"Nombre de la impresora: {sector.Printer}{Environment.NewLine}" +
                    $"Sector de impresión: {sector.Name}{Environment.NewLine}" +
                    $"Hora de impresión: {data.BeforeAt}{Environment.NewLine}" +
                    $"Restaurante: {data.Store.Name}{Environment.NewLine}");
            }
        }

        private async Task PrintAsync(PrintInfo extraData, string clientName, List<SectorItem> sectorsByItems)
        {
            var orderData = (Order)extraData.Content["orderData"];
            foreach (var item in sectorsByItems)
            {
                var itemData = await _itemRepository.GetById<ItemOrder>(item.ItemId, "items");
                var orderItem = orderData.Items.FirstOrDefault(f => f.Id == itemData.Id);
                var viewData = new Dictionary<string, object>()
                {
                    { "title", extraData.Content["title"] },
                    { "orderNumber", extraData.Content["orderNumber"] },
                    { "clientName", clientName},
                    { "item", orderItem }
                };
                if (orderData.OrderType.ToUpper() == OrderTypes.MESA)
                {
                    viewData.Add("tableNumber", extraData.Content["tableNumber"]);
                }
                if (orderData.OrderType.ToUpper() == OrderTypes.TAKEAWAY && !string.IsNullOrEmpty(orderData.GuestComment))
                {
                    viewData.Add("tableNumber", orderData.GuestComment);
                }
                foreach (var sector in item.Sectors.FindAll(f => f.AllowPrinting))
                {
                    if (!viewData.ContainsKey("allowLogo"))
                    {
                        viewData.Add("allowLogo", sector.AllowLogo);
                    }
                    else
                    {
                        viewData["allowLogo"] = sector.AllowLogo;
                    }
                    if (!viewData.ContainsKey("sector"))
                    {
                        viewData.Add("sector", sector.Name);
                    }
                    else
                    {
                        viewData["sector"] = sector.Name;
                    }
                    IFormaterService formatterService = FormaterFactory.Resolve(sector.IsHTML.GetValueOrDefault(), viewData, PrintTemplates.NEW_ITEM_ORDER);
                    var ticketData = formatterService.Create();


                    _generalWriter.WriteEntry($"Builder::SendToPrint(). Enviando a imprimir el ticket con la siguiente información. {Environment.NewLine}" +
                        $"Detalles:{Environment.NewLine}" +
                        $"Nombre de la impresora: {sector.Printer}{Environment.NewLine}" +
                        $"Sector de impresión: {sector.Name}{Environment.NewLine}" +
                        $"Hora de impresión: {extraData.BeforeAt}{Environment.NewLine}" +
                        $"Restaurante: {extraData.Store.Name}{Environment.NewLine}", EventLogEntryType.Information);
                }
            }
        }

        private async Task SendTicketAsync(PrinterConfiguration sector, PrintInfo info, string printEvent, string image)
        {
            try
            {
                var ticketId = Guid.NewGuid();
                _hub.SendToClient(ticketId, printEvent, sector.Id, image, sector.Copies);
                using (var sqlServerContext = new PrinterContext())
                {
                    await sqlServerContext.WriteToHistory(ticketId, info.Store, sector, printEvent, image);
                }
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"Builder::SendTicket(). Ha ocurrido un error en la base de datos. {Environment.NewLine}" +
                        $"Detalles:{Environment.NewLine}" +
                        $"{e.ToString()}", EventLogEntryType.Error);
            }
        }

        private async Task SendToPrintAsync(PrintInfo data, string typeDocument, string printEvent)
        {
            if (data == null || data.Content.Count == 0)
            {
                return;
            }
            var sectors = new List<PrinterConfiguration>();
            if (typeDocument != PrintTypes.ORDER)
            {
                sectors = await GetSectorsAsync(data.Store.Id, printEvent);
                await PrintAsync(data, printEvent, sectors);
            }
            else
            {
                var orderData = (Order)data.Content["orderData"];
                //string clientName = orderData.UserName;
                bool isTakeAway = orderData.OrderType == SubOrderPrintTypes.ORDER_TA;
                switch (printEvent)
                {
                    case PrintEvents.NEW_TAKE_AWAY:
                        if (!string.IsNullOrEmpty(orderData.GuestComment))
                        {
                            data.Content.Add("tableNumber", orderData.GuestComment);
                        }
                        sectors = await GetSectorsAsync(data.Store.Id, PrintEvents.NEW_TAKE_AWAY);
                        await PrintAsync(data, printEvent, sectors, true);
                        //TODO: Modificar la impresión de platos.
                        //var items = ItemExtensions.GetPrintSectorByItems(orderData.Items, _itemRepository);
                        //await PrintAsync(data, clientName, items);
                        break;
                    case PrintEvents.NEW_TABLE_ORDER:
                        await PrintAsync(data, PrintEvents.NEW_TABLE_ORDER, sectors);
                        //TODO: Modificar la impresión de platos.
                        //var sectorsByItems = ItemExtensions.GetPrintSectorByItems(orderData.Items, _itemRepository);
                        //await PrintAsync(data, clientName, sectorsByItems);
                        break;
                    case PrintEvents.REPRINT_ORDER:
                        printEvent = orderData.OrderType == SubOrderPrintTypes.ORDER_TA ? PrintEvents.NEW_TAKE_AWAY : PrintEvents.NEW_TABLE_ORDER;
                        sectors = await GetSectorsAsync(data.Store.Id, printEvent);
                        await PrintAsync(data, printEvent, sectors, isTakeAway);
                        break;
                    case PrintEvents.ORDER_CANCELLED:
                        sectors = await GetSectorsAsync(data.Store.Id, printEvent);
                        await PrintAsync(data, printEvent, sectors, isTakeAway);
                        break;
                }
            }
        }
        private void ServiceTimer_Tick(object sender, ElapsedEventArgs e)
        {
            try
            {
                long memSize = 0L;
                _tickCounter++;
                if (_tickCounter % 3600L == 0L)
                {
                    _timer.Stop();
                    //Proceso de mantenimiento de .NET.
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.WaitForFullGCComplete();
                    memSize = GC.GetTotalMemory(true);
                    _generalWriter.WriteEntry($"Builder::ServiceTimer_Tick(). Se ha liberado {memSize / 1024 / 1024 } MB.", EventLogEntryType.Warning);
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
