using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.DTO.Request;
using TaskManagerAPI.DTO.Response;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;

        public UsersController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ServiceResult<UserProfileDTO>>> Register(UserRequestDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(userDTO);
            if (!result.Success)
                return BadRequest(new { error = result.Message });

            return Ok(result.Data);

        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResultDTO>> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(loginDTO);
            if (!result.Success)
                return BadRequest(new { error = result.Message });

            return Ok(result);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileDTO>> Profile()
        {
            var userProfile = await _authService.GetProfile();
            return Ok(userProfile);
        }

    }
}
