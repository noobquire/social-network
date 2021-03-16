using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SocialNetworkApi.Data.Models;

namespace SocialNetworkApi.Data.Interfaces
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        public Task<TEntity> GetByIdAsync(Guid id);
        public Task<IEnumerable<TEntity>> GetAllAsync();
        public Task<IEnumerable<TEntity>> QueryAsync(Func<TEntity, bool> predicate);
        public Task<bool> DeleteByIdAsync(Guid id);
        public Task<bool> UpdateAsync(TEntity entity);
        public Task<bool> CreateAsync(TEntity entity);
    }
}
