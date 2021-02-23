using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Database.Firebase;
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
            string listenerLog = GlobalConfig.ConfigurationManager.GetSetting("serviceListenerName");
            string builderLog = GlobalConfig.ConfigurationManager.GetSetting("serviceBuilderName");
            if (!string.IsNullOrEmpty(listenerLog)) 
            {
                dependencyResolver.Register(() => {
                    var log = ConfigureListenerEventLog();
                    return log;
                }, "listener");
            }
            if (!string.IsNullOrEmpty(builderLog))
            {
                dependencyResolver.Register(() => {
                    var log = ConfigureBuilderEventLog();
                    return log;
                }, "builder");
            }
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
            dependencyResolver.Register(() => {
                var firebaseDb = GetInstanceFirebase();
                var userRepository = new UserRepository(firebaseDb);
                return userRepository;
            });
            dependencyResolver.Register<IPublisherService, PublisherService>();
        }

        static EventLog ConfigureListenerEventLog()
        {
            string sourceListenerName = GlobalConfig.ConfigurationManager.GetSetting("serviceListenerName");
            string logListenerName = GlobalConfig.ConfigurationManager.GetSetting("eventListenerLog");
            //if (!EventLog.SourceExists(sourceListenerName))
            //{
            //    EventLog.CreateEventSource(sourceListenerName, logListenerName);
            //}
            //logListenerName = EventLog.LogNameFromSourceName(sourceListenerName, ".");
            var generalWriter = new EventLog { Log = logListenerName, Source = sourceListenerName, EnableRaisingEvents = true };
            return generalWriter;
        }

        static EventLog ConfigureBuilderEventLog()
        {
            string sourceBuilderName = GlobalConfig.ConfigurationManager.GetSetting("serviceBuilderName");
            string logBuilderName = GlobalConfig.ConfigurationManager.GetSetting("eventBuilderLog");
            //if (!EventLog.SourceExists(sourceBuilderName))
            //{
            //    EventLog.CreateEventSource(sourceBuilderName, logBuilderName);
            //}
            var generalWriter = new EventLog { Log = logBuilderName, Source = sourceBuilderName, EnableRaisingEvents = true };
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
