using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.Firebase;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Repository
{
    public sealed class PaymentRepository : FirebaseRepository<Payment>
    {
        private const int TIME_OUT_QUERY = 1;

        private readonly FirestoreDb _firebaseDb;

        public PaymentRepository(FirestoreDb firebaseDb) 
            : base (firebaseDb)
        {
            _firebaseDb = firebaseDb;
        }

        public async Task<Payment> GetPaymentByIdAsync(long id)
        {
            Thread.Sleep(TIME_OUT_QUERY);
            var snapshots = await _firebaseDb.Collection("payments").WhereEqualTo("paymentId", id).GetSnapshotAsync();
            var document = snapshots.Documents.LastOrDefault();
            if (document.Exists) 
            {
                var data = document.ToDictionary();
                return data.GetObject<Payment>();
            }
            return null;
        }
    }
}
