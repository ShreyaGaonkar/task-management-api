using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManagerAPI.DTO.Request;
using TaskManagerAPI.DTO.Response;
using TaskManagerAPI.Helpers;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Data.Repositories
{
    public class TeamRepository : Repository<Team>, ITeamRepository
    {
        public TeamRepository(TaskManagementDbContext context) : base(context)
        {
        }

        public async Task<Team> GetTeamByName(string name)
        {
            return await _context.Teams.FirstOrDefaultAsync(t => t.TeamName.ToLower() == name.ToLower());
        }

        public async Task<Team> GetTeamWithMembersAsync(int id)
        {
            return await _context.Teams
                    .Include(t => t.UserTeams)
                    .ThenInclude(ut => ut.User)
                    .FirstOrDefaultAsync(t => t.Id == id);

        }

        public async Task<PaginatedList<Team>> GetPagedAsync(TeamRequestDTO teamRequestDTO)
        {
            Expression<Func<Team, bool>> predicate = p => true;

            if (!string.IsNullOrEmpty(teamRequestDTO.SearchTerm))
            {
                predicate = predicate.And(p => p.TeamName.ToLower().Contains(teamRequestDTO.SearchTerm.ToLower()));
            }

            var paginatedProjects = await GetAllAsync(predicate, teamRequestDTO.SortField, teamRequestDTO.SortOrder, teamRequestDTO.PageNumber, teamRequestDTO.PageSize);

            return paginatedProjects;
        }
    }
}
