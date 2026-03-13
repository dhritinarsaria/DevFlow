using DevFlow.Application.Interfaces;
using DevFlow.Application.DTOs.Snippets;
using DevFlow.Domain.Entities;

namespace DevFlow.Application.Services
{
    public class SnippetService : ISnippetService
    {
        private readonly ISnippetRepository _snippetRepository;
        private readonly IUserRepository _userRepository;

        public SnippetService(
            ISnippetRepository snippetRepository,
            IUserRepository userRepository)
        {
            _snippetRepository = snippetRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<SnippetDto>> GetUserSnippetsAsync(
            int userId, 
            string? search, 
            string? language, 
            List<string>? tags)
        {
            IEnumerable<CodeSnippet> snippets;

            if (!string.IsNullOrWhiteSpace(search) || 
                !string.IsNullOrWhiteSpace(language) ||
                tags != null && tags.Any())
            {
                snippets = await _snippetRepository.SearchAsync(userId, search, language, tags);
            }
            else
            {
                snippets = await _snippetRepository.GetByUserIdAsync(userId);
            }

            return snippets.Select(MapToSnippetDto);
        }

        public async Task<SnippetDto?> GetSnippetByIdAsync(int snippetId, int userId)
        {
            var snippet = await _snippetRepository.GetByIdAsync(snippetId);
            if (snippet == null || snippet.OwnerId != userId)
                return null;

            return MapToSnippetDto(snippet);
        }

        public async Task<SnippetDto> CreateSnippetAsync(CreateSnippetDto createDto, int userId)
        {
            if (string.IsNullOrWhiteSpace(createDto.Title))
                throw new ArgumentException("Title is required");

            if (string.IsNullOrWhiteSpace(createDto.Code))
                throw new ArgumentException("Code is required");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            var snippet = new CodeSnippet
            {
                Title = createDto.Title,
                Code = createDto.Code,
                Language = createDto.Language,
                Tags = createDto.Tags != null && createDto.Tags.Any()
                    ? string.Join(",", createDto.Tags)
                    : string.Empty,
                OwnerId = userId
            };

            var created = await _snippetRepository.AddAsync(snippet);
            created.Owner = user;

            return MapToSnippetDto(created);
        }

        public async Task<SnippetDto?> UpdateSnippetAsync(int snippetId, UpdateSnippetDto updateDto, int userId)
        {
            var snippet = await _snippetRepository.GetByIdAsync(snippetId);
            if (snippet == null || snippet.OwnerId != userId)
                return null;

            if (string.IsNullOrWhiteSpace(updateDto.Title))
                throw new ArgumentException("Title is required");

            if (string.IsNullOrWhiteSpace(updateDto.Code))
                throw new ArgumentException("Code is required");

            snippet.Title = updateDto.Title;
            snippet.Code = updateDto.Code;
            snippet.Language = updateDto.Language;
            snippet.Tags = updateDto.Tags != null && updateDto.Tags.Any()
                ? string.Join(",", updateDto.Tags)
                : string.Empty;

            await _snippetRepository.UpdateAsync(snippet);
            return MapToSnippetDto(snippet);
        }

        public async Task<bool> DeleteSnippetAsync(int snippetId, int userId)
        {
            var snippet = await _snippetRepository.GetByIdAsync(snippetId);
            if (snippet == null || snippet.OwnerId != userId)
                return false;

            await _snippetRepository.DeleteAsync(snippetId);
            return true;
        }

        private SnippetDto MapToSnippetDto(CodeSnippet snippet)
        {
            return new SnippetDto
            {
                Id = snippet.Id,
                Title = snippet.Title,
                Code = snippet.Code,
                Language = snippet.Language,
                Tags = !string.IsNullOrEmpty(snippet.Tags)
                    ? snippet.Tags.Split(',').ToList()
                    : new List<string>(),
                OwnerId = snippet.OwnerId,
                OwnerUsername = snippet.Owner?.Username ?? "Unknown",
                CreatedAt = snippet.CreatedAt
            };
        }
    }
}