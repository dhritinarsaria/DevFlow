namespace DevFlow.Application.DTOs.Projects
{
   
    /// DTO for creating a new project
    /// Contains only fields the client can provide
    public class CreateProjectDto
    {
  
        /// Project name (required, max 200 chars)
        public string Name { get; set; } = string.Empty;
        

        /// Project description (optional, max 1000 chars)
        public string? Description { get; set; }
        

        /// Tags for categorization (optional)
        /// Example: ["backend", "api", "dotnet"]
        public List<string>? Tags { get; set; }
    }
}