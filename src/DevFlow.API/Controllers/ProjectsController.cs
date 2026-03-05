using Microsoft.AspNetCore.Mvc;
using DevFlow.Application.Interfaces;
using DevFlow.Application.DTOs.Projects;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace DevFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProjectDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProject(int id)
        {
            int currentUserId = GetCurrentUserId();
            var project = await _projectService.GetProjectByIdAsync(id, currentUserId);

            if (project == null)
                return NotFound(new { message = "Project not found or access denied" });

            return Ok(project);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProjectListDto>), 200)]
        public async Task<IActionResult> GetMyProjects()
        {
            int currentUserId = GetCurrentUserId();
            var projects = await _projectService.GetUserProjectsAsync(currentUserId);
            return Ok(projects);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProjectDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto createDto)
        {
            int currentUserId = GetCurrentUserId();  // ✅ FIXED

            try
            {
                var project = await _projectService.CreateProjectAsync(createDto, currentUserId);

                return CreatedAtAction(
                    nameof(GetProject),
                    new { id = project.Id },
                    project
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProjectDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectDto updateDto)
        {
            int currentUserId = GetCurrentUserId();  // ✅ FIXED

            try
            {
                var project = await _projectService.UpdateProjectAsync(id, updateDto, currentUserId);

                if (project == null)
                    return NotFound(new { message = "Project not found or access denied" });

                return Ok(project);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteProject(int id)
        {
            int currentUserId = GetCurrentUserId();  // ✅ FIXED

            var deleted = await _projectService.DeleteProjectAsync(id, currentUserId);

            if (!deleted)
                return NotFound(new { message = "Project not found or access denied" });

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