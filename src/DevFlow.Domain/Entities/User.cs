using DevFlow.Domain.Enums;

namespace DevFlow.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public UserRole Role { get; set; }
        
        // Navigation properties
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<CodeSnippet> CodeSnippets { get; set; } = new List<CodeSnippet>();
    }
}