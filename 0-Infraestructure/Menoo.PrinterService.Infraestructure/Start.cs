using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Database.Firebase;
using Menoo.PrinterService.Infraestructure.Database.SqlServer;
using Menoo.PrinterService.Infraestructure.Repository;

namespace Menoo.PrinterService.Infraestructure
{
    [OnStartUp(Module = "Infrastructure", Order = int.MinValue)]
    public class Start
    {
        public Start()
        {
            var dependencyResolver = GlobalConfig.DependencyResolver;
            dependencyResolver.Register(GetInstanceFirebase);
            dependencyResolver.Register(() => new SqlServerContext());
            dependencyResolver.Register(() => {
                var firebaseDb = GetInstanceFirebase();
                var storeRepository = new StoreRepository(firebaseDb);
                return storeRepository;
            });
        }

        static FirestoreDb GetInstanceFirebase()
        {
            bool isDebug = bool.Parse(GlobalConfig.ConfigurationManager.GetSetting("isDebug"));
            var instance = FirebaseContext.GetInstance(isDebug);
            return instance;
        }
    }
}
