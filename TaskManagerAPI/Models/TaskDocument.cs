namespace TaskManagerAPI.Models
{
    public class TaskDocument : BaseEntity<int>
    {
        public int TaskId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int UploadedByUserId { get; set; }
        public DateTime UploadedDate { get; set; }

        public ProjectTask Task { get; set; }
        public User UploadedByUser { get; set; }
    }

}
