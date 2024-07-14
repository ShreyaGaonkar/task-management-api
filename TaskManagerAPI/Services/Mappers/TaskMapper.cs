using TaskManagerAPI.DTO.Response;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services.Mappers
{
    public static class TaskMapper
    {
        public static TaskDTO MapToTaskDTO(ProjectTask task)
        {
            return new TaskDTO
            {
                Id = task.Id,
                TaskName = task.TaskName,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority,
                Project = task.Project?.ProjectName,
                AssignedBy = MapToUserDTO(task.AssignedByUser),
                AssignedTo = MapToUserDTO(task.AssignedToUser),
            };
        }

        public static UserDTO MapToUserDTO(User user)
        {
            if (user == null)
            {
                return null;
            }

            return new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public static ProjectTask MapToTask(ProjectTask task, TaskDTO taskDTO, int userId)
        {
            task.TaskName = taskDTO.TaskName;
            task.Description = taskDTO.Description;
            task.DueDate = taskDTO.DueDate;
            task.Priority = taskDTO.Priority;
            task.UpdatedBy = userId;
            task.UpdatedDate = DateTime.UtcNow;

            return task;
        }

        public static TaskDocumentResponseDTO MapToTaskDocumentResponseDTO(TaskDocument taskDocument)
        {
            if (taskDocument == null)
                return null;

            return new TaskDocumentResponseDTO
            {
                Name = taskDocument.Name,
                Path = taskDocument.Path,
                UploadedByUser = taskDocument.UploadedByUser.FirstName + " " + taskDocument.UploadedByUser.LastName,
                UploadedDate = taskDocument.UploadedDate
            };
        }

        public static TaskNoteResponseDTO MapToTaskNoteResponseDTO(TaskNote taskNote)
        {
            if (taskNote == null)
                return null;

            return new TaskNoteResponseDTO
            {
                Note = taskNote.Note,
                AddedDate = taskNote.AddedDate,
                AddedByUser = $"{taskNote.AddedByUser.FirstName} {taskNote.AddedByUser.LastName}",
            };
        }
    }
}
