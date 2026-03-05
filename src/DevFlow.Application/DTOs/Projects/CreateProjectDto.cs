namespace DevFlow.Application.DTOs.Projects
{
    /// <summary>
    /// DTO for creating a new project
    /// Contains only fields the client can provide
    /// </summary>
    public class CreateProjectDto
    {
        /// <summary>
        /// Project name (required, max 200 chars)
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Project description (optional, max 1000 chars)
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Tags for categorization (optional)
        /// Example: ["backend", "api", "dotnet"]
        /// </summary>
        public List<string>? Tags { get; set; }
    }
}