using Microsoft.AspNetCore.Mvc;
using DevFlow.Application.Interfaces;
using DevFlow.Application.DTOs.Projects;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

// [ApiController] - Enables automatic model validation and binding
// [Route] - Defines URL pattern
// [HttpGet/Post/Put/Delete] - HTTP method for each action
// IActionResult - Can return different status codes (200, 201, 404, 400)
// [FromBody] - Read JSON from request body
// Try-catch - Handle service exceptions, return appropriate HTTP status

namespace DevFlow.API.Controllers
{
    
    /// API Controller for project operations
    /// Handles HTTP requests and returns appropriate responses
 
    [ApiController]  // Marks this as an API controller (enables automatic validation, model binding)
    [Route("api/[controller]")]  // URL: /api/projects
    [Authorize] 
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

  
        /// Constructor - DI injects ProjectService
        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

       
        /// GET /api/projects/{id}
        /// Get a single project by ID
        /// <returns>Project details or 404 if not found</returns>
        [HttpGet("{id}")]  // Matches GET /api/projects/5
        [ProducesResponseType(typeof(ProjectDto), 200)]  // Documents return type for Swagger
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProject(int id)
        {
            // For now, we'll hardcode userId = 1 (authentication comes later)
            // In real app, this comes from JWT token
            int currentUserId = GetCurrentUserId();;
            
            // Call service layer
            var project = await _projectService.GetProjectByIdAsync(id, currentUserId);
            
            // Service returns null if not found or no permission
            if (project == null)
                return NotFound(new { message = "Project not found or access denied" });
            
            // Return 200 OK with project data
            return Ok(project);
        }

     
        /// GET /api/projects
        /// Get all projects for current user
    
        /// <returns>List of user's projects</returns>
        [HttpGet]  // Matches GET /api/projects
        [ProducesResponseType(typeof(IEnumerable<ProjectListDto>), 200)]
        public async Task<IActionResult> GetMyProjects()
        {
            // Hardcoded userId for now
            int currentUserId = 1;
            
            var projects = await _projectService.GetUserProjectsAsync(currentUserId);
            
            return Ok(projects);
        }

     
        /// POST /api/projects
        /// Create a new project
        /// <returns>Created project with 201 status</returns>
        [HttpPost]  // Matches POST /api/projects
        [ProducesResponseType(typeof(ProjectDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto createDto)
        {
            // [FromBody] tells ASP.NET to read data from request body JSON
            
            // Hardcoded userId for now
            int currentUserId = 1;
            
            try
            {
                // Call service to create project
                var project = await _projectService.CreateProjectAsync(createDto, currentUserId);
                
                // Return 201 Created with Location header
                // Location: /api/projects/15 (where 15 is the new project ID)
                return CreatedAtAction(
                    nameof(GetProject),  // Action name for Location header
                    new { id = project.Id },  // Route values
                    project  // Response body
                );
            }
            catch (ArgumentException ex)
            {
                // Validation errors (name required, too long, etc.)
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Business rule violations (max 50 projects, etc.)
                return BadRequest(new { message = ex.Message });
            }
        }

      
        /// PUT /api/projects/{id}
        /// Update an existing project
        /// <param name="updateDto">Updated project data</param>
        /// <returns>Updated project or 404/403</returns>
        [HttpPut("{id}")]  // Matches PUT /api/projects/5
        [ProducesResponseType(typeof(ProjectDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectDto updateDto)
        {
            int currentUserId = 1;
            
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

       
        /// DELETE /api/projects/{id}
        /// Delete a project
        /// <param name="id">Project ID to delete</param>
        /// <returns>204 No Content if successful, 404 if not found</returns>
        [HttpDelete("{id}")]  // Matches DELETE /api/projects/5
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteProject(int id)
        {
            int currentUserId = 1;
            
            var deleted = await _projectService.DeleteProjectAsync(id, currentUserId);
            
            if (!deleted)
                return NotFound(new { message = "Project not found or access denied" });
            
            // 204 No Content - successful deletion, no response body
            return NoContent();
        }

// ClaimTypes.NameIdentifier = User ID inside JWT
// This method:
// ✔ Extracts user ID
// ✔ Converts it to int
// ✔ Uses it for authorization logic
        private int GetCurrentUserId()
{
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
    return int.Parse(userIdClaim!.Value);
}

    }
}