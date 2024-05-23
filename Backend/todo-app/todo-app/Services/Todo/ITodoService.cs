using todo_app.Contracts;
using todo_app.DTOs;
using todo_app.Filter;
using todo_app.Model.Request;

namespace todo_app.Services.TodoService
{
    public interface ITodoService
    {
        Task AddAsync(TodoRequest todoRequest,CancellationToken cancellationToken=default);
        Task<TodoDTO> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default);
        Task<int> CountAsync(TodoFilter todoFilter, CancellationToken cancellationToken = default);

        Task Delete(Guid Id, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid Id,TodoRequest todoRequest,CancellationToken cancellationToken=default);
        Task<IEnumerable<TodoDTO>> GetAllAsync(TodoFilter todoFilter,IPagingParams pagingParams,CancellationToken cancellationToken = default);
    }
}
