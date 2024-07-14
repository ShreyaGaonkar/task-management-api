using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;
using TaskManagerAPI.DTO.Request;
using TaskManagerAPI.DTO.Response;
using TaskManagerAPI.Helpers;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Data.Repositories
{
    public class TaskRepository : Repository<ProjectTask>, ITaskRepository
    {
        private readonly IProjectRepository _projectRepository;
        public TaskRepository(TaskManagementDbContext context, IProjectRepository projectRepository) : base(context)
        {
            _projectRepository = projectRepository;
        }

        public async Task<PaginatedList<ProjectTask>> GetPagedAsync(int projectId, TaskRequestDTO taskRequestDTO)
        {
            Expression<Func<ProjectTask, bool>> predicate = p => p.ProjectId == projectId;

            if (!string.IsNullOrEmpty(taskRequestDTO.SearchTerm))
            {
                predicate = predicate.And(p => p.TaskName.ToLower().Contains(taskRequestDTO.SearchTerm.ToLower()));
            }

            var paginatedTasks = await GetAllAsync(predicate, taskRequestDTO.SortField, taskRequestDTO.SortOrder, taskRequestDTO.PageNumber, taskRequestDTO.PageSize);

            return paginatedTasks;
        }

        public async Task<ProjectTask> GetTaskWithUserAsync(int id)
        {
            return await _context.ProjectTasks
                    .Include(p => p.Project)
                    .Include(p => p.AssignedToUser)
                    .Include(p => p.AssignedByUser)
                    .FirstOrDefaultAsync(p => p.Id == id);

        }
        public async Task AddTaskDocumentAsync(TaskDocument taskDocument)
        {
            await _context.TaskDocuments.AddAsync(taskDocument);
            await _context.SaveChangesAsync();
        }

        public async Task AddTaskNoteAsync(TaskNote taskNote)
        {
            await _context.TaskNotes.AddAsync(taskNote);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TaskNote>> GetTaskNotesAsync(int taskId)
        {
            return await _context.TaskNotes
                .Include(tn => tn.AddedByUser)
                .Where(tn => tn.TaskId == taskId)
                .ToListAsync();
        }

        public async Task<TaskNote> GetTaskNoteAsync(int taskId, int noteId)
        {
            return await _context.TaskNotes
                .Include(tn => tn.AddedByUser)
                .FirstOrDefaultAsync(tn => tn.TaskId == taskId && tn.Id == noteId);
        }

        public async Task<IEnumerable<TaskDocument>> GetTaskDocumentsAsync(int taskId)
        {
            return await _context.TaskDocuments
                .Include(td => td.UploadedByUser)
                .Where(td => td.TaskId == taskId)
                .ToListAsync();
        }

        public async Task<TaskDocument> GetTaskDocumentAsync(int taskId, int documentId)
        {
            return await _context.TaskDocuments
                .Include(td => td.UploadedByUser)
                .FirstOrDefaultAsync(td => td.TaskId == taskId && td.Id == documentId);
        }

        public async Task<List<ProjectTask>> GetTasksAsync(TaskReportDTO taskReportDTO)
        {
            IQueryable<ProjectTask> query = _context.ProjectTasks;

            query = query
                .Include(pt => pt.Project)
                    .ThenInclude(p => p.TeamProjects)
                        .ThenInclude(tp => tp.Team)
                            .ThenInclude(t => t.UserTeams)
                                .ThenInclude(ut => ut.User);

            if (taskReportDTO.TeamIds != null && taskReportDTO.TeamIds.Any())
            {
                query = query.Where(pt => pt.Project.TeamProjects.Any(tp => taskReportDTO.TeamIds.Contains(tp.TeamId)));
            }

            if (taskReportDTO.ProjectIds != null && taskReportDTO.ProjectIds.Any())
            {
                query = query.Where(pt => taskReportDTO.ProjectIds.Contains(pt.ProjectId));
            }

            var tasks = await query.ToListAsync();

            return tasks;
        }

    }
}
