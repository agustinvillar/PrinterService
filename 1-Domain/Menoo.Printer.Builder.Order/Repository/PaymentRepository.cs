using Google.Cloud.Firestore;
using Menoo.Printer.Builder.Orders.Constants;
using Menoo.PrinterService.Infraestructure.Database.Firebase;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Menoo.Printer.Builder.Orders.Repository
{
    public sealed class PaymentRepository : FirebaseRepository<Payment>
    {
        private readonly FirestoreDb _firebaseDb;

        public PaymentRepository(FirestoreDb firebaseDb) 
            : base (firebaseDb)
        {
            _firebaseDb = firebaseDb;
        }

        public async Task<Payment> GetPayment(string id, string type)
        {
            string filter = type == OrderTypes.MESA ? "tableOpening.id" : "taOpening.id";
            var documentSnapshots = await _firebaseDb.Collection("payments").WhereEqualTo(filter, id).GetSnapshotAsync();
            var payments = documentSnapshots.Documents.Select(d => d.ConvertTo<Payment>()).ToList();
            return payments.FirstOrDefault();
        }
    }
}
