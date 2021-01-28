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

        private void ConfigureEventLog()
        {
            if (!EventLog.SourceExists(Settings.ServiceSourceName))
            {
                EventLog.CreateEventSource(Settings.ServiceSourceName, Settings.ServiceSourceName);
            }
            generalWriter = new EventLog { Log = Settings.ServiceSourceName, Source = Settings.ServiceSourceName, EnableRaisingEvents = true };
        }

        private void InitializeService()
        {
            try
            {
                AutoLog = false;
                CanHandlePowerEvent = true;
                CanPauseAndContinue = false;
                CanShutdown = true;
                ServiceName = Settings.ServiceName;
                ConfigureEventLog();
            }
            catch (Exception ex)
            {
                EventLog eventLog = new EventLog
                {
                    Source = Settings.DefaultLog
                };
                eventLog.WriteEntry($"PrinterListener::InitializeService()" + Environment.NewLine + ex, EventLogEntryType.Error);
                eventLog.Dispose();
            }
        }
    }
}
