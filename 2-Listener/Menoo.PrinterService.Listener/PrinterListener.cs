using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Listener
{
    public partial class PrinterListener : ServiceBase
    {
        private readonly EventLog _generalWriter;

        public PrinterListener()
        {
            InitializeService();
            _generalWriter = GlobalConfig.DependencyResolver.ResolveByName<EventLog>("listener");
        }

        protected override void OnStart(string[] args)
        {
            _generalWriter.WriteEntry("PrinterListener::OnStart(). Iniciando servicio.", EventLogEntryType.Information);
            var listeners = GlobalConfig.DependencyResolver.ResolveAll<IFirebaseListener>();
            foreach (var listener in listeners)
            {
                _generalWriter.WriteEntry($"PrinterListener::OnStart(). Activando el listener de: {listener.ToString()}", EventLogEntryType.Information);
                listener.Listen();
            }
        }

        protected override void OnShutdown()
        {
            _generalWriter.WriteEntry("PrinterListener::OnShutdown(). Apagando servicio.", EventLogEntryType.Warning);
            base.OnShutdown();
        }

        protected override void OnStop()
        {
            _generalWriter.WriteEntry("PrinterListener::OnStop(). Deteniendo servicio.", EventLogEntryType.Warning);
        }

        private void InitializeService()
        {
            try
            {
                AutoLog = false;
                CanHandlePowerEvent = true;
                CanPauseAndContinue = false;
                CanShutdown = true;
                ServiceName = GlobalConfig.ConfigurationManager.GetSetting("serviceListenerName");
            }
            catch (Exception ex)
            {
                EventLog eventLog = new EventLog
                {
                    Source = GlobalConfig.ConfigurationManager.GetSetting("defaultLog")
                };
                eventLog.WriteEntry($"PrinterListener::InitializeService()" + Environment.NewLine + ex, EventLogEntryType.Error);
                eventLog.Dispose();
            }
        }
    }
}
