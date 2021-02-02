using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Database.SqlServer;
using Menoo.PrinterService.Infraestructure.Interfaces;
using System;
using System.Diagnostics;

namespace Menoo.Printer.Listener.Bookings
{
    [Handler]
    public class BookingListener : IFirebaseListener
    {
        private readonly FirestoreDb _firestoreDb;

        private readonly SqlServerContext _sqlServerContext;

        private readonly EventLog _generalWriter;

        public BookingListener(
            FirestoreDb firestoreDb,
            SqlServerContext sqlServerContext,
            EventLog generalWriter)
        {
            _firestoreDb = firestoreDb;
            _sqlServerContext = sqlServerContext;
            _generalWriter = generalWriter;
        }

        public void Listen()
        {
        }

        public override string ToString()
        {
            return "Booking.Listener";
        }
    }
}
