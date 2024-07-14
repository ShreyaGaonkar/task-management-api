namespace TaskManagerAPI.Models
{
    public class ProjectTask : BaseEntity<int>
    {
        public int ProjectId { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public int? AssignedToUserId { get; set; }
        public int AssignedByUserId { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }


        public Project Project { get; set; }
        public User AssignedToUser { get; set; }
        public User AssignedByUser { get; set; }
        public ICollection<TaskDocument> TaskDocuments { get; set; }
        public ICollection<TaskNote> TaskNotes { get; set; }
    }

}
