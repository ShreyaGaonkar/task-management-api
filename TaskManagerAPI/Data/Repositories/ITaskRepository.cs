using TaskManagerAPI.DTO.Request;
using TaskManagerAPI.DTO.Response;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Data.Repositories
{
    public interface ITaskRepository : IRepository<ProjectTask>
    {
        Task<PaginatedList<ProjectTask>> GetPagedAsync(int projectId, TaskRequestDTO taskRequestDTO);
        Task<ProjectTask> GetTaskWithUserAsync(int id);
        Task AddTaskDocumentAsync(TaskDocument taskDocument);
        Task AddTaskNoteAsync(TaskNote taskNote);
        Task<IEnumerable<TaskNote>> GetTaskNotesAsync(int taskId);
        Task<TaskNote> GetTaskNoteAsync(int taskId, int noteId);
        Task<IEnumerable<TaskDocument>> GetTaskDocumentsAsync(int taskId);
        Task<TaskDocument> GetTaskDocumentAsync(int taskId, int documentId);
        Task<List<ProjectTask>> GetTasksAsync(TaskReportDTO taskReportDTO);
    }
}
