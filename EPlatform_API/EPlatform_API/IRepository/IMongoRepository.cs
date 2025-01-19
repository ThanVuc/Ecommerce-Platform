using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.IRepository
{
    public interface IMongoRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task CreateAsync(T document);
        Task Update(string id, T document);
        Task DeleteAsync(string id);
        Task<T> GetByIdAsync(string id);

    }
}