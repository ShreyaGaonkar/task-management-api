using Microsoft.AspNetCore.JsonPatch;
using TaskManagerAPI.Data.Repositories;
using TaskManagerAPI.DTO.Request;
using TaskManagerAPI.DTO.Response;
using TaskManagerAPI.Exceptions;
using TaskManagerAPI.Helpers;
using TaskManagerAPI.Models;
using TaskManagerAPI.Services.Mappers;

namespace TaskManagerAPI.Services
{
    public interface ITaskService
    {
        Task<TaskDTO> CreateTaskAsync(int projectId, TaskCreateDTO taskCreateDTO);
        Task UpdateTaskAsync(int projectId, int taskId, JsonPatchDocument<ProjectTask> patch);
        Task<TaskDTO> GetTaskAsync(int projectId, int taskId);
        Task<PaginatedList<TaskDTO>> GetTasksAsync(int projectId, TaskRequestDTO taskRequestDTO);
        Task AddTaskDocumentAsync(int projectId, int taskId, List<IFormFile> files);
        Task<TaskNoteResponseDTO> AddTaskNoteAsync(int projectId, int taskId, TaskNoteDTO taskNoteDto);
        Task<IEnumerable<TaskNoteResponseDTO>> GetTaskNotesAsync(int projectId, int taskId);
        Task<TaskNoteResponseDTO> GetTaskNoteAsync(int projectId, int taskId, int noteId);
        Task<IEnumerable<TaskDocumentResponseDTO>> GetTaskDocumentsAsync(int projectId, int taskId);
        Task<TaskDocumentResponseDTO> GetTaskDocumentAsync(int projectId, int taskId, int documentId);

    }

    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserContextService _userContextService;

