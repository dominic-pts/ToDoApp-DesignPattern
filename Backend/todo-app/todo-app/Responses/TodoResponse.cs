namespace todo_app.Responses
{
    public class TodoResponse<T> 
    {
        public IEnumerable<T> Data { get; set; }
        public TodoResponse(IEnumerable<T> data)
        {
            this.Data = data;
        }
    }
}
