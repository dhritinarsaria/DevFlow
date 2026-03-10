namespace DevFlow.Application.DTOs.Snippets
{
    public class UpdateSnippetDto
    {
        public string Title { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public List<string>? Tags { get; set; }
    }
}