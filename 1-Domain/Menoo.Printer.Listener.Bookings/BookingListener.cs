using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Interfaces;
using System;

namespace Menoo.Printer.Listener.Bookings
{
    public class BookingListener : IFirebaseListener
    {
        private readonly FirestoreDb _firestoreDb;

        public BookingListener(FirestoreDb firestoreDb) 
        {
            _firestoreDb = firestoreDb;
        }

        public void Listen()
        {
            throw new NotImplementedException();
        }
    }
}
