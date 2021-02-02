using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Database.SqlServer;
using Menoo.PrinterService.Infraestructure.Interfaces;
using System.Diagnostics;

namespace Menoo.Printer.Listener.Orders
{
    [Handler]
    public class OrderListener : IFirebaseListener
    {
        private readonly FirestoreDb _firestoreDb;

        private readonly SqlServerContext _sqlServerContext;

        private readonly EventLog _generalWriter;

        public OrderListener(
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
            //Nuevo TA creado.
            _firestoreDb.Collection("orders")
               .WhereEqualTo("status", "pendiente")
               .Listen(OnTakeAwayCreated);

            //Nueva orden de reserva o mesa.
            _firestoreDb.Collection("orders")
               .WhereEqualTo("status", "preparando")
               .Listen(OnOrderCreated);

            //Orden cancelada.
            _firestoreDb.Collection("orders")
                .WhereEqualTo("status", "cancelado")
                .Listen(OnCancelled);
        }

        public override string ToString()
        {
            return "Order.Listener";
        }

        #region private methods
        private void OnCancelled(QuerySnapshot snapshot)
        {

        }

        private void OnOrderCreated(QuerySnapshot snapshot)
        {

        }

        private void OnTakeAwayCreated(QuerySnapshot snapshot)
        {
            
        }
        #endregion
    }
}
