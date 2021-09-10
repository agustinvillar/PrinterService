using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Extensions;
using Menoo.PrinterService.Infraestructure.Interceptors;
using Menoo.PrinterService.Infraestructure.Interfaces;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Menoo.Printer.Listener.Generic
{
    public class GenericListener : IFirebaseListener
    {
        private readonly FirestoreDb _firestoreDb;

        private readonly OnActionRecieve _interceptor;

        private readonly EventLog _generalWriter;

        private readonly IPublisherService _publisherService;

        private readonly int _delayTask;

        public GenericListener(
            FirestoreDb firestoreDb,
            IPublisherService publisherService)
        {
            _interceptor = new OnActionRecieve();
            _firestoreDb = firestoreDb;
            _publisherService = publisherService;
            _generalWriter = GlobalConfig.DependencyResolver.ResolveByName<EventLog>("listener");
            _delayTask = int.Parse(GlobalConfig.ConfigurationManager.GetSetting("listenerDelay"));
        }

        public void Listen()
        {
            _firestoreDb.Collection("printEvent")
                  .Listen(OnRecieve);
        }

        public override string ToString()
        {
            return PrintListeners.GENERIC_LISTENER;
        }

        private void OnRecieve(QuerySnapshot snapshot)
        {
            bool isEntry = _interceptor.OnEntry(snapshot);
            if (!isEntry)
            {
                return;
            }
            var documentReference = snapshot.Documents.OrderByDescending(o => o.CreateTime).FirstOrDefault();
            var message = PrintExtensions.GetMessagePrintType(documentReference);
            _interceptor.OnExit(snapshot);
            Thread.Sleep(_delayTask);
        }
    }
}
