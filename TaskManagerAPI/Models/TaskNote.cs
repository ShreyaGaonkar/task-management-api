namespace TaskManagerAPI.Models
{
    public class TaskNote : BaseEntity<int>
    {
        public int TaskId { get; set; }
        public string Note { get; set; }
        public int AddedByUserId { get; set; }
        public DateTime AddedDate { get; set; }

        public ProjectTask Task { get; set; }
        public User AddedByUser { get; set; }
    }
}
