using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema;
using Menoo.PrinterService.Infraestructure.Extensions;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
using Menoo.PrinterService.Infraestructure.Repository;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Menoo.Printer.Builder.BookingBuilder
{
    [Handler]
    public class BookingBuilder : ITicketBuilder
    {
        private readonly PrinterContext _printerContext;

        private readonly FirestoreDb _firestoreDb;

        private readonly EventLog _generalWriter;

        private readonly UserRepository _userRepository;

        private readonly BookingRepository _bookingRepository;

        private readonly StoreRepository _storeRepository;

        private readonly TicketRepository _ticketRepository;

        public BookingBuilder(
            PrinterContext printerContext,
            FirestoreDb firestoreDb,
            StoreRepository storeRepository,
            TicketRepository ticketRepository,
            BookingRepository bookingRepository,
            UserRepository userRepository)
        {
            _printerContext = printerContext;
            _firestoreDb = firestoreDb;
            _storeRepository = storeRepository;
            _ticketRepository = ticketRepository;
            _bookingRepository = bookingRepository;
            _userRepository = userRepository;
            _generalWriter = GlobalConfig.DependencyResolver.ResolveByName<EventLog>("builder");
        }

        public async Task BuildAsync(string id, PrintMessage data)
        {
            if (data.Builder != PrintBuilder.BOOKING_BUILDER)
            {
                return;
            }
            var bookingDTO = await _bookingRepository.GetById<Booking>(data.DocumentId);
            var userDTO = await _userRepository.GetById<User>(bookingDTO.UserId);
            await SaveTicketBooking(id, bookingDTO, userDTO, data.PrintEvent);
        }

        private async Task SaveTicketBooking(string id, Booking booking, User user, string printEvent)
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            var store = await _storeRepository.GetById<Store>(booking.Store.Id, "stores");
            var sectors = store.GetPrintSettings(PrintEvents.NEW_BOOKING);
            if (sectors.Count > 0)
            {
                foreach (var sector in sectors.Where(f => f.AllowPrinting).OrderBy(o => o.Name))
                {
                    string title = string.Empty;
                    if (printEvent == PrintEvents.NEW_BOOKING)
                    {
                        title = "Nueva reserva";
                    }
                    else if (printEvent == PrintEvents.CANCELED_BOOKING)
                    {
                        title = "Reserva cancelada";
                    }

                    var ticket = new Ticket
                    {
                        TicketType = printEvent,
                        PrintBefore = Utils.BeforeAt(now, 10),
                        StoreId = booking.Store.Id,
                        Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                        Copies = sector.Copies,
                        PrinterName = sector.Printer,
                        StoreName = store.Name
                    };

                    ticket.SetBookingData(title, booking.BookingNumber, booking.BookingDate, booking.GuestQuantity, user.Name);
                    _generalWriter.WriteEntry($"BookingBuilder::SaveTicketBooking(). Enviando a imprimir la reserva con la siguiente información.{Environment.NewLine}Detalles:{Environment.NewLine}" +
                                $"Nombre de la impresora: {sector.Printer}{Environment.NewLine}" +
                                $"Sector de impresión: {sector.Name}{Environment.NewLine}" +
                                $"Hora de impresión: {ticket.PrintBefore}{Environment.NewLine}" +
                                $"Restaurante: {ticket.StoreName}{Environment.NewLine}" +
                                $"Número de reserva: {booking.BookingNumber}{Environment.NewLine}" +
                                $"Estado de la reserva: {booking.BookingState.ToUpper()}");
                    await _ticketRepository.SaveAsync(ticket);
                    await _printerContext.UpdateAsync(id, ticket.Data);
                }
            }
        }

        public override string ToString()
        {
            return PrintBuilder.BOOKING_BUILDER;
        }
    }
}
