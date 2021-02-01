using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Interfaces;

namespace Menoo.Printer.Listener.Orders
{
    public class OrderListener : IFirebaseListener
    {
        private readonly FirestoreDb _firestoreDb;

        public OrderListener(FirestoreDb firestoreDb) 
        {
            _firestoreDb = firestoreDb;
        }

        public void Listen()
        {
            throw new System.NotImplementedException();
        }
    }
}
