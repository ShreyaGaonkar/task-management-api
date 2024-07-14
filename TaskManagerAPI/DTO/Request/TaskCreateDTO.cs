namespace TaskManagerAPI.DTO.Response
{
    public class TaskCreateDTO
    {
        public string TaskName { get; set; }
        public string Description { get; set; }
        public int AssignedToUserId { get; set; }
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; } = "Medium";
        public string Status { get; set; } = "ToDo";

    }
}
