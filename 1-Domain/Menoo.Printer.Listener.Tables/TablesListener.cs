using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Database.SqlServer;
using Menoo.PrinterService.Infraestructure.Interfaces;
using System.Diagnostics;

namespace Menoo.Printer.Listener.Tables
{
    [Handler]
    public class TablesListener : IFirebaseListener
    {
        private readonly FirestoreDb _firestoreDb;

        private readonly SqlServerContext _sqlServerContext;

        private readonly EventLog _generalWriter;

        public TablesListener(
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
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "Tables.Listener";
        }
    }
}
