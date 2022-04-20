using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Domain.Repositories
{
    public interface IRepository<TEntity, TKey>
    {
        Task<TEntity> GetAsync(TKey key);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? condition = null);
        Task<IQueryable<TEntity>> GetAsQueryableAsync();
        Task<TEntity> InsertAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task DeleteAsync(TKey key);
    }
}
