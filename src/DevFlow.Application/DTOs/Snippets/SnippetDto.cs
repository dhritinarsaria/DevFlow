namespace DevFlow.Application.DTOs.Snippets
{
    public class SnippetDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public int OwnerId { get; set; }
        public string OwnerUsername { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}