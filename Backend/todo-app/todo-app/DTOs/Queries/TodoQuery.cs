using todo_app.Enums;

namespace todo_app.DTOs.Queries
{
    public class TodoQuery
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public TodoEnum.TaskStatus? Status { get; set; }
    }
}
