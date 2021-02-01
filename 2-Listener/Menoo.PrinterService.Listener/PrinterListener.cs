using Menoo.PrinterService.Infraestructure;
using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace Menoo.PrinterService.Listener
{
    public partial class PrinterListener : ServiceBase
    {
        /// <summary>Logger mediante visor de eventos.</summary>
        public EventLog generalWriter;

        public PrinterListener()
        {
            generalWriter = null;
            InitializeService();
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnShutdown()
        {
            generalWriter.WriteEntry("PrinterListener::OnShutdown(). Apagando servicio.", EventLogEntryType.Warning);
            base.OnShutdown();
        }

        protected override void OnStop()
        {
        }

        private void InitializeService()
        {
            try
            {
                AutoLog = false;
                CanHandlePowerEvent = true;
                CanPauseAndContinue = false;
                CanShutdown = true;
                ServiceName = GlobalConfig.ConfigurationManager.GetSetting("ServiceName");
                generalWriter = Utils.ConfigureEventLog(GlobalConfig.ConfigurationManager);
            }
            catch (Exception ex)
            {
                EventLog eventLog = new EventLog
                {
                    Source = GlobalConfig.ConfigurationManager.GetSetting("DefaultLog")
                };
                eventLog.WriteEntry($"PrinterListener::InitializeService()" + Environment.NewLine + ex, EventLogEntryType.Error);
                eventLog.Dispose();
            }
        }
    }
}
