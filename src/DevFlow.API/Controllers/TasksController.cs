using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevFlow.Application.Interfaces;
using DevFlow.Application.DTOs.Tasks;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enums;
using System.Security.Claims;

namespace DevFlow.API.Controllers
{
    [ApiController]
    [Route("api/projects/{projectId}/tasks")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;

        public TasksController(
            ITaskRepository taskRepository,
            IProjectRepository projectRepository)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
        }

        /// <summary>
        /// GET /api/projects/{projectId}/tasks
        /// Get all tasks for a project
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProjectTasks(int projectId)
        {
            int userId = GetCurrentUserId();

            // Verify user owns project
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null || project.OwnerId != userId)
                return NotFound(new { message = "Project not found" });

            var tasks = await _taskRepository.GetByProjectIdAsync(projectId);

            var taskDtos = tasks.Select(t => MapToTaskDto(t, project.Name));
            return Ok(taskDtos);
        }

        /// <summary>
        /// POST /api/projects/{projectId}/tasks
        /// Create a new task
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateTask(int projectId, [FromBody] CreateTaskDto createDto)
        {
            int userId = GetCurrentUserId();

            // Verify user owns project
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null || project.OwnerId != userId)
                return NotFound(new { message = "Project not found" });

            // Validate
            if (string.IsNullOrWhiteSpace(createDto.Title))
                return BadRequest(new { message = "Title is required" });

            if (createDto.Priority < 1 || createDto.Priority > 4)
                return BadRequest(new { message = "Priority must be 1-4" });

            var task = new ProjectTask
            {
                Title = createDto.Title,
                Description = createDto.Description,
                ProjectId = projectId,
                Status = ProjectTaskStatus.Todo,
                Priority = (TaskPriority)createDto.Priority,
                DueDate = createDto.DueDate
            };

            var createdTask = await _taskRepository.AddAsync(task);
            createdTask.Project = project;

            return CreatedAtAction(
                nameof(GetProjectTasks),
                new { projectId },
                MapToTaskDto(createdTask, project.Name));
        }

        /// <summary>
        /// PUT /api/projects/{projectId}/tasks/{id}
        /// Update a task
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(
            int projectId,
            int id,
            [FromBody] UpdateTaskDto updateDto)
        {
            int userId = GetCurrentUserId();

            // Verify user owns project
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null || project.OwnerId != userId)
                return NotFound(new { message = "Project not found" });

            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null || task.ProjectId != projectId)
                return NotFound(new { message = "Task not found" });

            // Validate
            if (string.IsNullOrWhiteSpace(updateDto.Title))
                return BadRequest(new { message = "Title is required" });

            // Update
            task.Title = updateDto.Title;
            task.Description = updateDto.Description;
            task.Status = (ProjectTaskStatus)updateDto.Status;
            task.Priority = (TaskPriority)updateDto.Priority;
            task.DueDate = updateDto.DueDate;

            await _taskRepository.UpdateAsync(task);

            task.Project = project;
            return Ok(MapToTaskDto(task, project.Name));
        }

        /// <summary>
        /// DELETE /api/projects/{projectId}/tasks/{id}
        /// Delete a task
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int projectId, int id)
        {
            int userId = GetCurrentUserId();

            // Verify user owns project
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null || project.OwnerId != userId)
                return NotFound(new { message = "Project not found" });

            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null || task.ProjectId != projectId)
                return NotFound(new { message = "Task not found" });

            await _taskRepository.DeleteAsync(id);

            return NoContent();
        }

        private TaskDto MapToTaskDto(ProjectTask task, string projectName)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                DueDate = task.DueDate,
                ProjectId = task.ProjectId,
                ProjectName = projectName,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim!.Value);
        }
    }
}