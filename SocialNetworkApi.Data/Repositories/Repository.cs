﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialNetworkApi.Data.Interfaces;
using SocialNetworkApi.Data.Models;

namespace SocialNetworkApi.Data.Repositories
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        protected readonly SocialNetworkDbContext Context;

        protected Repository(SocialNetworkDbContext context)
        {
            Context = context;
        }

        public async Task<TEntity> GetByIdAsync(string id)
        {
            return (await GetAllAsync())
                .FirstOrDefault(e => e.Id.ToString() == id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await Context
                .Set<TEntity>()
                .ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetPaginatedAsync(PaginationFilter filter)
        {
           return await Context.Set<TEntity>()
               .Skip((filter.PageNumber - 1) * filter.PageSize)
               .Take(filter.PageSize)
               .ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> QueryAsync(Func<TEntity, bool> predicate)
        {
            return (await GetAllAsync())
                .Where(predicate);
        }

        public virtual async Task<bool> DeleteByIdAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
            {
                return false;
            }

            Context.Remove(entity);
            await Context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(TEntity entity)
        {
            var dbEntity = await GetByIdAsync(entity.Id.ToString());
            if (dbEntity == null)
            {
                return false;
            }
            Context.Update(entity);
            await Context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CreateAsync(TEntity entity)
        {
            await Context.AddAsync(entity);
            await Context.SaveChangesAsync();
            return true;
        }

        public async Task<int> CountAsync()
        {
            return await Context.Set<TEntity>().CountAsync();
        }
    }
}