using todo_app.Enums;

namespace todo_app.Filter
{
    public class TodoFilter
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public TodoEnum.TaskStatus? Status { get; set; }
    }
}
