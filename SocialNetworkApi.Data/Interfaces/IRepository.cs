using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SocialNetworkApi.Data.Models;

namespace SocialNetworkApi.Data.Interfaces
{
    public interface IRepository<TEntity>
    {
        Task<TEntity> GetByIdAsync(string id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> GetPaginatedAsync(PaginationFilter filter);
        Task<IEnumerable<TEntity>> QueryAsync(Func<TEntity, bool> predicate);
        Task<bool> DeleteByIdAsync(string id);
        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> CreateAsync(TEntity entity);
        Task<int> CountAsync();
    }
}
