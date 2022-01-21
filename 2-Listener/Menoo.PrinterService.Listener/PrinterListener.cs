using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Interfaces;
using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Timers;

namespace Menoo.PrinterService.Listener
{
    public partial class PrinterListener : ServiceBase
    {
        private long _tickCounter;

        private Timer _timer;

        private readonly EventLog _generalWriter;

        public PrinterListener()
        {
            InitializeService();
            _generalWriter = GlobalConfig.DependencyResolver.ResolveByName<EventLog>("listener");
        }

        protected override void OnStart(string[] args)
        {
            //Debugger.Launch();
            _generalWriter.WriteEntry("PrinterListener::OnStart(). Iniciando servicio.", EventLogEntryType.Information);
            Listen(true);
            _timer.Start();
        }

        protected override void OnShutdown()
        {
            _generalWriter.WriteEntry("PrinterListener::OnShutdown(). Apagando servicio.", EventLogEntryType.Warning);
            base.OnShutdown();
        }

        protected override void OnStop()
        {
            _timer.Stop();
            _generalWriter.WriteEntry("PrinterListener::OnStop(). Deteniendo servicio.", EventLogEntryType.Warning);
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

        private void Listen(bool onStart = false) 
        {
            var listener = GlobalConfig.DependencyResolver.ResolveAll<IFirebaseListener>().FirstOrDefault();
            _generalWriter.WriteEntry($"PrinterListener::OnStart(). Activando el listener de: {listener.ToString()}", EventLogEntryType.Information);
            listener.Listen(onStart);
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
                ConfigureTimer();
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

        private void ServiceTimer_Tick(object sender, ElapsedEventArgs e)
        {
            try
            {
                long memSize = 0L;
                _tickCounter++;
                if (_tickCounter % 60L == 0L)
                {
                    memSize = GC.GetTotalMemory(true);
                    _generalWriter.WriteEntry($"Listener::ServiceTimer_Tick(). Se ha liberado {memSize / 1024 }.", EventLogEntryType.Warning);
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
