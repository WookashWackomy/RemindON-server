using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RemindONServer.Domain.Repositories
{
    public interface IRepository<T> 
    {
        Task<IEnumerable<T>> ListAsync();
        Task AddAsync(T entity);
        Task<T> FindByIdAsync(int id);
        void Update(T entity);
        void Remove(T entity);
    }
}
