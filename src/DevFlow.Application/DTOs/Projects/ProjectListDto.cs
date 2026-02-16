namespace DevFlow.Application.DTOs.Projects
{

    /// DTO for returning project in a list
    /// Lighter than ProjectDto - only essential info for lists
    public class ProjectListDto
    {
       
        /// Project ID
        public int Id { get; set; }
        
 
        /// Project name
        public string Name { get; set; } = string.Empty;
        

        /// Short description (truncated if needed)
        public string? Description { get; set; }
        
     
        /// Number of tasks
        public int TaskCount { get; set; }
        
       
        /// When created
        public DateTime CreatedAt { get; set; }
    }
}

// Why separate from ProjectDto:

// Lighter payload (don't send tags, owner info for lists)
// Faster API responses (less data transferred)
// Frontend often needs different data for lists vs detail views