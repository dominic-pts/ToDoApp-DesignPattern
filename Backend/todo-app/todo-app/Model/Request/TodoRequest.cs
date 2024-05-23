using System.ComponentModel.DataAnnotations;
using todo_app.Enums;

namespace todo_app.Model.Request
{
    public class TodoRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public TodoEnum.TaskStatus Status { get; set; }
    }
}
