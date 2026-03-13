using DevFlow.Domain.Entities;

namespace DevFlow.Application.Interfaces
{
    public interface ISnippetRepository
    {
        Task<CodeSnippet?> GetByIdAsync(int id);
        Task<IEnumerable<CodeSnippet>> GetByUserIdAsync(int userId);
        Task<CodeSnippet> AddAsync(CodeSnippet snippet);
        Task UpdateAsync(CodeSnippet snippet);
        Task DeleteAsync(int id);
        Task<IEnumerable<CodeSnippet>> SearchAsync(int userId, string? searchTerm, string? language,
    List<string>? tags);
    }
}