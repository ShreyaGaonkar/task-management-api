using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.DTO.Request;
using TaskManagerAPI.DTO.Response;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Controllers
{
    [Route("api/v1/teams")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpPost]
        public async Task<ActionResult<TeamDTO>> CreateTeam([FromBody] TeamCreateDTO teamCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _teamService.CreateTeamAsync(teamCreateDTO);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TeamDTO>> UpdateTeam(int id, [FromBody] TeamCreateDTO teamCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _teamService.UpdateTeamAsync(id, teamCreateDTO);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDTO>> GetTeam(int id)
        {
            var result = await _teamService.GetTeamByIdAsync(id);

            return Ok(result);
        }

        [HttpPost("list")]
        public async Task<ActionResult<PaginatedList<TeamDTO>>> GetTeams([FromBody] TeamRequestDTO teamRequestDTO)
        {
            var result = await _teamService.GetTeamsAsync(teamRequestDTO);

            return Ok(result);
        }
    }
}
