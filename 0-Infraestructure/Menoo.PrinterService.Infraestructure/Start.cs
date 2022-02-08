using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Database.Firebase;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Repository;
using Menoo.PrinterService.Infraestructure.Services;
using System.Diagnostics;

namespace Menoo.PrinterService.Infraestructure
{
    [OnStartUp(Module = "Infrastructure", Order = int.MinValue)]
    public class Start
    {
        public Start()
        {
            var dependencyResolver = GlobalConfig.DependencyResolver;
            string builderLog = GlobalConfig.ConfigurationManager.GetSetting("serviceBuilderName");
            var firebaseDb = GetInstanceFirebase();
            if (!string.IsNullOrEmpty(builderLog))
            {
                dependencyResolver.Register(() => {
                    var log = ConfigureBuilderEventLog();
                    return log;
                }, "builder");
            }
            dependencyResolver.Register(GetInstanceFirebase);
            dependencyResolver.Register(() => {
                var storeRepository = new StoreRepository(firebaseDb);
                return storeRepository;
            });
            dependencyResolver.Register(() => {
                var ticketRepository = new TicketRepository(firebaseDb);
                return ticketRepository;
            });
            dependencyResolver.Register(() => {
                var userRepository = new UserRepository(firebaseDb);
                return userRepository;
            });
            dependencyResolver.Register(() => {

                var tableOpeningRepository = new TableOpeningFamilyRepository(firebaseDb);
                return tableOpeningRepository;
            });
            dependencyResolver.Register(() => {
                var paymentRepository = new PaymentRepository(firebaseDb);
                return paymentRepository;
            });
            dependencyResolver.Register<IFirebaseStorage, FirebaseStorageService>();
        }

        static EventLog ConfigureBuilderEventLog()
        {
            string sourceBuilderName = GlobalConfig.ConfigurationManager.GetSetting("serviceBuilderName");
            string logBuilderName = GlobalConfig.ConfigurationManager.GetSetting("eventBuilderLog");

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
