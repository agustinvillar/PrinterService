using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.SqlServer;
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
            //Aceptada
            _firestoreDb.Collection("bookings")
                       .WhereEqualTo("bookingState", "aceptada")
                       .OrderByDescending("createdAt")
                       .Listen(OnAcepted);
            //Cancelada
            _firestoreDb.Collection("bookings")
                       .WhereEqualTo("bookingState", "cancelada")
                       .OrderByDescending("updatedAt")
                       .Listen(OnCancelled);
        }

        public override string ToString()
        {
            return "Booking.Listener";
        }

        #region listener
        private void OnAcepted(QuerySnapshot snapshot)
        {
            _today = DateTime.UtcNow.ToString("dd/MM/yyyy");
            try
            {
                if (snapshot.Documents.Count == 0)
                {
                    return;
                }
                var ticketsOrders = snapshot.Documents
                                            .Where(filter => filter.Exists)
                                            .Where(filter => filter.UpdateTime.GetValueOrDefault().ToDateTime().ToString("dd/MM/yyyy") == _today)
                                            .OrderByDescending(o => o.CreateTime)
                                            .Select(s => s.Id)
                                            .ToList();
                var ticketsToPrint = GetBookingsToPrint(ticketsOrders, true, false);
                if (ticketsToPrint == null || ticketsToPrint.Count == 0)
                {
                    return;
                }
                foreach (var ticket in ticketsToPrint)
                {
                    var messageQueue = new PrintMessage
                    {
                        DocumentId = ticket,
                        PrintEvent = PrintEvents.NEW_BOOKING,
                        TypeDocument = PrintTypes.BOOKING
                    };
                    try
                    {
                        _publisherService.PublishAsync(messageQueue).GetAwaiter().GetResult();
                        SetBookingAsPrintedAsync(messageQueue).GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        _generalWriter.WriteEntry($"BookingListener::OnAcepted(). No se envió la reserva [{ticket}], a la cola de impresión.{Environment.NewLine} Detalles: {e}", EventLogEntryType.Error);
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

        private void OnCancelled(QuerySnapshot snapshot) 
        {
            _today = DateTime.UtcNow.ToString("dd/MM/yyyy");
            try
            {
                if (snapshot.Documents.Count == 0)
                {
                    return;
                }
                var ticketsOrders = snapshot.Documents
                                            .Where(filter => filter.Exists)
                                            .Where(filter => filter.UpdateTime.GetValueOrDefault().ToDateTime().ToString("dd/MM/yyyy") == _today)
                                            .OrderByDescending(o => o.UpdateTime)
                                            .Select(s => s.Id)
                                            .ToList();
                var ticketsToPrint = GetBookingsToPrint(ticketsOrders, false, true);
                if (ticketsToPrint == null || ticketsToPrint.Count == 0)
                {
                    return;
                }
                foreach (var ticket in ticketsToPrint)
                {
                    var messageQueue = new PrintMessage
                    {
                        DocumentId = ticket,
                        PrintEvent = PrintEvents.CANCELED_BOOKING,
                        TypeDocument = PrintTypes.BOOKING
                    };
                    try
                    {
                        _publisherService.PublishAsync(messageQueue).GetAwaiter().GetResult();
                        SetBookingAsPrintedAsync(messageQueue, false, true).GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        _generalWriter.WriteEntry($"BookingListener::OnCancelled(). No se envió la reserva [{ticket}], a la cola de impresión.{Environment.NewLine} Detalles: {e}", EventLogEntryType.Error);
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
        #endregion

        #region private methods
        private List<string> GetBookingsToPrint(List<string> documentIds, bool isCreated, bool isCancelled)
        {
            List<string> ticketsToPrint = null;
            using (var sqlServerContext = new SqlServerContext())
            {
                ticketsToPrint = sqlServerContext.GetItemsToPrint(documentIds, isCreated, isCancelled);
            }
            return ticketsToPrint;
        }

        private async Task SetBookingAsPrintedAsync(PrintMessage message, bool isNew = true, bool isCancelled = false)
        {
            using (var sqlServerContext = new SqlServerContext())
            {
                await sqlServerContext.SetPrintedAsync(message, isNew, isCancelled);
            }
        }
        #endregion
    }
}
