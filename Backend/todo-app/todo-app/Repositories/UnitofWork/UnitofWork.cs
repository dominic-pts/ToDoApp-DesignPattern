using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using todo_app.Common;
using todo_app.Data;
using todo_app.Repositories.TodoRepository;

namespace todo_app.Repositories.UnitofWork
{
    public class UnitofWork : IUnitOfWork
    {
        private TodoDbContext _context { get; set; }

        public ITodoRepository Todos { get; }

        public UnitofWork(ITodoRepository todoRepository, TodoDbContext todoDbContext) 
        {
            Todos = todoRepository;
            _context = todoDbContext;
        }
        
        public void Commit()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }       
    }
}
