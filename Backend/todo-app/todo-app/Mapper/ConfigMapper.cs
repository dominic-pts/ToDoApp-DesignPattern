using AutoMapper;
using todo_app.DTOs;
using todo_app.DTOs.Queries;
using todo_app.Entities;
using todo_app.Filter;
using todo_app.Model.Request;

namespace todo_app.Mapper
{
    public class ConfigMapper : Profile
    {
        public ConfigMapper()
        {
            CreateMap<TodoRequest, Todo>().ReverseMap();
            CreateMap<TodoDTO, Todo>().ReverseMap();
            CreateMap<TodoQuery, TodoFilter>().ReverseMap();
        }
    }
}
