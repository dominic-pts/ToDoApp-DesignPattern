using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using todo_app.DTOs;
using todo_app.Filter;
using todo_app.Model;
using todo_app.Model.Request;
using todo_app.Responses;
using todo_app.Services.TodoService;

namespace todo_app.Controllers
{
    [ApiController]
    [Route("api/todo")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todosService;
        public TodoController(ITodoService todoService)
        {
            _todosService = todoService;
        }

        [HttpGet("{id:Guid}")]

        public async Task<IActionResult> GetById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            try
            {
                 var result = await _todosService.GetByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException e)
            {

                return NotFound(new TodoErrorResponse<string>($"Not Found {id}",e.Message ));
            }
           
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedTodoResponse<TodoDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] TodoFilter todoFilter, [FromQuery] PagingModel pagingModel)
        {
            var validPageNumber = Math.Max(pagingModel.PageNumber, 1);
            var validPageSize = pagingModel.PageSize < 0 ? 10 : pagingModel.PageSize;
            var result = await _todosService.GetAllAsync(todoFilter, pagingModel);
            var total = await _todosService.CountAsync(todoFilter);
            return Ok(new PagedTodoResponse<TodoDTO>(result, validPageNumber, validPageSize, total));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Add(TodoRequest todoRequest)
        {
            if (todoRequest == null)
            {
                return BadRequest("Request cannot be null");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new TodoErrorResponse<IEnumerable<string>>("Model isn't valid", errors));
            }
            await _todosService.AddAsync(todoRequest);

            return Ok();
        }

        [HttpPut("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Guid id, [FromBody] TodoRequest todoRequest)
        {
            try
            {
                await _todosService.UpdateAsync(id, todoRequest);
            }
            catch (Exception ex)
            {
                return BadRequest(new TodoErrorResponse<string>(ex.GetType().Name, null));
            }
            return NoContent();
        }

        [HttpDelete("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _todosService.Delete(id);
            }
            catch (Exception ex)
            {
                return BadRequest(new TodoErrorResponse<string>(ex.GetType().Name, null));
            }
            return NoContent();
        }
    }
}
