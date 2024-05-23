using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using todo_app.Contracts;
using todo_app.Data;
using todo_app.DTOs.Queries;
using todo_app.Entities;

namespace todo_app.Repositories.TodoRepository
{
    public class TodoRepository : GenericRepository<Todo>, ITodoRepository
    {
        public TodoRepository(TodoDbContext todoDbContext) : base(todoDbContext) { }

        public async Task<int> CountAsync(TodoQuery todoQuery, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Todo>().Where(GenerateQuery(todoQuery)).CountAsync();
        }
        public async Task<IEnumerable<Todo>> GetAllAsync(TodoQuery todoQuery, IPagingParams pagingParams, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Todo>()
                .Where(GenerateQuery(todoQuery))
                .Skip((pagingParams.PageNumber-1)*pagingParams.PageSize)
                .Take(pagingParams.PageSize)
                .ToListAsync();
        }
        private Expression<Func<Todo,bool>> GenerateQuery(TodoQuery todoQuery)
        {

           
            Expression<Func<Todo, bool>> query = t => true;

            if (!string.IsNullOrEmpty(todoQuery.Name))
            {
                query = query.And(t => t.Name.Contains(todoQuery.Name));
            }

            if (!string.IsNullOrEmpty(todoQuery.Description))
            {
                query = query.And(t => t.Description.Contains(todoQuery.Description));
            }

            if (todoQuery.Status != null)
            {
                query = query.And(t => t.Status == todoQuery.Status);
            }

            return query;
        }
       

    }
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var body = Expression.AndAlso(
                Expression.Invoke(expr1, parameter),
                Expression.Invoke(expr2, parameter)
            );

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }
}
