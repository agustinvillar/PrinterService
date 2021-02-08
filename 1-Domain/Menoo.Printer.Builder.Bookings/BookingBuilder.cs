using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
using Menoo.PrinterService.Infraestructure.Repository;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Menoo.Printer.Builder.Orders
{
    [Handler]
    public class BookingBuilder : ITicketBuilder
    {
        private readonly FirestoreDb _firestoreDb;

        private readonly EventLog _generalWriter;

        private readonly int _queueDelay;

        private readonly BookingRepository _bookingRepository;

        private readonly StoreRepository _storeRepository;

        private readonly TicketRepository _ticketRepository;

        public BookingBuilder(
            FirestoreDb firestoreDb,
            StoreRepository storeRepository,
            TicketRepository ticketRepository,
            BookingRepository bookingRepository)
        {
            _firestoreDb = firestoreDb;
            _storeRepository = storeRepository;
            _ticketRepository = ticketRepository;
            _bookingRepository = bookingRepository;
            _queueDelay = int.Parse(GlobalConfig.ConfigurationManager.GetSetting("queueDelay"));
            _generalWriter = GlobalConfig.DependencyResolver.ResolveByName<EventLog>("builder");
        }

        public async Task BuildAsync(PrintMessage data)
        {
            if (data.Builder != PrintBuilder.BOOKING_BUILDER)
            {
                return;
            }
            var bookingDTO = await _bookingRepository.GetById<Booking>(data.DocumentId);
            if (data.PrintEvent == PrintEvents.NEW_BOOKING)
            {
                BuildBookingCreated(bookingDTO);
            }
            else if (data.PrintEvent == PrintEvents.CANCELED_BOOKING)
            {
                BuildBookingCancelled(bookingDTO);
            }
        }

        public override string ToString()
        {
            return PrintBuilder.BOOKING_BUILDER;
        }

        #region private methods
        private void BuildBookingCancelled(Booking bookingDTO)
        {
            throw new NotImplementedException();
        }

        private void BuildBookingCreated(Booking bookingDTO)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
