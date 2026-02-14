using DevFlow.Domain.Entities;

namespace DevFlow.Application.Interfaces
{
    /// <summary>
    /// Repository interface for Project entity
    /// Defines all data access operations for projects
    /// Controllers depend on this interface, not the implementation
    /// </summary>
    public interface IProjectRepository
    {

        /// Get project by ID with related entities (Owner, Tasks)
    
        /// <param name="id">Project ID</param>
        /// <returns>Project if found, null otherwise</returns>
        Task<Project?> GetByIdAsync(int id);
     

        /// Get all projects for a specific user
     
        /// <param name="userId">Owner's user ID</param>
        /// <returns>List of projects owned by the user</returns>
        Task<IEnumerable<Project>> GetByUserIdAsync(int userId);
        
    
        /// Get all projects (used for admin views)

        /// <returns>All projects in the system</returns>
        Task<IEnumerable<Project>> GetAllAsync();
        
     
        /// Add a new project to database
   
        /// <param name="project">Project entity to add</param>
        /// <returns>The added project with generated ID</returns>
        Task<Project> AddAsync(Project project);
        

        /// Update an existing project
      
        /// <param name="project">Project with updated values</param>
        Task UpdateAsync(Project project);
        
 
        /// Delete a project by ID
        /// Cascades to delete all tasks in the project
      
        /// <param name="id">Project ID to delete</param>
        Task DeleteAsync(int id);
        

        /// Check if a project exists
        /// Useful for validation before operations
        /// <param name="id">Project ID</param>
        /// <returns>True if exists, false otherwise</returns>
        Task<bool> ExistsAsync(int id);
    }
}