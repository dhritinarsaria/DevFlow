using DevFlow.Application.DTOs.Projects;

namespace DevFlow.Application.Interfaces
{
  
    /// Service interface for project business logic
    /// Defines all operations that can be performed on projects
    public interface IProjectService
    {
        
        /// /// Get single project by ID with full details
        /// </summary>
        /// <param name="id">Project ID</param>
        /// <param name="userId">Current user ID (for permission check)</param>
        Task<ProjectDto?> GetProjectByIdAsync(int id, int userId);
        
     
        /// Get all projects for a specific user
      
        /// <param name="userId">User ID</param>
        /// <returns>List of user's projects</returns>
        Task<IEnumerable<ProjectListDto>> GetUserProjectsAsync(int userId);
        
       
        /// Create a new project
        /// <param name="createDto">Project creation data</param>
        /// <param name="userId">Owner user ID</param>
        /// <returns>Created project details</returns>
        Task<ProjectDto> CreateProjectAsync(CreateProjectDto createDto, int userId);
        
        
        /// Update existing project
        
        /// <param name="id">Project ID</param>
        /// <param name="updateDto">Updated data</param>
        /// <param name="userId">Current user ID (for permission check)</param>
        /// <returns>Updated project or null if not found/no permission</returns>
        Task<ProjectDto?> UpdateProjectAsync(int id, UpdateProjectDto updateDto, int userId);
        
       
        /// Delete a project
        
        /// <param name="id">Project ID</param>
        /// <param name="userId">Current user ID (for permission check)</param>
        /// <returns>True if deleted, false if not found/no permission</returns>
        Task<bool> DeleteProjectAsync(int id, int userId);
    }
}