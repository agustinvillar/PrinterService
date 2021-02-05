using Menoo.PrinterService.Infraestructure;
using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace Menoo.PrinterService.Builder
{
    public partial class Builder : ServiceBase
    {
        private readonly EventLog _generalWriter;

        public Builder()
        {
            InitializeService();
            _generalWriter = GlobalConfig.DependencyResolver.ResolveByName<EventLog>("builder");
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnShutdown()
        {
            _generalWriter.WriteEntry("Builder::OnShutdown(). Apagando servicio.", EventLogEntryType.Warning);
            base.OnShutdown();
        }

        protected override void OnStop()
        {
            _generalWriter.WriteEntry("Builder::OnStop(). Deteniendo servicio.", EventLogEntryType.Warning);
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
