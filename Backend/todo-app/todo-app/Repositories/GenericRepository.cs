using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using todo_app.Common;
using todo_app.Data;
using todo_app.Entities;

namespace todo_app.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected TodoDbContext _context;

        public GenericRepository(TodoDbContext todoDbContext)
        {
            _context = todoDbContext;
        }
        public virtual void Add(T todo)
        {
            _context.Add(todo);
        }

        public Task<int> CountAsync(Expression<Func<T, bool>> cretial, CancellationToken cancellationToken = default)
        {
            return _context.Set<T>().Where(cretial).CountAsync(cancellationToken);
        }

        public virtual void Delete(T todo)
        {
            _context.Remove(todo);
        }
        public async virtual Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>()
                                 .AsNoTracking()
                                 .Where(criteria)
                                 .ToListAsync(cancellationToken);
        }
        public async virtual Task<T> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
        }
        public virtual void Update(T todo)
        {
            _context.Update(todo);
        }
    }
}
