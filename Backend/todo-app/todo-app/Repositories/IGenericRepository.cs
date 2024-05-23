using System.Linq.Expressions;
using todo_app.Common;
using todo_app.Entities;

namespace todo_app.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        void Add(T todo);
        void Delete(T todo);
        void Update(T todo);
        Task<T> GetByIdAsync(object id,CancellationToken cancellationToken=default);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> cretial,CancellationToken cancellationToken=default);
        Task<int> CountAsync(Expression<Func<T, bool>> cretial, CancellationToken cancellationToken = default);
    }
}
