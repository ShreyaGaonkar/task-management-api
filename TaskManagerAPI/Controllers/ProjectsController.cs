using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.DTO.Request;
using TaskManagerAPI.DTO.Response;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Controllers
{
    [Route("api/v1/projects")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ProjectDTO>> CreateProject([FromBody] ProjectCreateDTO projectCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _projectService.CreateProjectAsync(projectCreateDTO);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ProjectDTO>> UpdateProject(int id, [FromBody] ProjectCreateDTO projectCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _projectService.UpdateProjectAsync(id, projectCreateDTO);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDTO>> GetProject(int id)
        {
            var result = await _projectService.GetProjectByIdAsync(id);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("list")]
        public async Task<ActionResult<PaginatedList<ProjectDTO>>> GetProjects([FromBody] ProjectRequestDTO projectRequestDTO)
        {
            var result = await _projectService.GetProjectsAsync(projectRequestDTO);

            return Ok(result);
        }
    }
}
