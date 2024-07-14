namespace TaskManagerAPI.Models
{
    public class User : BaseEntity<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }


        public Role Role { get; set; }
        public ICollection<UserTeam> UserTeams { get; set; }
        public ICollection<ProjectTask> AssignedTasks { get; set; }
        public ICollection<ProjectTask> CreatedTasks { get; set; }
    }

}
