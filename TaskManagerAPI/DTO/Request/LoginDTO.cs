using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTO.Request
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
