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

        public TicketRepository(FirestoreDb db)
            : base(db)
        {
            _firebaseDb = db;
        }

        public async Task SetPrintedAsync(PrintMessage message, string ticketImage) 
        {
            var entity = new TicketHistory
            {
                TicketImage = ticketImage,
                DayCreatedAt = DateTime.Now.ToString("dd/MM/yyyy"),
                PrintEvent = message.PrintEvent,
                CreatedAt = DateTime.UtcNow
            };

            if (message.PrintEvent == PrintEvents.REPRINT_ORDER)
            {
                entity.EntityId = new System.Collections.Generic.List<string>() { message.DocumentId };
            }
            else
            {
                entity.EntityId = message.DocumentsId != null && message.DocumentsId.Count > 0 ? message.DocumentsId : new System.Collections.Generic.List<string>() { message.DocumentId };
            }
            await base.SaveAsync(entity, TICKET_HISTORY);
        }
    }
}
