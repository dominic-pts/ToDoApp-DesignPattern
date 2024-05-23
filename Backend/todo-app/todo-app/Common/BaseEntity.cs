namespace todo_app.Common
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public BaseEntity() 
        { 
            Id = Guid.NewGuid();
        }
    }
}
