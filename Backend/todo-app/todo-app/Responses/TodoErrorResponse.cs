namespace todo_app.Responses
{
    public class TodoErrorResponse<T>
    {
        public string Type { get; }
        public T Message { get; }
        public TodoErrorResponse(string type, T message)
        {
            Type = type;
            Message = message;
        }
    }
}
