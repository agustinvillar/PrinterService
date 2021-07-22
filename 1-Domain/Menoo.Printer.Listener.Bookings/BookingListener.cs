using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema;
using Menoo.PrinterService.Infraestructure.Extensions;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
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

        private string _today;

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
        private void OnAcepted(List<Tuple<string, PrintMessage>> tickets)
        {
            try
            {
                var ticketsToPrint = GetBookingsToPrint(tickets, true, false);
                foreach (var ticket in ticketsToPrint)
                {
                    try
                    {
                        _publisherService.PublishAsync(ticket.Item2).GetAwaiter().GetResult();
                        SetBookingAsPrintedAsync(ticket).GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        _generalWriter.WriteEntry($"OrderListener::OnAcepted(). No se envió la reserva [{ticket}], a la cola de impresión.{Environment.NewLine} Detalles: {e}", EventLogEntryType.Error);
                    }
                    finally
                    {
                        Thread.Sleep(_delayTask);
                    }
                }
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"BookingListener::OnAcepted(). Ha ocurrido un error al capturar nuevas reservas. {Environment.NewLine} Detalles: {e.ToString()}", EventLogEntryType.Error);
            }
        }

        private void OnCancelled(List<Tuple<string, PrintMessage>> tickets) 
        {
            try
            {
                var ticketsToPrint = GetBookingsToPrint(tickets, false, true);
                foreach (var ticket in ticketsToPrint)
                {
                    try
                    {
                        _publisherService.PublishAsync(ticket.Item2).GetAwaiter().GetResult();
                        SetBookingAsPrintedAsync(ticket, false, true).GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        _generalWriter.WriteEntry($"OrderListener::OnCancelled(). No se envió la reserva [{ticket}], a la cola de impresión.{Environment.NewLine} Detalles: {e}", EventLogEntryType.Error);
                    }
                    finally
                    {
                        Thread.Sleep(_delayTask);
                    }
                }
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"BookingListener::OnCancelled(). Ha ocurrido un error al cancelar reservas. {Environment.NewLine} Detalles: {e.ToString()}", EventLogEntryType.Error);
            }
        }

        private void OnRecieve(QuerySnapshot snapshot)
        {
            _today = DateTime.UtcNow.ToString("dd/MM/yyyy");
            if (snapshot.Documents.Count == 0)
            {
                return;
            }
            var tickets = snapshot.Documents
                                        .Where(filter => filter.Exists)
                                        .Where(filter => filter.CreateTime.GetValueOrDefault().ToDateTime().ToString("dd/MM/yyyy") == _today)
                                        .OrderByDescending(o => o.CreateTime)
                                        .Select(s => s.GetMessagePrintType())
                                        .ToList();
            var newBookings = tickets.FindAll(f => f.Item2.PrintEvent == PrintEvents.NEW_BOOKING);
            var cancelledBookings = tickets.FindAll(f => f.Item2.PrintEvent == PrintEvents.CANCELED_BOOKING);
            if (newBookings.Count() > 0)
            {
                OnAcepted(newBookings);
            }
            if (cancelledBookings.Count() > 0)
            {
                OnCancelled(cancelledBookings);
            }
        }
        #endregion

        #region private methods
        private List<Tuple<string, PrintMessage>> GetBookingsToPrint(List<Tuple<string, PrintMessage>> documents, bool isCreated, bool isCancelled)
        {
            List<Tuple<string, PrintMessage>> ticketsToPrint = null;
            using (var sqlServerContext = new PrinterContext())
            {
                ticketsToPrint = sqlServerContext.GetItemsToPrint(documents, isCreated, isCancelled);
            }
            return ticketsToPrint;
        }

        private async Task SetBookingAsPrintedAsync(Tuple<string, PrintMessage> message, bool isNew = true, bool isCancelled = false)
        {
            using (var sqlServerContext = new PrinterContext())
            {
                await sqlServerContext.SetPrintedAsync(message, isNew, isCancelled);
            }
        }
        #endregion
    }
}
