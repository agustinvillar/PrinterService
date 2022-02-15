using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Database.Firebase;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Repository
{
    public class TicketRepository : FirebaseRepository<Ticket>
    {
        private readonly FirestoreDb _firebaseDb;

        private readonly string _ticketHistory = ConfigurationManager.AppSettings["printTicketHistoryCollection"].ToString();

        private readonly string _ticket = ConfigurationManager.AppSettings["printCollection"].ToString();

        public TicketRepository(FirestoreDb db)
            : base(db)
        {
            _firebaseDb = db;
        }

        public async Task<DocumentReference> SaveAsync(Ticket document)
        {
            return await base.SaveAsync(document, _ticket);
        }


        public async Task SetPrintedAsync(string printEvent, string printId)
        {
            var entity = new TicketHistory
            {
                DayCreatedAt = DateTime.Now.ToString("dd/MM/yyyy"),
                PrintEvent = printEvent,
                CreatedAt = DateTime.UtcNow,
                PrintId = printId
            };
            await base.SaveAsync(entity, _ticketHistory);
        }
    }
}
