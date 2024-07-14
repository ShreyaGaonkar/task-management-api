using TaskManagerAPI.DTO.Response;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services.Mappers
{
    public static class ProjectMapper
    {
        public static ProjectDTO MapToProjectDTO(Project project)
        {
            return new ProjectDTO
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Teams = project?.TeamProjects?
                                .Where(tp => tp?.Team != null)
                                .Select(t => t.Team)
                                .Select(user => MapToTeamDTO(user))
                                .ToList() ?? new List<TeamDTO>()
            };
        }

        public static TeamDTO MapToTeamDTO(Team team)
        {
            return new TeamDTO
            {
                Id = team.Id,
                TeamName = team.TeamName
            };
        }

        public static Project MapToProject(Project project, ProjectCreateDTO projectDTO, int userId)
        {
            project.ProjectName = projectDTO.ProjectName;
            project.Description = projectDTO.Description;
            project.StartDate = projectDTO.StartDate;
            project.EndDate = projectDTO.EndDate;
            project.UpdatedBy = userId;
            project.UpdatedDate = DateTime.UtcNow;

            return project;
        }
    }
}
