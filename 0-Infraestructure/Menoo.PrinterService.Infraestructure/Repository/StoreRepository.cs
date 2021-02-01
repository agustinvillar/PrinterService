using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Database.Firebase;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;

namespace Menoo.PrinterService.Infraestructure.Repository
{
    public class StoreRepository : FirebaseRepository<Store>
    {
        public StoreRepository(FirestoreDb db) 
            : base (db)
        {
        }


    }
}
