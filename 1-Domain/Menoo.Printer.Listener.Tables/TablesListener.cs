using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Interfaces;

namespace Menoo.Printer.Listener.Tables
{
    public class TablesListener : IFirebaseListener
    {
        private readonly FirestoreDb _firestoreDb;

        public TablesListener(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        public void Listen()
        {
            throw new System.NotImplementedException();
        }
    }
}
