using Menoo.Backend.Integrations.Constants;
using Menoo.Backend.Integrations.Messages;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Repository;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Menoo.Printer.Builder.BookingBuilder
{
    [Handler]
    public class BookingBuilder : ITicketBuilder
    {
        private readonly UserRepository _userRepository;

        private readonly BookingRepository _bookingRepository;

        private readonly StoreRepository _storeRepository;

        public BookingBuilder(BookingRepository bookingRepository, UserRepository userRepository, StoreRepository storeRepository)
        {
            _bookingRepository = bookingRepository;
            _userRepository = userRepository;
            _storeRepository = storeRepository;
        }

        public async Task<PrintInfo> BuildAsync(PrintMessage data)
        {
            if (data.Builder != PrintBuilder.BOOKING_BUILDER)
            {
                return null;
            }
            var bookingDTO = await _bookingRepository.GetById<Booking>(data.DocumentId);
            var store = await _storeRepository.GetById<Store>(bookingDTO.Store.Id, "stores");
            var userDTO = await _userRepository.GetById<User>(bookingDTO.UserId);
            var dataToPrint = new PrintInfo
            {
                Store = store,
                BeforeAt = Utils.BeforeAt(DateTime.Now.ToString("yyyy-MM-dd HH:mm"), 10),
                Template = PrintTemplates.NEW_BOOKING
            };
            dataToPrint.Content = SaveTicketBooking(bookingDTO, userDTO, data.PrintEvent);
            return dataToPrint;
        }

        private Dictionary<string, object> SaveTicketBooking(Booking booking, User user, string printEvent)
        {
            string title = string.Empty;
            var data = new Dictionary<string, object>();
            var dateTime = Convert.ToDateTime(booking.BookingDate);
            if (printEvent == PrintEvents.NEW_BOOKING)
            {
                title = "Nueva reserva";
            }
            else if (printEvent == PrintEvents.CANCELED_BOOKING)
            {
                title = "Reserva cancelada";
            }
            data.Add("title", title);
            data.Add("bookingNumber", booking.BookingNumber.ToString());
            data.Add("quantity", booking.GuestQuantity.ToString());
            data.Add("bookingDate", dateTime.ToString("dd/MM/yyyy"));
            data.Add("bookingTime", dateTime.ToString("HH:mm:ss"));
            data.Add("clientName", user.Name);
            return data;
        }

        public override string ToString()
        {
            return PrintBuilder.BOOKING_BUILDER;
        }
    }
}
