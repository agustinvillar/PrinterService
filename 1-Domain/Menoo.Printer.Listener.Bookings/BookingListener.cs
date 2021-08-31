using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema;
using Menoo.PrinterService.Infraestructure.Extensions;
using Menoo.PrinterService.Infraestructure.Interceptors;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Menoo.Printer.Listener.Bookings
{
    [Handler]
    public class BookingListener : IFirebaseListener
    {
        private readonly FirestoreDb _firestoreDb;

        private readonly EventLog _generalWriter;

        private readonly IPublisherService _publisherService;

        private readonly int _delayTask;

        public BookingListener(
            FirestoreDb firestoreDb,
            IPublisherService publisherService)
        {
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
            return PrintListeners.BOOKING_LISTENER;
        }

        #region listener
        private void OnAcepted(Tuple<string, PrintMessage> ticket)
        {
            try
            {
                _publisherService.PublishAsync(ticket.Item1, ticket.Item2).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"BookingListener::OnAcepted(). Ha ocurrido un error al capturar nuevas reservas. {Environment.NewLine} Detalles: {e} {JsonConvert.SerializeObject(ticket.Item2, Formatting.Indented)}", EventLogEntryType.Error);
            }
        }

        private void OnCancelled(Tuple<string, PrintMessage> ticket)
        {
            try
            {
                _publisherService.PublishAsync(ticket.Item1, ticket.Item2).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"BookingListener::OnCancelled(). No se envió la reserva a la cola de impresión. {Environment.NewLine} Detalles: {e} {JsonConvert.SerializeObject(ticket.Item2, Formatting.Indented)}", EventLogEntryType.Error);
            }
        }

        [OnActionRecieve]
        private void OnRecieve(QuerySnapshot snapshot)
        {
            var documentReference = snapshot.Single();
            var message = PrintExtensions.GetMessagePrintType(documentReference);
            if (message.Item2.PrintEvent == PrintEvents.NEW_BOOKING)
            {
                OnAcepted(message);
            }
            else if (message.Item2.PrintEvent == PrintEvents.CANCELED_BOOKING)
            {
                OnCancelled(message);
            }
            Thread.Sleep(_delayTask);
        }
        #endregion
    }
}
