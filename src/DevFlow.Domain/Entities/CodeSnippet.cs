namespace DevFlow.Domain.Entities
{
    public class CodeSnippet
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty; // Stored as comma-separated
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public User User { get; set; } = null!;
    }
}