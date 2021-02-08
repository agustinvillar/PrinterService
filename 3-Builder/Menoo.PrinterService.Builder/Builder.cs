using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
using Rebus.Activation;
using Rebus.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Builder
{
    public partial class Builder : ServiceBase, ISubscriptionService
    {
        private readonly string _queueName;

        private readonly string _queueConnectionString;

        private readonly int _queueDelay;

        private readonly EventLog _generalWriter;

        private readonly BuiltinHandlerActivator _adapter;

        public Builder()
        {
            InitializeService();
            _adapter = new BuiltinHandlerActivator();
            _queueName = GlobalConfig.ConfigurationManager.GetSetting("queueName");
            _queueConnectionString = GlobalConfig.ConfigurationManager.GetSetting("queueConnectionString");
            _queueDelay = int.Parse(GlobalConfig.ConfigurationManager.GetSetting("queueDelay"));
            _generalWriter = GlobalConfig.DependencyResolver.ResolveByName<EventLog>("builder");
        }

        public async Task RecieveAsync(PrintMessage data, Dictionary<string, string> extras = null)
        {
            var builders = GlobalConfig.DependencyResolver.ResolveAll<ITicketBuilder>();
            foreach (var builder in builders)
            {
                _generalWriter.WriteEntry($"Builder::OnStart(). Activando el builder de: {builder.ToString()}", EventLogEntryType.Information);
                if (builder.ToString() == data.Builder) 
                {
                    string type = !string.IsNullOrEmpty(data.SubTypeDocument) ? $"{data.TypeDocument}-{data.SubTypeDocument}" : $"{data.TypeDocument}";
                    _generalWriter.WriteEntry(
                        $"{builder.ToString()}::BuildAsync(). Nuevo ticket de impresión recibido. {Environment.NewLine}" +
                        $"Evento: {data.PrintEvent}{Environment.NewLine}" +
                        $"Tipo: {type}{Environment.NewLine}" +
                        $"FirebaseId: {data.DocumentId}{Environment.NewLine}", EventLogEntryType.Information);
                    try
                    {
                        await builder.BuildAsync(data);
                    }
                    catch (Exception e) 
                    {
                        _generalWriter.WriteEntry(
                            $"{builder.ToString()}::RecieveAsync(). NO se imprimió el ticket de impresión recibido. {Environment.NewLine}" +
                            $"Evento: {data.PrintEvent}{Environment.NewLine}" +
                            $"Tipo: {type}{Environment.NewLine}" +
                            $"FirebaseId: {data.DocumentId}{Environment.NewLine}" +
                            $"Excepción: {e.ToString()}", EventLogEntryType.Error);
                    }
                    break;
                }
            }
            await Task.Delay(_queueDelay);
        }

        protected override void OnStart(string[] args)
        {
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
            _generalWriter.WriteEntry("Builder::OnStop(). Deteniendo servicio.", EventLogEntryType.Warning);
        }

        private void ConfigureWorker()
        {
            _adapter.Handle<PrintMessage>(async message =>
            {
                await RecieveAsync(message);
            });
            Configure.With(_adapter)
                .Logging(l => l.Serilog())
                .Transport(t => t.UseRabbitMq(_queueConnectionString, _queueName))
                .Options(o => o.SetMaxParallelism(1))
                .Options(o => o.SetNumberOfWorkers(1))
                .Start();
            _adapter.Bus.Subscribe<PrintMessage>().GetAwaiter().GetResult();
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
            }
            catch (Exception ex)
            {
                EventLog eventLog = new EventLog
                {
                    Source = GlobalConfig.ConfigurationManager.GetSetting("defaultLog")
                };
                eventLog.WriteEntry($"Builder::InitializeService()" + Environment.NewLine + ex, EventLogEntryType.Error);
                eventLog.Dispose();
            }
        }
    }
}
