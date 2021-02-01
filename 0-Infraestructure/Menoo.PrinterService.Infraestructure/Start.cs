using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Database;
using Menoo.PrinterService.Infraestructure.Database.Firebase;

namespace Menoo.PrinterService.Infraestructure
{
    [OnStartUp(Module = "Infrastructure", Order = int.MinValue)]
    public class Start
    {
        public Start()
        {
            var dependencyResolver = GlobalConfig.DependencyResolver;
            bool isDebug = bool.Parse(GlobalConfig.ConfigurationManager.GetSetting("isDebug"));

            dependencyResolver.Register(GetInstanceFirebase);
        }

        static FirestoreDb GetInstanceFirebase()
        {
            bool isDebug = bool.Parse(GlobalConfig.ConfigurationManager.GetSetting("isDebug"));
            var instance = FirebaseContext.GetInstance(isDebug);
            return instance;
        }
    }
}
