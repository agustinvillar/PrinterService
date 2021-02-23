using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Database.Firebase;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Repository
{
    public sealed class UserRepository : FirebaseRepository<User>
    {
        public UserRepository(FirestoreDb firebaseDb)
            : base(firebaseDb)
        {
        }

        public override Task<TEntity> GetById<TEntity>(string id, string collection = "customers")
        {
            return base.GetById<TEntity>(id, collection);
        }
    }
}
