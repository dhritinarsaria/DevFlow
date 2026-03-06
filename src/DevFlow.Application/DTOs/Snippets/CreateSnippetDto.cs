namespace DevFlow.Application.DTOs.Snippets
{
    public class CreateSnippetDto
    {
        public string Title { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<string>? Tags { get; set; }
    }
}