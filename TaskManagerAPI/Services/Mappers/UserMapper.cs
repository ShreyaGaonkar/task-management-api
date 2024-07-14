using TaskManagerAPI.DTO.Response;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services.Mappers
{
    public static class UserMapper
    {
        public static UserProfileDTO MapToUserProfileDTO(User user)
        {
            return new UserProfileDTO
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
        }
    }
}
