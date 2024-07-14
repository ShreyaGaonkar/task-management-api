using TaskManagerAPI.Helpers;

namespace TaskManagerAPI.Services
{
    public interface IUserContextService
    {
        int UserId { get; }
        string Role { get; }
    }

    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId
        {
            get
            {
                if (_httpContextAccessor.HttpContext.Items.TryGetValue("UserId", out var userIdObj) && userIdObj is int userId)
                {
                    return userId;
                }

                throw new UnauthorizedAccessException(Captions.InvalidUserId);
            }
        }

        public string Role
        {
            get
            {
                if (_httpContextAccessor.HttpContext.Items.TryGetValue("Role", out var roleObj) && roleObj is string role)
                {
                    return role;
                }

                throw new UnauthorizedAccessException("Role information not available.");
            }
        }
    }
}
