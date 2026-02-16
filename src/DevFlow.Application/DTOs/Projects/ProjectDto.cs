namespace DevFlow.Application.DTOs.Projects
{
   
    /// DTO for returning full project details
    /// Contains all data client needs to display a project
    public class ProjectDto
    {
   
        /// Project ID
        public int Id { get; set; }
        
   
        /// Project name
        public string Name { get; set; } = string.Empty;
        
       
        /// Project description
        public string? Description { get; set; }
        
        
        /// Owner's username (not full User object - security!)
        public string OwnerUsername { get; set; } = string.Empty;
        
       
        /// Owner's ID (useful for permission checks in frontend)
        public int OwnerId { get; set; }
        
     
        /// Tags as list (converted from comma-separated in database)
        public List<string> Tags { get; set; } = new List<string>();
        
  
        /// Number of tasks in project (computed field!)
        public int TaskCount { get; set; }
        
      
        /// When project was created
        public DateTime CreatedAt { get; set; }
        
    
        /// When project was last updated (null if never updated)
        public DateTime? UpdatedAt { get; set; }
    }
}