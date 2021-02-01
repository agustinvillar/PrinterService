using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Database.SqlServer;
using Menoo.PrinterService.Infraestructure.Repository;
using System.Diagnostics;
using System.ServiceProcess;

namespace Menoo.PrinterService.Listener
{
    static class Program
    {
        static void Main()
        {
            Boostrapper.Bootstrap();

            #region Dependencies
            var firestoreDb = GlobalConfig.DependencyResolver.Resolve<FirestoreDb>();
            var logger = GlobalConfig.DependencyResolver.Resolve<EventLog>();
            var sqlServerDb = GlobalConfig.DependencyResolver.Resolve<SqlServerContext>();
            var storeRepository = GlobalConfig.DependencyResolver.Resolve<StoreRepository>();
            #endregion
            
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new PrinterListener(firestoreDb, logger, sqlServerDb, storeRepository)
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
