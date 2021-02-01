using System.Collections.Generic;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Interfaces
{
    public interface IFirebaseRepository<T> where T : class
    {
        Task AddPropertyAsync(string collection, string id, string fieldName, string value);

        Task DeleteAsync(string collection, string id);

        Task<bool> ExistsByIdAsync(string collection, string id);

        Task<List<TEntity>> GetAllAsync<TEntity>(string collection);

        Task<TEntity> GetById<TEntity>(string collection, string id);

        Task SaveAsync<TEntity>(string collection, TEntity item);
    }
}
