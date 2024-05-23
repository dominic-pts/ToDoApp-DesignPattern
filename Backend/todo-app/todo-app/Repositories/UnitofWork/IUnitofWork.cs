using todo_app.Repositories.TodoRepository;

namespace todo_app.Repositories.UnitofWork
{
    public interface IUnitOfWork
    {
        ITodoRepository Todos { get; }
        void Commit();  
    }
}
