using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.AppServices.Interfaces
{
    public interface ITrasherAppService<TEntity,TKey>
    {
        Task MarkAsDeletedAsync(TKey key);
        Task<TEntity> UnMarkAsDeletedAsync(TKey key);
        Task<IEnumerable<TEntity>> GetDeletedAsync();
        Task<bool> IsDeleted(TKey key);
    }
}
