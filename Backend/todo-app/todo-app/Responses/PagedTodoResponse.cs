namespace todo_app.Responses
{
    public class PagedTodoResponse<T>:TodoResponse<T> 
    {
        public int PageNumber { get { return PageIndex + 1; }set { PageIndex = value - 1; } }
        public int PageSize { get; set; }
        public int TotalPage { get { var total = TotalItems / PageSize;if (TotalItems % PageSize>0) { ++total; } return total; } }
        public int TotalItems { get; set; }
        public int PageIndex { get; private set ; }
        public PagedTodoResponse(IEnumerable<T> data,int pageNumber,int pageSize,int totalItems) :base(data)
        {
            PageNumber = pageNumber;
            TotalItems= totalItems;
            PageSize = pageSize;
        }
    }
}
