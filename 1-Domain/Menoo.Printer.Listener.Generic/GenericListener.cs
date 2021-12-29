using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Extensions;
using Menoo.PrinterService.Infraestructure.Interceptors;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
using Menoo.PrinterService.Infraestructure.Repository;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;

namespace Menoo.Printer.Listener.Generic
{
    [Handler]
    public class GenericListener : IFirebaseListener
    {
        private readonly FirestoreDb _firestoreDb;

        private readonly OnActionRecieve _interceptor;

        private readonly EventLog _generalWriter;

        private readonly IPublisherService _publisherService;

        public GenericListener(
            OnActionRecieve interceptor,
            FirestoreDb firestoreDb,
            TicketRepository ticketRepository,
            IPublisherService publisherService)
        {
            _firestoreDb = firestoreDb;
            _interceptor = interceptor;
            _publisherService = publisherService;
            _generalWriter = GlobalConfig.DependencyResolver.ResolveByName<EventLog>("listener");
        }

        public void Listen()
        {
            _firestoreDb.Collection("rePrint")
                    .Listen(RePrintOrder);
            _firestoreDb.Collection("printEvent")
                    .Listen(OnRecieve);
        }

        public override string ToString()
        {
            return PrintListeners.GENERIC_LISTENER;
        }

        #region private methods
        private void OnBookingAcepted(Tuple<string, PrintMessage> ticket)
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

        private void OnBookingCancelled(Tuple<string, PrintMessage> ticket)
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

        private void OnOrderCancelled(Tuple<string, PrintMessage> ticket)
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

        private void OnOpenFamily(Tuple<string, PrintMessage> ticket)
        {
            try
            {
                _publisherService.PublishAsync(ticket.Item1, ticket.Item2).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"TablesListener::OnTakeAwayCreated(). No se envió la apertura de mesa a la cola de impresión. {Environment.NewLine} Detalles: {e}{Environment.NewLine} {JsonConvert.SerializeObject(ticket.Item2, Formatting.Indented)}", EventLogEntryType.Error);
            }
        }

        private void OnRequestPayment(Tuple<string, PrintMessage> ticket)
        {
            try
            {
                _publisherService.PublishAsync(ticket.Item1, ticket.Item2).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"TablesListener::OnRequestPayment(). No se envió la solicitud de pago de la mesa a la cola de impresión. {Environment.NewLine} Detalles: {e}{Environment.NewLine} {JsonConvert.SerializeObject(ticket.Item2, Formatting.Indented)}", EventLogEntryType.Error);
            }
        }

        private void OnTableClose(Tuple<string, PrintMessage> ticket)
        {
            try
            {
                _publisherService.PublishAsync(ticket.Item1, ticket.Item2).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"TablesListener::OnClose(). No se envió el cierre de mesa a la cola de impresión. {Environment.NewLine} Detalles: {e}{Environment.NewLine} {JsonConvert.SerializeObject(ticket.Item2, Formatting.Indented)}", EventLogEntryType.Error);
            }
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
            switch (message.Item2.PrintEvent)
            {
                case "NEW_TABLE_ORDER":
                    OnOrderCreated(message);
                    break;
                case "NEW_TAKE_AWAY":
                    OnTakeAwayCreated(message);
                    break;
                case "ORDER_CANCELLED":
                    OnOrderCancelled(message);
                    break;
                case "TABLE_OPENED":
                    OnOpenFamily(message);
                    break;
                case "TABLE_CLOSED":
                    OnTableClose(message);
                    break;
                case "REQUEST_PAYMENT":
                    OnRequestPayment(message);
                    break;
                case "NEW_BOOKING":
                    OnBookingAcepted(message);
                    break;
                case "CANCELED_BOOKING":
                    OnBookingCancelled(message);
                    break;
            }
            _interceptor.OnExit(snapshot);
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
        #endregion
    }
}
