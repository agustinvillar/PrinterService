using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Database.SqlServer;
using Menoo.PrinterService.Infraestructure.Repository;
using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace Menoo.PrinterService.Listener
{
    public partial class PrinterListener : ServiceBase
    {
        private readonly FirestoreDb _firestoreDb;

        private readonly SqlServerContext _sqlServerContext;

        private readonly StoreRepository _storeRepository;

        private readonly EventLog _generalWriter;

        public PrinterListener(
            FirestoreDb firestoreDb,
            EventLog generalWriter,
            SqlServerContext sqlServerDb,
            StoreRepository storeRepository)
        {
            InitializeService();
            _firestoreDb = firestoreDb;
            _sqlServerContext = sqlServerDb;
            _storeRepository = storeRepository;
            _generalWriter = generalWriter;
        }

        protected override void OnStart(string[] args)
        {
            _generalWriter.WriteEntry("PrinterListener::OnStart(). Iniciando servicio.", EventLogEntryType.Information);
            Debugger.Launch();
            var result = _storeRepository.GetAllAsync<Store>(collection: "stores").GetAwaiter().GetResult();
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
                ServiceName = GlobalConfig.ConfigurationManager.GetSetting("ServiceName");
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
