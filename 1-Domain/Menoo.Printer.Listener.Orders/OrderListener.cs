using Google.Cloud.Firestore;
using Menoo.Printer.Listener.Orders.Constants;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Database.SqlServer;
using Menoo.PrinterService.Infraestructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Menoo.Printer.Listener.Orders
{
    [Handler]
    public class OrderListener : IFirebaseListener
    {
        private readonly FirestoreDb _firestoreDb;

        private readonly SqlServerContext _sqlServerContext;

        private readonly IPublisherService _publisherService;

        private readonly EventLog _generalWriter;

        private string _today;

        public OrderListener(
            FirestoreDb firestoreDb,
            SqlServerContext sqlServerContext,
            IPublisherService publisherService,
            EventLog generalWriter) 
        {
            _firestoreDb = firestoreDb;
            _sqlServerContext = sqlServerContext;
            _publisherService = publisherService;
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
            _today = DateTime.Now.ToString("dd/MM/yyyy");
        }

        private void OnOrderCreated(QuerySnapshot snapshot)
        {
            _today = DateTime.Now.ToString("dd/MM/yyyy");
        }

        private void OnTakeAwayCreated(QuerySnapshot snapshot)
        {
            _today = DateTime.Now.ToString("dd/MM/yyyy");
            if (snapshot.Documents.Count == 0)
            {
                return;
            }
            var ticketsTakeAway = snapshot.Documents
                                        .Where(filter => filter.Exists)
                                        .Where(filter => filter.CreateTime.GetValueOrDefault().ToDateTime().ToString("dd/MM/yyyy") == _today);
            //var ticketsTakeAway = snapshot.Documents
            //                            .Where(filter => filter.Exists)
            //                            .Where(filter => filter.CreateTime.GetValueOrDefault().ToDateTime().ToString("dd/MM/yyyy") == _today)
            //                            .OrderByDescending(o => o.CreateTime)
            //                            .Select(s => s.Id)
            //                            .ToList();
            //var ticketsToPrint = GetNewTakeAwayToPrint(ticketsTakeAway);
        }

        private List<string> GetNewTakeAwayToPrint(List<string> documentIds) 
        {
            //var ticketsToPrint = new List<string>();
            var printedTicketIds = _sqlServerContext.TicketHistorySettings
                                        .Where(f => f.Name == OrderProperties.IS_NEW_PRINTED)
                                        .Where(f => bool.Parse(f.Value))
                                        .Select(s => s.TicketHistory.Id)
                                        .ToList();
            var ticketsToPrint = from c in documentIds
                                 where !printedTicketIds.Contains(c)
                                 select c;
            return ticketsToPrint.ToList();
        }
        #endregion
    }
}
