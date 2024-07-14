using System.Security.Claims;
using TaskManagerAPI.Helpers;

namespace TaskManagerAPI.Middlewares
{
    public class UserContextMiddleware
    {
        private readonly RequestDelegate _next;

        public UserContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments("/api/v1/users/register") && !context.Request.Path.StartsWithSegments("/api/v1/users/login"))
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = context.User.FindFirst(ClaimTypes.Role)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
                {
                    throw new UnauthorizedAccessException(Captions.InvalidUserId);
                }

                if (string.IsNullOrEmpty(role))
                {
                    throw new UnauthorizedAccessException(Captions.InvalidRole);
                }

                context.Items["UserId"] = parsedUserId;
                context.Items["Role"] = role;
            }
            await _next(context);
        }
    }
}
