using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Database.Firebase
{
    public class FirebaseRepository<T> : IFirebaseRepository<T> where T : class
    {
        private readonly FirestoreDb _db;

        private const int DELAY_QUERY = 5000;

        public FirebaseRepository(FirestoreDb db)
        {
            _db = db;
        }

        public async Task AddPropertyAsync(string id, string fieldName, object value, string collection)
        {
            await _db.Collection(collection).Document(id).UpdateAsync(fieldName, value);
        }

        public virtual async Task DeleteAsync(string id, string collection)
        {
            await _db.Collection(collection).Document(id).DeleteAsync();
        }

        public virtual async Task<bool> ExistsByIdAsync(string id, string collection)
        {
            Thread.Sleep(DELAY_QUERY);
            var result = await _db.Collection(collection).Document(id).GetSnapshotAsync();
            return result != null && result.Exists;
        }

        public virtual async Task<List<TEntity>> GetAllAsync<TEntity>(string collection)
        {
            Thread.Sleep(DELAY_QUERY);
            var entities = new List<TEntity>();
            var snapshot = await _db.Collection(collection).GetSnapshotAsync();
            foreach (var item in snapshot.Documents)
            {
                var data = item.ToDictionary();
                var entityObject = data.GetObject<TEntity>();
                entities.Add(entityObject);
            }
            return entities;
        }

        public virtual async Task<TEntity> GetById<TEntity>(string id, string collection)
        {
            Thread.Sleep(DELAY_QUERY);
            var result = await _db.Collection(collection).Document(id).GetSnapshotAsync();
            if (result.Exists)
            {
                var data = result.ToDictionary();
                return data.GetObject<TEntity>();
            }
            return default;
        }

        public virtual async Task<DocumentReference> SaveAsync<TEntity>(TEntity item, string collection)
        {
            return await _db.Collection(collection).AddAsync(item);
        }

        public virtual async Task SaveAsync<TEntity>(string id, TEntity item, string collection)
        {
            await _db.Collection(collection).Document(id).CreateAsync(item);
        }
    }
}
