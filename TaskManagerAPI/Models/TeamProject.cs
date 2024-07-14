namespace TaskManagerAPI.Models
{
    public class TeamProject : BaseEntity<int>
    {
        public int TeamId { get; set; }
        public int ProjectId { get; set; }
        public DateTime AssignedDate { get; set; }

        public Team Team { get; set; }
        public Project Project { get; set; }
    }
}
