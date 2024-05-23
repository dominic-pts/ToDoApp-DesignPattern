namespace todo_app.Common
{
    public interface IDatedModification
    {
        DateTime CreateAt {  get; set; }
        DateTime UpdateAt { get; set; }
    }
}
