namespace DevFlow.Application.DTOs.Projects
{
    /// <summary>
    /// DTO for returning full project details
    /// Contains all data client needs to display a project
    /// </summary>
    public class ProjectDto
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
        /// Project description
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Owner's username (not full User object - security!)
        /// </summary>
        public string OwnerUsername { get; set; } = string.Empty;
        
        /// <summary>
        /// Owner's ID (useful for permission checks in frontend)
        /// </summary>
        public int OwnerId { get; set; }
        
        /// <summary>
        /// Tags as list (converted from comma-separated in database)
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();
        
        /// <summary>
        /// Number of tasks in project (computed field!)
        /// </summary>
        public int TaskCount { get; set; }
        
        /// <summary>
        /// When project was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// When project was last updated (null if never updated)
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}