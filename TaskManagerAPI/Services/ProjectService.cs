using TaskManagerAPI.Data.Repositories;
using TaskManagerAPI.DTO.Request;
using TaskManagerAPI.DTO.Response;
using TaskManagerAPI.Exceptions;
using TaskManagerAPI.Helpers;
using TaskManagerAPI.Models;
using TaskManagerAPI.Services.Mappers;

namespace TaskManagerAPI.Services
{
    public interface IProjectService
    {
        Task<ProjectDTO> CreateProjectAsync(ProjectCreateDTO projectCreateDTO);
        Task<ProjectDTO> GetProjectByIdAsync(int id);
        Task<PaginatedList<ProjectDTO>> GetProjectsAsync(ProjectRequestDTO projectRequestDTO);
        Task<ProjectDTO> UpdateProjectAsync(int id, ProjectCreateDTO projectCreateDTO);
    }

    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<TeamProject> _teamProjectRepository;
        private readonly IUserContextService _userContextService;

        public ProjectService(IProjectRepository projectRepository, IUserRepository userRepository, IRepository<TeamProject> teamProjectRepository, IUserContextService userContextService)
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _teamProjectRepository = teamProjectRepository;
            _userContextService = userContextService;
        }

        public async Task<ProjectDTO> CreateProjectAsync(ProjectCreateDTO projectCreateDTO)
        {
            var projectExists = await _projectRepository.GetProjectByName(projectCreateDTO.ProjectName);
            if (projectExists != null)
            {
                throw new NotFoundException(Captions.ProjectNameAlreadyExist);
            }

            var projectDTO = new Project
            {
                ProjectName = projectCreateDTO.ProjectName,
                Description = projectCreateDTO.Description,
                StartDate = projectCreateDTO.StartDate,
                EndDate = projectCreateDTO.EndDate,
                CreatedBy = _userContextService.UserId,
                CreatedDate = DateTime.UtcNow
            };

            var Newproject = await _projectRepository.InsertAsync(projectDTO);

            if (projectCreateDTO.TeamIds != null && projectCreateDTO.TeamIds.Any())
            {
                var teamProjects = projectCreateDTO.TeamIds.Select(teamId => new TeamProject
                {
                    TeamId = teamId,
                    ProjectId = Newproject.Id,
                    AssignedDate = DateTime.UtcNow
                }).ToList();

                await _teamProjectRepository.InsertRangeAsync(teamProjects);
            }

            var project = await _projectRepository.GetProjectWithTeamsAsync(Newproject.Id);
            return ProjectMapper.MapToProjectDTO(project);
        }

        public async Task<ProjectDTO> GetProjectByIdAsync(int id)
        {
            var project = await _projectRepository.GetProjectWithTeamsAsync(id);

            if (project == null)
            {
                throw new NotFoundException(Captions.ProjectNotFound);
            }

            var hasProjectAccess = await _projectRepository.UserHasAccessToProjectAsync(_userContextService.UserId, _userContextService.Role, id);
            if (!hasProjectAccess)
            {
                throw new ForbiddenException(Captions.AccessDenied);
            }

            return ProjectMapper.MapToProjectDTO(project);
        }

        public async Task<PaginatedList<ProjectDTO>> GetProjectsAsync(ProjectRequestDTO projectRequestDTO)
        {
            var paginatedProjects = await _projectRepository.GetPagedAsync(_userContextService.UserId, _userContextService.Role, projectRequestDTO);

            var projectDTOs = paginatedProjects.Data.Select(x => ProjectMapper.MapToProjectDTO(x)).ToList();
            var paginatedDTO = new PaginatedList<ProjectDTO>(projectDTOs, paginatedProjects.Total);

            return paginatedDTO;
        }

        public async Task<ProjectDTO> UpdateProjectAsync(int id, ProjectCreateDTO projectCreateDTO)
        {
            var projectExists = await _projectRepository.GetProjectByName(projectCreateDTO.ProjectName);
            if (projectExists != null && projectExists.Id != id)
            {
                throw new NotFoundException(Captions.ProjectNameAlreadyExist);
            }

            var projectExist = await _projectRepository.GetByIdAsync(id);

            if (projectExist == null)
            {
                throw new NotFoundException(Captions.ProjectNotFound);
            }

            var hasProjectAccess = await _projectRepository.UserHasAccessToProjectAsync(_userContextService.UserId, _userContextService.Role, id);
            if (!hasProjectAccess)
            {
                throw new ForbiddenException(Captions.AccessDenied);
            }

            var existingProject = ProjectMapper.MapToProject(projectExist, projectCreateDTO, _userContextService.UserId);

            await _projectRepository.UpdateAsync(existingProject);

            var existingTeamProjects = await _teamProjectRepository.GetAllAsync(tp => tp.ProjectId == id);
            var existingTeamIds = existingTeamProjects.Select(tp => tp.TeamId).ToList();
            var newTeamIds = projectCreateDTO.TeamIds;

            var teamIdsToRemove = existingTeamIds.Except(newTeamIds).ToList();
            if (teamIdsToRemove.Any())
            {
                var teamProjectsToRemove = existingTeamProjects.Where(tp => teamIdsToRemove.Contains(tp.TeamId)).ToList();
                await _teamProjectRepository.DeleteRangeAsync(teamProjectsToRemove);
            }

            var teamIdsToAdd = newTeamIds.Except(existingTeamIds).ToList();
            if (teamIdsToAdd.Any())
            {
                var teamProjectsToAdd = teamIdsToAdd.Select(teamId => new TeamProject
                {
                    TeamId = teamId,
                    ProjectId = id,
                    AssignedDate = DateTime.UtcNow
                }).ToList();
                await _teamProjectRepository.InsertRangeAsync(teamProjectsToAdd);
            }

            var project = await _projectRepository.GetProjectWithTeamsAsync(id);
            return ProjectMapper.MapToProjectDTO(project);
        }
    }
}
