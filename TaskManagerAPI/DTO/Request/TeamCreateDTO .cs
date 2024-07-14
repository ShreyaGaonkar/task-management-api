using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTO.Response
{
    public class TeamCreateDTO
    {
        [Required(ErrorMessage = "Team name is required")]
        [StringLength(50, ErrorMessage = "Team name cannot exceed 50 characters")]
        public string TeamName { get; set; }
        public List<int> UserIds { get; set; }
    }
}
