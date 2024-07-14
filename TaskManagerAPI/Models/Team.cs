namespace TaskManagerAPI.Models
{
    public class Team : BaseEntity<int>
    {
        public string TeamName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }


        public ICollection<UserTeam> UserTeams { get; set; }
        public ICollection<TeamProject> TeamProjects { get; set; }
    }

}
