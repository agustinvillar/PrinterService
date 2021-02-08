using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Database.Firebase;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using System.Threading.Tasks;

namespace Menoo.Printer.Builder.Orders.Repository
{
    public sealed class ItemRepository : FirebaseRepository<SectorItem>
    {
        private readonly FirestoreDb _firebaseDb;

        public ItemRepository(FirestoreDb firebaseDb)
            : base(firebaseDb)
        {
            _firebaseDb = firebaseDb;
        }

        public async Task<SectorItem> GetSectorItemById(string documentId) 
        {
            SectorItem item = null;
            var resultQuery = await GetById<ItemOrder>(documentId, "items");
            if (resultQuery != null && resultQuery.Sectors != null && resultQuery.Sectors.Count > 0)
            {
                item = new SectorItem
                {
                    Sectors = resultQuery.Sectors,
                    ItemId = documentId
                };
            }
            return item;
        }
    }
}
