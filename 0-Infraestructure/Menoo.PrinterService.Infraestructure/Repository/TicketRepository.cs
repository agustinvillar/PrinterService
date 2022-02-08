using Google.Cloud.Firestore;
using Menoo.Backend.Integrations.Constants;
using Menoo.Backend.Integrations.Messages;
using Menoo.PrinterService.Infraestructure.Database.Firebase;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using System;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Repository
{
    public class TicketRepository : FirebaseRepository<Ticket>
    {
        private readonly FirestoreDb _firebaseDb;

        private const string TICKET_HISTORY = "ticketHistory";

        private const string TICKET_QUEUE = "print";

        public TicketRepository(FirestoreDb db)
            : base(db)
        {
            _firebaseDb = db;
        }

        public async Task<DocumentReference> SaveAsync(Ticket document) 
        {
            return await base.SaveAsync(document, TICKET_QUEUE);
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
            await base.SaveAsync(entity, TICKET_HISTORY);
        }
    }
}
