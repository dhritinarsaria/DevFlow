using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevFlow.Application.Interfaces;
using DevFlow.Application.DTOs.Snippets;
using DevFlow.Domain.Entities;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace DevFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SnippetsController : ControllerBase
    {
        private readonly ISnippetRepository _snippetRepository;
        private readonly IUserRepository _userRepository;

        public SnippetsController(
            ISnippetRepository snippetRepository,
            IUserRepository userRepository)
        {
            _snippetRepository = snippetRepository;
            _userRepository = userRepository;
        }

       
        
        /// <summary>
        /// GET /api/snippets?search=regex&language=JavaScript
        /// Search and filter user's snippets
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMySnippets(
            [FromQuery] string? search,     // Optional search term
            [FromQuery] string? language)   // Optional language filter
        {
            int userId = GetCurrentUserId();

            IEnumerable<CodeSnippet> snippets;

            if (!string.IsNullOrWhiteSpace(search) || !string.IsNullOrWhiteSpace(language))
            {
                // Use search if filters provided
                snippets = await _snippetRepository.SearchAsync(userId, search, language);
            }
            else
            {
                // Get all snippets
                snippets = await _snippetRepository.GetByUserIdAsync(userId);
            }

            var snippetDtos = snippets.Select(s => new SnippetDto
            {
                Id = s.Id,
                Title = s.Title,
                Code = s.Code,
                Language = s.Language,
                Tags = !string.IsNullOrEmpty(s.Tags)
                    ? s.Tags.Split(',').ToList()
                    : new List<string>(),
                OwnerId = s.OwnerId,
                OwnerUsername = s.Owner?.Username ?? "Unknown",
                CreatedAt = s.CreatedAt
            });

            return Ok(snippetDtos);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSnippet(int id)
        {
            int userId = GetCurrentUserId();
            var snippet = await _snippetRepository.GetByIdAsync(id);

            if (snippet == null || snippet.OwnerId != userId)
                return NotFound(new { message = "Snippet not found" });

            var snippetDto = new SnippetDto
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

            return Ok(snippetDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSnippet([FromBody] CreateSnippetDto createDto)
        {
            int userId = GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(createDto.Title))
                return BadRequest(new { message = "Title is required" });

            if (string.IsNullOrWhiteSpace(createDto.Code))
                return BadRequest(new { message = "Code is required" });

            var user = await _userRepository.GetByIdAsync(userId);

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
            created.Owner = user!;

            var snippetDto = new SnippetDto
            {
                Id = created.Id,
                Title = created.Title,
                Code = created.Code,
                Language = created.Language,
                Tags = createDto.Tags ?? new List<string>(),
                OwnerId = created.OwnerId,
                OwnerUsername = user!.Username,
                CreatedAt = created.CreatedAt
            };

            return CreatedAtAction(nameof(GetSnippet), new { id = created.Id }, snippetDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSnippet(int id)
        {
            int userId = GetCurrentUserId();
            var snippet = await _snippetRepository.GetByIdAsync(id);

            if (snippet == null || snippet.OwnerId != userId)
                return NotFound(new { message = "Snippet not found" });

            await _snippetRepository.DeleteAsync(id);
            return NoContent();
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                           ?? User.FindFirst("sub")
                           ?? User.FindFirst(JwtRegisteredClaimNames.Sub);

            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User ID not found in token");

            return int.Parse(userIdClaim.Value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSnippet(int id, [FromBody] UpdateSnippetDto updateDto)
        {
            int userId = GetCurrentUserId();

            var snippet = await _snippetRepository.GetByIdAsync(id);

            if (snippet == null || snippet.OwnerId != userId)
                return NotFound(new { message = "Snippet not found" });

            if (string.IsNullOrWhiteSpace(updateDto.Title))
                return BadRequest(new { message = "Title is required" });

            if (string.IsNullOrWhiteSpace(updateDto.Code))
                return BadRequest(new { message = "Code is required" });

            // Update fields
            snippet.Title = updateDto.Title;
            snippet.Code = updateDto.Code;
            snippet.Language = updateDto.Language;
            snippet.Tags = updateDto.Tags != null && updateDto.Tags.Any()
                ? string.Join(",", updateDto.Tags)
                : string.Empty;

            await _snippetRepository.UpdateAsync(snippet);

            var snippetDto = new SnippetDto
            {
                Id = snippet.Id,
                Title = snippet.Title,
                Code = snippet.Code,
                Language = snippet.Language,
                Tags = updateDto.Tags ?? new List<string>(),
                OwnerId = snippet.OwnerId,
                OwnerUsername = snippet.Owner?.Username ?? "Unknown",
                CreatedAt = snippet.CreatedAt
            };

            return Ok(snippetDto);
        }

    }
}