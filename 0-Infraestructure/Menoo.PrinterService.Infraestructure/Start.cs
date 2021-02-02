using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Database.Firebase;
using Menoo.PrinterService.Infraestructure.Database.SqlServer;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
using Menoo.PrinterService.Infraestructure.Repository;
using Rebus.Activation;
using System.Diagnostics;

namespace Menoo.PrinterService.Infraestructure
{
    [OnStartUp(Module = "Infrastructure", Order = int.MinValue)]
    public class Start
    {
        public Start()
        {
            var dependencyResolver = GlobalConfig.DependencyResolver;
            dependencyResolver.Register(ConfigureEventLog);
            dependencyResolver.Register(GetInstanceFirebase);
            dependencyResolver.Register(() => new SqlServerContext());
            dependencyResolver.Register(() => {
                var firebaseDb = GetInstanceFirebase();
                var storeRepository = new StoreRepository(firebaseDb);
                return storeRepository;
            });
            dependencyResolver.Register<IPublisherService, PublisherService>();
        }

        static EventLog ConfigureEventLog()
        {
            string sourceName = GlobalConfig.ConfigurationManager.GetSetting("serviceSourceName");
            if (!EventLog.SourceExists(sourceName))
            {
                EventLog.CreateEventSource(sourceName, sourceName);
            }
            var generalWriter = new EventLog { Log = sourceName, Source = sourceName, EnableRaisingEvents = true };
            return generalWriter;
        }

        static FirestoreDb GetInstanceFirebase()
        {
            bool isDebug = bool.Parse(GlobalConfig.ConfigurationManager.GetSetting("isDebug"));
            var instance = FirebaseContext.GetInstance(isDebug);
            return instance;
        }
    }
}
