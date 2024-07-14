namespace TaskManagerAPI.Models
{
    public class BaseEntity<TKey>
    {
        public TKey Id { get; set; }
    }
}
