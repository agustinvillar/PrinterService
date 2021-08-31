using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Extensions;
using Menoo.PrinterService.Infraestructure.Interceptors;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Menoo.Printer.Listener.Orders
{
    [Handler]
    public class OrderListener : IFirebaseListener
    {
        private readonly FirestoreDb _firestoreDb;

        private readonly OnActionRecieve _interceptor;

        private readonly EventLog _generalWriter;

        private readonly IPublisherService _publisherService;

        private readonly int _delayTask;

        public OrderListener(
            FirestoreDb firestoreDb,
            IPublisherService publisherService)
        {
            _interceptor = new OnActionRecieve(PrintBuilder.ORDER_BUILDER);
            _firestoreDb = firestoreDb;
            _publisherService = publisherService;
            _generalWriter = GlobalConfig.DependencyResolver.ResolveByName<EventLog>("listener");
            _delayTask = int.Parse(GlobalConfig.ConfigurationManager.GetSetting("listenerDelay"));
        }

        public void Listen()
        {
            //Reimpresión de orden
            _firestoreDb.Collection("rePrint")
                .Listen(RePrintOrder);

            _firestoreDb.Collection("printEvent")
                  .Listen(OnRecieve);
        }

        public override string ToString()
        {
            return PrintListeners.ORDER_LISTENER;
        }

        #region listeners
        private void OnCancelled(Tuple<string, PrintMessage> ticket)
        {
            try
            {
                _publisherService.PublishAsync(ticket.Item1, ticket.Item2).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"OrderListener::OnCancelled(). No se envió la orden a la cola de impresión.{Environment.NewLine} Detalles: {e}{JsonConvert.SerializeObject(ticket.Item2, Formatting.Indented)}", EventLogEntryType.Error);
            }
        }

        private void OnOrderCreated(Tuple<string, PrintMessage> ticket)
        {
            try
            {
                _publisherService.PublishAsync(ticket.Item1, ticket.Item2).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"OrderListener::OnOrderCreated(). No se envió la orden a la cola de impresión. {Environment.NewLine} Detalles: {e}{JsonConvert.SerializeObject(ticket.Item2, Formatting.Indented)}", EventLogEntryType.Error);
            }
        }

        private void OnTakeAwayCreated(Tuple<string, PrintMessage> ticket)
        {
            try
            {
                _publisherService.PublishAsync(ticket.Item1, ticket.Item2).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"OrderListener::OnTakeAwayCreated(). No se envió la orden TA a la cola de impresión. {Environment.NewLine} Detalles: {e}{JsonConvert.SerializeObject(ticket.Item2, Formatting.Indented)}", EventLogEntryType.Error);
            }
        }

        private void RePrintOrder(QuerySnapshot snapshot)
        {
            bool isEntry = _interceptor.OnEntry(snapshot, true);
            if (!isEntry)
            {
                return;
            }
            var documentReference = snapshot.Single();
            var message = PrintExtensions.GetReprintMessage(documentReference);
            try
            {
                _publisherService.PublishAsync(message.Item1, message.Item2).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"OrderListener::RePrintOrder(). No se envió la orden a la cola de impresión. {Environment.NewLine} Detalles: {e}{Environment.NewLine} {JsonConvert.SerializeObject(message, Formatting.Indented)}", EventLogEntryType.Error);
            }
            _interceptor.OnExit(snapshot, true);
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
            if (message.Item2.PrintEvent == PrintEvents.NEW_TABLE_ORDER)
            {
                OnOrderCreated(message);
            }
            else if (message.Item2.PrintEvent == PrintEvents.NEW_TAKE_AWAY)
            {
                OnTakeAwayCreated(message);
            }
            else if (message.Item2.PrintEvent == PrintEvents.ORDER_CANCELLED) 
            {
                OnCancelled(message);
            }
            _interceptor.OnExit(snapshot);
            Thread.Sleep(_delayTask);
        }
        #endregion
    }
}
