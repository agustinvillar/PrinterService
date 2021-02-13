using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Database.Firebase;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;

namespace Menoo.Printer.Builder.Tables.Repository
{
    public class TableOpeningFamilyRepository : FirebaseRepository<TableOpeningFamily>
    {
        public TableOpeningFamilyRepository(FirestoreDb firebaseDb)
                : base(firebaseDb)
        {
        }
    }
}
