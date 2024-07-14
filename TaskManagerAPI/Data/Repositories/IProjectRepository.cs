using TaskManagerAPI.DTO.Request;
using TaskManagerAPI.DTO.Response;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Data.Repositories
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<Project> GetProjectByName(string name);
        Task<Project> GetProjectWithTeamsAsync(int id);
        Task<Project> GetProjectWithTaskAsync(int id, int taskId);
        Task<PaginatedList<Project>> GetPagedAsync(int userId, string role, ProjectRequestDTO projectRequestDTO);
        Task<bool> UserHasAccessToProjectAsync(int userId, string role, int projectId);
        Task<List<int>> GetAccessibleProjectIdsAsync(int userId, string role);
    }
}
