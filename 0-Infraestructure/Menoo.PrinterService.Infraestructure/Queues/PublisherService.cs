using Menoo.PrinterService.Infraestructure.Interfaces;
using Rebus.Activation;
using Rebus.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Queues
{
    public sealed class PublisherService : IPublisherService, IDisposable
    {
        private readonly string _queueName;

        private readonly string _queueConnectionString;

        private readonly BuiltinHandlerActivator _adapter;

        private readonly EventLog _logger;

        public PublisherService(EventLog logger)
        {
            _logger = logger;
            _queueName = GlobalConfig.ConfigurationManager.GetSetting("queueName");
            _queueConnectionString = GlobalConfig.ConfigurationManager.GetSetting("queueConnectionString");
            _adapter = new BuiltinHandlerActivator();
            Configure();
        }

        public void Dispose()
        {
            if (_adapter != null) 
            {
                _adapter.Dispose();
            }
        }

        public async Task PublishAsync(PrintMessage data, Dictionary<string, string> extras = null)
        {
            string type = !string.IsNullOrEmpty(data.SubTypeDocument) ? $"{data.TypeDocument}-{data.SubTypeDocument}" : $"{data.TypeDocument}";
            _logger.WriteEntry(
                $"PublisherService::PublishAsync(). Enviado mensaje a la cola printer {Environment.NewLine}" +
                $"Evento: {data.PrintEvent}{Environment.NewLine}" +
                $"Tipo: {type}{Environment.NewLine}" +
                $"FirebaseId: {data.DocumentId}{Environment.NewLine}", EventLogEntryType.Information);
            //await _adapter.Bus.Publish(data, extras);
        }

        private void Configure()
        {
            Rebus.Config.Configure.With(_adapter)
                .Logging(l => l.Serilog())
                .Transport(t => t.UseRabbitMqAsOneWayClient(_queueConnectionString))
                .Start();
        }
    }
}
