namespace TaskManagerAPI.Models
{
    public class Project : BaseEntity<int>
    {
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }


        public ICollection<ProjectTask> Tasks { get; set; }
        public ICollection<TeamProject> TeamProjects { get; set; }
    }

}
