using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Database.Firebase;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Repository
{
    public class TicketRepository : FirebaseRepository<Ticket>
    {
        public TicketRepository(FirestoreDb db)
            : base(db)
        {
        }

        public override Task SaveAsync<TEntity>(TEntity item, string collection = "print")
        {
            return base.SaveAsync(item, collection);
        }
    }
}
