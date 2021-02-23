﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Interfaces
{
    public interface IFirebaseRepository<T> where T : class
    {
        Task AddPropertyAsync(string id, string fieldName, string value, string collection);

        Task DeleteAsync(string id, string collection);

        Task<bool> ExistsByIdAsync(string id, string collection);

        Task<List<TEntity>> GetAllAsync<TEntity>(string collection);

        Task<TEntity> GetById<TEntity>(string id, string collection);

        Task SaveAsync<TEntity>(TEntity item, string collection);
    }
}
