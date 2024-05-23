using System.Linq.Expressions;
using todo_app.Contracts;
using todo_app.DTOs.Queries;
using todo_app.Entities;

namespace todo_app.Repositories.TodoRepository
{
    public interface ITodoRepository:IGenericRepository<Todo>
    {
        Task<IEnumerable<Todo>> GetAllAsync(TodoQuery todoQuery,IPagingParams pagingParams,CancellationToken cancellationToken=default);
        Task<int> CountAsync(TodoQuery todoQuery, CancellationToken cancellationToken = default);
    }
}
