namespace DevFlow.Application.DTOs.Projects
{
    /// <summary>
    /// DTO for returning project in a list
    /// Lighter than ProjectDto - only essential info for lists
    /// </summary>
    public class ProjectListDto
    {
        /// <summary>
        /// Project ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Project name
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Short description (truncated if needed)
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Number of tasks
        /// </summary>
        public int TaskCount { get; set; }
        
        /// <summary>
        /// When created
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}


