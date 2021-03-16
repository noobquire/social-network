using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialNetworkApi.Data.Interfaces;
using SocialNetworkApi.Data.Models;

namespace SocialNetworkApi.Data.Repositories
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        private readonly SocialNetworkDbContext _context;

        protected Repository(SocialNetworkDbContext context)
        {
            _context = context;
        }

        public async Task<TEntity> GetByIdAsync(string id)
        {
            return await _context
                .Set<TEntity>()
                .FirstOrDefaultAsync(e => e.Id.ToString() == id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context
                .Set<TEntity>()
                .ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> QueryAsync(Func<TEntity, bool> predicate)
        {
            return (await _context
                    .Set<TEntity>()
                    .ToListAsync())
                .Where(predicate);
        }

        public async Task<bool> DeleteByIdAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
            {
                return false;
            }
            try
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(TEntity entity)
        {
            var dbEntity = await GetByIdAsync(entity.Id.ToString());
            if (dbEntity == null)
            {
                return false;
            }
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CreateAsync(TEntity entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}