namespace DevFlow.Application.DTOs.Projects
{
    /// <summary>
    /// DTO for updating an existing project
    /// Contains only fields that can be modified
    /// </summary>
    public class UpdateProjectDto
    {
        /// <summary>
        /// Updated project name
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Updated description
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Updated tags
        /// </summary>
        public List<string>? Tags { get; set; }
    }
}