using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Database.Firebase
{
    public class FirebaseRepository<T> : IFirebaseRepository<T> where T : class
    {
        private readonly FirestoreDb _db;

        public FirebaseRepository(FirestoreDb db)
        {
            _db = db;
        }

        public async Task AddPropertyAsync(string collection, string id, string fieldName, string value)
        {
            await _db.Collection(collection).Document(id).UpdateAsync(fieldName, true);
        }

        public async Task DeleteAsync(string collection, string id)
        {
            await _db.Collection(collection).Document(id).DeleteAsync();
        }

        public async Task<bool> ExistsByIdAsync(string collection, string id)
        {
            var result = await _db.Collection(collection).Document(id).GetSnapshotAsync();
            return result != null && result.Exists;
        }

        public virtual async Task<List<TEntity>> GetAllAsync<TEntity>(string collection)
        {
            var stores = new List<TEntity>();
            var snapshot = await _db.Collection(collection).GetSnapshotAsync();
            foreach (var item in snapshot.Documents)
            {
                var storeData = item.ToDictionary();
                var storeObject = storeData.GetObject<TEntity>();
                stores.Add(storeObject);
            }
            return stores;
        }

        public virtual async Task<TEntity> GetById<TEntity>(string collection, string id)
        {
            var result = await _db.Collection(collection).Document(id).GetSnapshotAsync();
            if (result.Exists) 
            {
                return result.GetObject<TEntity>();
            }
            return default;
        }

        public async Task SaveAsync<TEntity>(string collection, TEntity item)
        {
            await _db.Collection(collection).AddAsync(item);
        }
    }
}
