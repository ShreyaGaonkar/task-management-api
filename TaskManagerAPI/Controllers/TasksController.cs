using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.DTO.Request;
using TaskManagerAPI.DTO.Response;
using TaskManagerAPI.Exceptions;
using TaskManagerAPI.Helpers;
using TaskManagerAPI.Models;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Controllers
{
    [Authorize]
    [Route("api/v1/projects/{projectId}/tasks")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        public async Task<ActionResult<TaskDTO>> CreateTask(int projectId, [FromBody] TaskCreateDTO taskDTO)
        {
            var result = await _taskService.CreateTaskAsync(projectId, taskDTO);
            return Ok(result);
        }

        [HttpPatch("{taskId}")]
        public async Task<ActionResult<TaskDTO>> UpdateTask(int projectId, int taskId, [FromBody] JsonPatchDocument<ProjectTask> patch)
        {
            if (patch == null)
            {
                return BadRequest("Invalid patch document.");
            }

            try
            {
                await _taskService.UpdateTaskAsync(projectId, taskId, patch);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the task.");
            }
        }

        [HttpGet("{taskId}")]
        public async Task<ActionResult<TaskDTO>> GetTask(int projectId, int taskId)
        {
            var result = await _taskService.GetTaskAsync(projectId, taskId);
            return Ok(result);
        }

        [HttpPost("list")]
        public async Task<ActionResult<PaginatedList<TaskDTO>>> GetTasks(int projectId, [FromBody] TaskRequestDTO taskRequestDTO)
        {
            var result = await _taskService.GetTasksAsync(projectId, taskRequestDTO);
            return Ok(result);
        }

        [HttpPost("{taskId}/documents")]
        public async Task<IActionResult> AddTaskDocument(int projectId, int taskId, List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest(Captions.FileUploadError);
            }

            try
            {
                await _taskService.AddTaskDocumentAsync(projectId, taskId, files);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost("{taskId}/notes")]
        public async Task<IActionResult> AddTaskNote(int projectId, int taskId, [FromBody] TaskNoteDTO taskNoteDto)
        {
            var result = await _taskService.AddTaskNoteAsync(projectId, taskId, taskNoteDto);
            return Ok(result);
        }

        [HttpGet("{taskId}/notes")]
        public async Task<ActionResult<IEnumerable<TaskNoteResponseDTO>>> GetTaskNotes(int projectId, int taskId)
        {
            var result = await _taskService.GetTaskNotesAsync(projectId, taskId);
            return Ok(result);
        }

        [HttpGet("{taskId}/notes/{noteId}")]
        public async Task<ActionResult<TaskNoteResponseDTO>> GetTaskNote(int projectId, int taskId, int noteId)
        {
            var result = await _taskService.GetTaskNoteAsync(projectId, taskId, noteId);
            return Ok(result);
        }

        [HttpGet("{taskId}/documents")]
        public async Task<ActionResult<IEnumerable<TaskDocumentResponseDTO>>> GetTaskDocuments(int projectId, int taskId)
        {
            var result = await _taskService.GetTaskDocumentsAsync(projectId, taskId);
            return Ok(result);
        }

        [HttpGet("{taskId}/documents/{documentId}")]
        public async Task<ActionResult<TaskDocumentResponseDTO>> GetTaskDocument(int projectId, int taskId, int documentId)
        {
            var result = await _taskService.GetTaskDocumentAsync(projectId, taskId, documentId);
            return Ok(result);
        }
    }
}
