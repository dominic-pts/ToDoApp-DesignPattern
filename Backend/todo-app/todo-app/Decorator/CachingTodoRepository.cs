using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;
using todo_app.Contracts;
using todo_app.DTOs.Queries;
using todo_app.Entities;
using todo_app.Repositories.TodoRepository;

namespace todo_app.Decorator
{
    public class CachingTodoRepository : ITodoRepository
{
    private readonly ITodoRepository _innerRepository;
    private readonly IMemoryCache _cache;
    private const string KeyAll = "all";
    public CachingTodoRepository(ITodoRepository innerRepository, IMemoryCache cache)
    {
        _innerRepository = innerRepository;
        _cache = cache;
    }
    public async Task<int> CountAsync(TodoQuery todoQuery, CancellationToken cancellationToken = default)
    {
        var count = await _innerRepository.CountAsync(todoQuery, cancellationToken);
        return count;
    }

    public async Task<int> CountAsync(Expression<Func<Todo, bool>> cretial, CancellationToken cancellationToken = default)
    {
        var count = await _innerRepository.CountAsync(cretial, cancellationToken);
        return count;
    }
    public async Task<IEnumerable<Todo>> GetAllAsync(TodoQuery todoQuery, IPagingParams pagingParams, CancellationToken cancellationToken = default)
    {
        string cacheKey = KeyAll;
        if (!_cache.TryGetValue(cacheKey, out IEnumerable<Todo> todos))
        {
            todos = await _innerRepository.GetAllAsync(todoQuery, pagingParams, cancellationToken);
            _cache.Set(cacheKey, todos, TimeSpan.FromMinutes(5)); // Cache for 5 minutes
        }
        return todos;
    }
    public async Task<IEnumerable<Todo>> GetAllAsync(Expression<Func<Todo, bool>> cretial, CancellationToken cancellationToken = default)
    {
        var todos = await _innerRepository.GetAllAsync(cretial, cancellationToken);
        return todos;
    }
    public async Task<Todo> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        string cacheKey = $"GetById_{id}";
        if (!_cache.TryGetValue(cacheKey, out Todo todo))
        {
            todo = await _innerRepository.GetByIdAsync(id, cancellationToken);
            _cache.Set(cacheKey, todo, TimeSpan.FromMinutes(5)); // Cache for 5 minutes
        }
        return todo;
    }
    public void Add(Todo todo)
    {
        _innerRepository.Add(todo);
        _cache.Remove(KeyAll);
    }
    public void Delete(Todo todo)
    {
        _innerRepository.Delete(todo);
        _cache.Remove(KeyAll);
        _cache.Remove($"GetById_{todo.Id}");
    }
    public void Update(Todo todo)
    {
        _innerRepository.Update(todo);
        _cache.Remove(KeyAll);
        _cache.Remove($"GetById_{todo.Id}");
    }
}
}
