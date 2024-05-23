using Microsoft.EntityFrameworkCore;
using todo_app.Data;
using todo_app.Decorator;
using todo_app.Repositories.TodoRepository;
using todo_app.Repositories.UnitofWork;
using todo_app.Services.TodoService;


var builder = WebApplication.CreateBuilder(args);
const string Cors = "Allow*";
// Add services to the container.
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddDbContext<TodoDbContext>(options => options.UseMySQL(builder.Configuration.GetConnectionString("TodoDb")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IUnitOfWork, UnitofWork>();
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();
// Đăng ký decorator cho repository
builder.Services.Decorate<ITodoRepository, CachingTodoRepository>();
builder.Services.AddCors(options
                                       => options.AddPolicy(Cors
                                       , policies =>
                                               policies
                                               .AllowAnyHeader()
                                               .SetIsOriginAllowed((host) => true)
                                               .AllowCredentials()
                                               .AllowAnyMethod()));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(Cors);
app.UseAuthorization();

app.MapControllers();

app.Run();
