using DevFlow.Application.DTOs.Snippets;

namespace DevFlow.Application.Interfaces
{
    public interface ISnippetService
    {
        Task<IEnumerable<SnippetDto>> GetUserSnippetsAsync(int userId, string? search, string? language, List<string>? tags);
        Task<SnippetDto?> GetSnippetByIdAsync(int snippetId, int userId);
        Task<SnippetDto> CreateSnippetAsync(CreateSnippetDto createDto, int userId);
        Task<SnippetDto?> UpdateSnippetAsync(int snippetId, UpdateSnippetDto updateDto, int userId);
        Task<bool> DeleteSnippetAsync(int snippetId, int userId);
    }
}