        public TaskService(ITaskRepository taskRepository, IProjectRepository projectRepository, IUserContextService userContextService)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _userContextService = userContextService;
        }

        public async Task<TaskDTO> CreateTaskAsync(int projectId, TaskCreateDTO taskCreateDTO)
        {
            var projectExist = await _projectRepository.GetByIdAsync(projectId);

            if (projectExist == null)
            {
                throw new NotFoundException(Captions.ProjectNotFound);
            }

            var hasProjectAccess = await _projectRepository.UserHasAccessToProjectAsync(_userContextService.UserId, _userContextService.Role, projectId);
            if (!hasProjectAccess)
            {
                throw new ForbiddenException(Captions.AccessDenied);
            }

            var taskDTO = new ProjectTask
            {
                TaskName = taskCreateDTO.TaskName,
                Description = taskCreateDTO.Description,
                ProjectId = projectId,
                AssignedToUserId = taskCreateDTO.AssignedToUserId,
                AssignedByUserId = _userContextService.UserId,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = _userContextService.UserId,
                Priority = taskCreateDTO.Priority,
                Status = taskCreateDTO.Status

            };

            var Newtask = await _taskRepository.InsertAsync(taskDTO);

            var task = await _taskRepository.GetTaskWithUserAsync(Newtask.Id);

            return TaskMapper.MapToTaskDTO(task);
        }

        public async Task UpdateTaskAsync(int projectId, int taskId, JsonPatchDocument<ProjectTask> patchDoc)
        {
            await ValidateProjectTaskAsync(projectId, taskId);

            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
            {
                throw new NotFoundException(Captions.TaskNotFound);
            }

            patchDoc.ApplyTo(task);
            task.UpdatedDate = DateTime.UtcNow;
            task.UpdatedBy = _userContextService.UserId;

            await _taskRepository.UpdateAsync(task);

        }

        public async Task<TaskDTO> GetTaskAsync(int projectId, int taskId)
        {
            await ValidateProjectTaskAsync(projectId, taskId);

            var task = await _taskRepository.GetTaskWithUserAsync(taskId);

            return TaskMapper.MapToTaskDTO(task);
        }

        public async Task<PaginatedList<TaskDTO>> GetTasksAsync(int projectId, TaskRequestDTO taskRequestDTO)
        {
            var projectExist = await _projectRepository.GetByIdAsync(projectId);

            if (projectExist == null)
            {
                throw new NotFoundException(Captions.ProjectNotFound);
            }

            var hasProjectAccess = await _projectRepository.UserHasAccessToProjectAsync(_userContextService.UserId, _userContextService.Role, projectId);
            if (!hasProjectAccess)
            {
                throw new ForbiddenException(Captions.AccessDenied);
            }

            var paginatedTasks = await _taskRepository.GetPagedAsync(projectId, taskRequestDTO);

            var taskDTOs = paginatedTasks.Data.Select(x => TaskMapper.MapToTaskDTO(x)).ToList();
            var paginatedDTO = new PaginatedList<TaskDTO>(taskDTOs, paginatedTasks.Total);

            return paginatedDTO;
        }

        public async Task AddTaskDocumentAsync(int projectId, int taskId, List<IFormFile> files)
        {
            await ValidateProjectTaskAsync(projectId, taskId);

            var uploadsFolderPath = Path.Combine("Documents");
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            foreach (var file in files)
            {
                var filePath = Path.Combine(uploadsFolderPath, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);

                var taskDocument = new TaskDocument
                {
                    TaskId = taskId,
                    Name = fileNameWithoutExtension,
                    Path = filePath,
                    UploadedByUserId = _userContextService.UserId,
                    UploadedDate = DateTime.UtcNow
                };

                await _taskRepository.AddTaskDocumentAsync(taskDocument);
            }

        }

        public async Task<TaskNoteResponseDTO> AddTaskNoteAsync(int projectId, int taskId, TaskNoteDTO taskNoteDto)
        {
            await ValidateProjectTaskAsync(projectId, taskId);

            var NewtaskNote = new TaskNote
            {
                TaskId = taskId,
                Note = taskNoteDto.Note,
                AddedByUserId = _userContextService.UserId,
                AddedDate = DateTime.UtcNow
            };

            await _taskRepository.AddTaskNoteAsync(NewtaskNote);

            var taskNote = await _taskRepository.GetTaskNoteAsync(taskId, NewtaskNote.Id);
            return TaskMapper.MapToTaskNoteResponseDTO(taskNote);
        }

        public async Task<IEnumerable<TaskNoteResponseDTO>> GetTaskNotesAsync(int projectId, int taskId)
        {
            await ValidateProjectTaskAsync(projectId, taskId);

            var taskNotes = await _taskRepository.GetTaskNotesAsync(taskId);
            return taskNotes.Select(x => TaskMapper.MapToTaskNoteResponseDTO(x));
        }

        public async Task<TaskNoteResponseDTO> GetTaskNoteAsync(int projectId, int taskId, int noteId)
        {
            await ValidateProjectTaskAsync(projectId, taskId);

            var taskNote = await _taskRepository.GetTaskNoteAsync(taskId, noteId);
            return TaskMapper.MapToTaskNoteResponseDTO(taskNote);
        }

        public async Task<IEnumerable<TaskDocumentResponseDTO>> GetTaskDocumentsAsync(int projectId, int taskId)
        {
            await ValidateProjectTaskAsync(projectId, taskId);

            var taskDocuments = await _taskRepository.GetTaskDocumentsAsync(taskId);
            return taskDocuments.Select(x => TaskMapper.MapToTaskDocumentResponseDTO(x));
        }

        public async Task<TaskDocumentResponseDTO> GetTaskDocumentAsync(int projectId, int taskId, int documentId)
        {
            await ValidateProjectTaskAsync(projectId, taskId);

            var taskDocument = await _taskRepository.GetTaskDocumentAsync(taskId, documentId);
            return TaskMapper.MapToTaskDocumentResponseDTO(taskDocument);
        }

        private async Task ValidateProjectTaskAsync(int projectId, int taskId)
        {
            var projectTaskExist = await _projectRepository.GetProjectWithTaskAsync(projectId, taskId);

            if (projectTaskExist == null)
            {
                throw new NotFoundException(Captions.ProjectTaskNotFound);
            }

            var hasProjectAccess = await _projectRepository.UserHasAccessToProjectAsync(_userContextService.UserId, _userContextService.Role, projectId);
            if (!hasProjectAccess)
            {
                throw new ForbiddenException(Captions.AccessDenied);
            }
        }

    }
}
