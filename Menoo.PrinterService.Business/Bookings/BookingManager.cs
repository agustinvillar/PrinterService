using Google.Cloud.Firestore;
using Menoo.PrinterService.Business.Core;
using Menoo.PrinterService.Business.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Menoo.PrinterService.Business.Entities.Ticket;

namespace Menoo.PrinterService.Business.Bookings
{
    /// <summary>
    /// Maneja los eventos de la colección booking
    /// </summary>
    public class BookingManager
    {
        private readonly FirestoreDb _db;

        public BookingManager(FirestoreDb db) 
        {
            _db = db;
        }

        /// <summary>
        /// Escucha los eventos disparados en la colección bookings.
        /// </summary>
        public void Listen() 
        {
            _db.Collection("bookings")
                       .OrderByDescending("updatedAt")
                       .Limit(1)
                       .Listen(OnAcepted);
            _db.Collection("bookings")
                       .WhereEqualTo("bookingState", "cancelada")
                       .Listen(OnCancelled);
        }

        #region events
        /// <summary>
        /// Evento para una reserva aceptada.
        /// </summary>
        private void OnAcepted(QuerySnapshot snapshot) 
        {
            try
            {
                var document = snapshot.Single();
                var booking = document.ConvertTo<Booking>();
                User user = null;
                var d = document.ToDictionary();
                var snapshotUser = _db.Collection("customers").Document(booking.UserId).GetSnapshotAsync().GetAwaiter().GetResult();

                if (snapshotUser.Exists) 
                {
                    user = snapshotUser.ConvertTo<User>();
                }
                if (!booking.PrintedAccepted)
                {
                    SetBookingPrintedAsync(document.Id, Booking.PRINT_TYPE.ACCEPTED).GetAwaiter().GetResult();
                    _ = SaveAcceptedBooking(booking, user);
                }
            }
            catch (Exception ex)
            {
                Utils.LogError(ex.Message);
            }
        }

        /// <summary>
        /// Evento para una reserva cancelada.
        /// </summary>
        private void OnCancelled(QuerySnapshot snapshot) 
        {
            try
            {
                User user = null;
                foreach (var document in snapshot.Documents.OrderByDescending(o => o.CreateTime))
                {
                    var booking = document.ConvertTo<Booking>();
                    var snapshotUser = _db.Collection("customers").Document(booking.UserId).GetSnapshotAsync().GetAwaiter().GetResult();
                    if (snapshotUser.Exists) 
                    {
                        user = snapshotUser.ConvertTo<User>();
                    }
                    if (!booking.PrintedCancelled)
                    {
                        SetBookingPrintedAsync(document.Id, Booking.PRINT_TYPE.CANCELLED).GetAwaiter().GetResult();
                        _ = SaveCancelledBooking(booking, user);
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.LogError(ex.Message);
            }
        }
        #endregion

        #region private methods
        private async Task SaveAcceptedBooking(Booking booking, User user)
        {
            var store = await Utils.GetStores(_db, booking.Store.StoreId);
            if (!store.AllowPrint(PrintEvents.NEW_BOOKING)) 
            {
                return;
            }
            if (booking.BookingState.Equals("aceptada", StringComparison.OrdinalIgnoreCase))
            {
                var ticket = new Ticket
                {
                    TicketType = TicketTypeEnum.NEW_BOOKING.ToString(),
                    PrintBefore = Utils.BeforeAt(booking.BookingDate, -10),
                    StoreId = booking.Store.StoreId,
                    Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                };
                ticket.SetBookingData("Nueva reserva", booking.BookingNumber, booking.BookingDate, booking.GuestQuantity, user.Name);
                await Utils.SaveTicketAsync(_db, ticket);
            }
        }

        private async Task SaveCancelledBooking(Booking booking, User user)
        {
            var store = await Utils.GetStores(_db, booking.Store.StoreId);
            if (!store.AllowPrint(PrintEvents.CANCELED_BOOKING))
            {
                return;
            }
            var ticket = Utils.CreateInstanceOfTicket();
            ticket.TicketType = TicketTypeEnum.CANCELLED_BOOKING.ToString();
            if (booking.BookingState.Equals("cancelada", StringComparison.OrdinalIgnoreCase))
            {
                ticket.SetBookingData("Reserva cancelada", booking.BookingNumber, booking.BookingDate, booking.GuestQuantity, user.Name);
            }
            ticket.PrintBefore = Utils.BeforeAt(booking.BookingDate, -10);
            ticket.StoreId = booking.Store.StoreId;
            ticket.Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            await Utils.SaveTicketAsync(_db, ticket);
        }

        private async Task<WriteResult> SetBookingPrintedAsync(string doc, Booking.PRINT_TYPE type)
        {
            var result = await _db.Collection("bookings")
                      .Document(doc)
                      .UpdateAsync(type == Booking.PRINT_TYPE.ACCEPTED ? "printedAccepted"
                                                                       : "printedCancelled", true);
            return result;
        }
        #endregion
    }
}
