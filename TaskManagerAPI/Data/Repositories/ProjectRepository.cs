using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;
using TaskManagerAPI.DTO.Request;
using TaskManagerAPI.DTO.Response;
using TaskManagerAPI.Helpers;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Data.Repositories
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        public ProjectRepository(TaskManagementDbContext context) : base(context)
        {
        }

        public async Task<Project> GetProjectByName(string name)
        {
            return await _context.Projects.FirstOrDefaultAsync(p => p.ProjectName.ToLower() == name.ToLower());
        }

        public async Task<Project> GetProjectWithTeamsAsync(int id)
        {
            return await _context.Projects
                    .Include(p => p.TeamProjects)
                    .ThenInclude(tp => tp.Team)
                    .FirstOrDefaultAsync(p => p.Id == id);

        }

        public async Task<Project> GetProjectWithTaskAsync(int id, int taskId)
        {
            return await _context.Projects
                           .Include(p => p.Tasks)
                           .FirstOrDefaultAsync(p => p.Id == id && p.Tasks.Any(t => t.Id == taskId));

        }

        public async Task<PaginatedList<Project>> GetPagedAsync(int userId, string role, ProjectRequestDTO projectRequestDTO)
        {
            var accessibleProjectIds = await GetAccessibleProjectIdsAsync(userId, role);
            Expression<Func<Project, bool>> predicate = p => true;

            if (accessibleProjectIds.Any())
            {
                predicate = predicate.And(p => accessibleProjectIds.Contains(p.Id));
            }
            else
            {
                return new PaginatedList<Project>(new List<Project>(), 0);
            }

            if (!string.IsNullOrEmpty(projectRequestDTO.SearchTerm))
            {
                predicate = predicate.And(p => p.ProjectName.ToLower().Contains(projectRequestDTO.SearchTerm.ToLower()));
            }

            var paginatedProjects = await GetAllAsync(predicate, projectRequestDTO.SortField, projectRequestDTO.SortOrder, projectRequestDTO.PageNumber, projectRequestDTO.PageSize);

            return paginatedProjects;
        }

        public async Task<bool> UserHasAccessToProjectAsync(int userId, string role, int projectId)
        {

            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == role);
            var userTeams = await _context.UserTeams
                                           .Where(ut => ut.UserId == userId)
                                           .Select(ut => ut.TeamId)
                                           .ToListAsync();

            if (userRole.RoleName == "Admin")
            {
                return true;
            }

            var accessibleProjects = await _context.Projects
                                                  .Where(p => userRole.RoleName != "Admin" &&
                                                              (p.TeamProjects.Any(tp => userTeams.Contains(tp.TeamId))))
                                                  .Select(p => p.Id)
                                                  .ToListAsync();

            return accessibleProjects.Contains(projectId);
        }

        public async Task<List<int>> GetAccessibleProjectIdsAsync(int userId, string role)
        {
            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName.ToLower() == role.ToLower());
            var userTeams = await _context.UserTeams
                                           .Where(ut => ut.UserId == userId)
                                           .Select(ut => ut.TeamId)
                                           .ToListAsync();

            if (userRole.RoleName == "Admin")
            {
                return await _context.Projects
                                      .Select(p => p.Id)
                                      .ToListAsync();
            }

            var accessibleProjects = await _context.Projects
                                                  .Where(p => userRole.RoleName != "Admin" &&
                                                              (p.TeamProjects.Any(tp => userTeams.Contains(tp.TeamId))))
                                                  .Select(p => p.Id)
                                                  .ToListAsync();

            return accessibleProjects;
        }
    }
}
