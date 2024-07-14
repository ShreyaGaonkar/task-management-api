namespace TaskManagerAPI.Models
{
    public class UserTeam : BaseEntity<int>
    {
        public int UserId { get; set; }
        public int TeamId { get; set; }
        public DateTime AssignedDate { get; set; }

        public User User { get; set; }
        public Team Team { get; set; }
    }

}
