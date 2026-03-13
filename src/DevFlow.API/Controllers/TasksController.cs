using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevFlow.Application.Interfaces;
using DevFlow.Application.DTOs.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace DevFlow.API.Controllers
{
    [ApiController]
    [Route("api/projects/{projectId}/tasks")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectTasks(int projectId)
        {
            int userId = GetCurrentUserId();
            var tasks = await _taskService.GetProjectTasksAsync(projectId, userId);
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int projectId, int id)
        {
            int userId = GetCurrentUserId();
            var task = await _taskService.GetTaskByIdAsync(id, userId);

            if (task == null)
                return NotFound(new { message = "Task not found" });

            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(int projectId, [FromBody] CreateTaskDto createDto)
        {
            int userId = GetCurrentUserId();

            try
            {
                var task = await _taskService.CreateTaskAsync(projectId, createDto, userId);
                return CreatedAtAction(nameof(GetTask), new { projectId, id = task.Id }, task);
            }
            catch (UnauthorizedAccessException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int projectId, int id, [FromBody] UpdateTaskDto updateDto)
        {
            int userId = GetCurrentUserId();

            try
            {
                var task = await _taskService.UpdateTaskAsync(projectId, id, updateDto, userId);

                if (task == null)
                    return NotFound(new { message = "Task not found" });

                return Ok(task);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int projectId, int id)
        {
            int userId = GetCurrentUserId();
            var deleted = await _taskService.DeleteTaskAsync(projectId, id, userId);

            if (!deleted)
                return NotFound(new { message = "Task not found" });

            return NoContent();
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                           ?? User.FindFirst("sub")
                           ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User ID not found in token");

            return int.Parse(userIdClaim.Value);
        }
    }
}