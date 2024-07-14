namespace TaskManagerAPI.DTO.Response
{
    public class TaskDTO
    {
        public int Id { get; set; }
        public string TaskName { get; set; }
        public string Project { get; set; }
        public string Priority { get; set; }
        public string Description { get; set; }
        public UserDTO AssignedTo { get; set; }
        public UserDTO AssignedBy { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
