namespace DevFlow.Application.DTOs.Projects
{
    
    /// DTO for updating an existing project
    /// Contains only fields that can be modified
    public class UpdateProjectDto
    {
        /// Updated project name
        public string Name { get; set; } = string.Empty;
        
   
        /// Updated description
        public string? Description { get; set; }
        
        
        /// Updated tags
        public List<string>? Tags { get; set; }
    }
}