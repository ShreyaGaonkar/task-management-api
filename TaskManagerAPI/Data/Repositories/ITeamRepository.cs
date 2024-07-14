using TaskManagerAPI.DTO.Request;
using TaskManagerAPI.DTO.Response;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Data.Repositories
{
    public interface ITeamRepository : IRepository<Team>
    {
        Task<Team> GetTeamByName(string name);
        Task<Team> GetTeamWithMembersAsync(int id);
        Task<PaginatedList<Team>> GetPagedAsync(TeamRequestDTO teamRequestDTO);
    }
}
