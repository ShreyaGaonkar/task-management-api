using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTO.Response
{
    public class ProjectCreateDTO
    {
        [Required(ErrorMessage = "Project Name is required")]
        [StringLength(50, ErrorMessage = "Project Name cannot exceed 50 characters")]
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<int> TeamIds { get; set; }
    }
}
