using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialNetworkApi.Data.Interfaces;
using SocialNetworkApi.Data.Models;

namespace SocialNetworkApi.Data.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private readonly SocialNetworkDbContext _context;

        public UserRepository(SocialNetworkDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(string id)
        {
            return await _context
                .Set<User>()
                .FirstOrDefaultAsync(e => e.Id.ToString() == id);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context
                .Set<User>()
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> QueryAsync(Func<User, bool> predicate)
        {
            return (await _context
                    .Set<User>()
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

        public async Task<bool> UpdateAsync(User entity)
        {
            var dbEntity = await GetByIdAsync(entity.Id);
            if (dbEntity == null)
            {
                return false;
            }
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CreateAsync(User entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
