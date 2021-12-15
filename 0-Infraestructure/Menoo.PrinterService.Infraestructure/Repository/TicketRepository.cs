using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Database.Firebase;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Queues;
using System;
using System.Collections.Generic;
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

        public override async Task SaveAsync<TEntity>(TEntity item, string collection = "print")
        {
            await base.SaveAsync(item, collection);
        }

        public async Task<bool> IsTicketPrinted(Tuple<string, PrintMessage> message)
        {
            QuerySnapshot documentSnapshots;
            if (message.Item2.DocumentsId != null && message.Item2.DocumentsId.Count > 0)
            {
                documentSnapshots = await _firebaseDb.Collection(TICKET_HISTORY)
                    .WhereEqualTo("dayCreatedAt", DateTime.Now.ToString("dd/MM/yyyy"))
                    .WhereEqualTo("printEvent", message.Item2.PrintEvent)
                    .WhereArrayContainsAny("entityId", message.Item2.DocumentsId).GetSnapshotAsync();
            }
            else 
            {
                var filter = new List<string>() { message.Item2.DocumentId };
                documentSnapshots = await _firebaseDb.Collection(TICKET_HISTORY)
                    .WhereEqualTo("dayCreatedAt", DateTime.Now.ToString("dd/MM/yyyy"))
                    .WhereEqualTo("printEvent", message.Item2.PrintEvent)
                    .WhereArrayContainsAny("entityId", filter).GetSnapshotAsync();
            }
            bool exists = documentSnapshots.Documents.Count > 0;
            return exists;
        }

        public async Task SetPrintedAsync(Tuple<string, PrintMessage> message) 
        {
            bool isExists = await IsTicketPrinted(message);
            if (isExists) 
            {
                return;
            }
            var entity = new TicketHistory
            {
                Id = message.Item1,
                DayCreatedAt = DateTime.Now.ToString("dd/MM/yyyy"),
                PrintEvent = message.Item2.PrintEvent,
                CreatedAt = DateTime.UtcNow
            };
            entity.EntityId = message.Item2.DocumentsId != null && message.Item2.DocumentsId.Count > 0 ? message.Item2.DocumentsId : new System.Collections.Generic.List<string>() { message.Item2.DocumentId };
            await base.SaveAsync(message.Item1, entity, TICKET_HISTORY);
        }

        public async Task SetTicketImageAsync(string id, string ticketImage) 
        {
            await AddPropertyAsync(id, "ticketImage", ticketImage, TICKET_HISTORY);
        }
    }
}
