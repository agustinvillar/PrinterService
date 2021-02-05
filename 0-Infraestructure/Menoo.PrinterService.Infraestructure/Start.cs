using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Database.Firebase;
using Menoo.PrinterService.Infraestructure.Database.SqlServer;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
using Menoo.PrinterService.Infraestructure.Repository;
using System.Diagnostics;

namespace Menoo.PrinterService.Infraestructure
{
    [OnStartUp(Module = "Infrastructure", Order = int.MinValue)]
    public class Start
    {
        public Start()
        {
            var dependencyResolver = GlobalConfig.DependencyResolver;
            dependencyResolver.Register(ConfigureListenerEventLog, "listener");
            dependencyResolver.Register(ConfigureBuilderEventLog, "builder");
            dependencyResolver.Register(GetInstanceFirebase);
            dependencyResolver.Register(() => {
                var firebaseDb = GetInstanceFirebase();
                var storeRepository = new StoreRepository(firebaseDb);
                return storeRepository;
            });
            dependencyResolver.Register(() => {
                var firebaseDb = GetInstanceFirebase();
                var ticketRepository = new TicketRepository(firebaseDb);
                return ticketRepository;
            });
            dependencyResolver.Register<IPublisherService, PublisherService>();
        }

        static EventLog ConfigureListenerEventLog()
        {
            string sourceName = GlobalConfig.ConfigurationManager.GetSetting("serviceListenerSourceName");
            if (!EventLog.SourceExists(sourceName))
            {
                EventLog.CreateEventSource(sourceName, sourceName);
            }
            var generalWriter = new EventLog { Log = sourceName, Source = sourceName, EnableRaisingEvents = true };
            return generalWriter;
        }

        static EventLog ConfigureBuilderEventLog()
        {
            string sourceName = GlobalConfig.ConfigurationManager.GetSetting("serviceBuilderSourceName");
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
