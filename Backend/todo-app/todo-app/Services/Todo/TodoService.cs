using AutoMapper;
using todo_app.Contracts;
using todo_app.Data;
using todo_app.DTOs;
using todo_app.DTOs.Queries;
using todo_app.Entities;
using todo_app.Filter;
using todo_app.Model.Request;
using todo_app.Repositories.TodoRepository;
using todo_app.Repositories.UnitofWork;


namespace todo_app.Services.TodoService
{
    public class TodoService : ITodoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TodoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task AddAsync(TodoRequest todoRequest, CancellationToken cancellationToken = default)
        {
            var mapper = _mapper.Map<Todo>(todoRequest);
            _unitOfWork.Todos.Add(mapper);
            _unitOfWork.Commit();
        }

        public async Task<int> CountAsync(TodoFilter todoFilter, CancellationToken cancellationToken = default)
        {
            var mapper = _mapper.Map<TodoQuery>(todoFilter);
            return await _unitOfWork.Todos.CountAsync(mapper);
        }

        public async Task Delete(Guid Id, CancellationToken cancellationToken = default)
        {
            var todo = await _unitOfWork.Todos.GetByIdAsync(Id);
            if (todo is null)
            {
                throw new KeyNotFoundException($"{Id}");
            }
            _unitOfWork.Todos.Delete(todo);
            _unitOfWork.Commit();
        }
        public async Task<IEnumerable<TodoDTO>> GetAllAsync(TodoFilter todoFilter, IPagingParams pagingParams, CancellationToken cancellationToken = default)
        {
            var mapper = _mapper.Map<TodoQuery>(todoFilter);
            return _mapper.Map<IEnumerable<TodoDTO>>(await _unitOfWork.Todos.GetAllAsync(mapper, pagingParams, cancellationToken));
        }

        public async Task<TodoDTO> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            var todo = await _unitOfWork.Todos.GetByIdAsync(Id);
            if (todo is null)
            {
                throw new KeyNotFoundException($"{Id}");
            }
            return _mapper.Map<TodoDTO>(todo);
        }

        public async Task UpdateAsync(Guid Id, TodoRequest todoRequest, CancellationToken cancellationToken = default)
        {
            var todo = await _unitOfWork.Todos.GetByIdAsync(Id);
            if (todo is null)
            {
                throw new KeyNotFoundException($"{Id}");
            }
            var mapper = _mapper.Map(todoRequest, todo);
            _unitOfWork.Todos.Update(todo);
            _unitOfWork.Commit();
        }
    }
}
