﻿using GLSPM.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
namespace GLSPM.Application.EFCore.Repositories
{
    public class BaseRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class
    {
        private readonly GLSPMDBContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;
        public BaseRepository(GLSPMDBContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }
        public async Task DeleteAsync(TEntity entity)
        {
             _dbSet.Remove(entity);
        }

        public async Task DeleteAsync(TKey key)
        {
            var entity= await GetAsync(key);
            await DeleteAsync(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? condition = null, string sorting = null, int skipCound = 0, int maxResult = 100)
        {
            var data = condition != null ? 
                await _dbSet.Where(condition) // applying conditions
                .OrderBy(sorting)
                .Skip(skipCound)
                .Take(maxResult)
                .ToArrayAsync() :
                await _dbSet //without conditions
                .OrderBy(sorting)
                .Skip(skipCound)
                .Take(maxResult)
                .ToArrayAsync();

            return data;
        }

        public async Task<IQueryable<TEntity>> GetAsQueryableAsync()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<TEntity> GetAsync(TKey key)
        {
            return await _dbSet.FindAsync(keyValues: key);
        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            var entry= await _dbSet.AddAsync(entity);
            return entry.Entity;
        }
        public virtual async Task UpdateAsync(TEntity entity)
        {
           _dbSet.Update(entity);
        }
    }
}
