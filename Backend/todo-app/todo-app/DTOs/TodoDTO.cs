﻿using todo_app.Common;
using todo_app.Enums;

namespace todo_app.DTOs
{
    public class TodoDTO: BaseDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public TodoEnum.TaskStatus Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
