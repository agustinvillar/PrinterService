using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Database.Firebase;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Repository
{
    public class BookingRepository : FirebaseRepository<Booking>
    {
        public BookingRepository(FirestoreDb firebaseDb)
            : base(firebaseDb)
        {
        }

        public override Task<TEntity> GetById<TEntity>(string id, string collection = "bookings")
        {
            return base.GetById<TEntity>(id, collection);
        }
    }
}
