using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevFlow.Application.Interfaces;
using DevFlow.Application.DTOs.Snippets;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace DevFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SnippetsController : ControllerBase
    {
        private readonly ISnippetService _snippetService;

        public SnippetsController(ISnippetService snippetService)
        {
            _snippetService = snippetService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMySnippets(
            [FromQuery] string? search,
            [FromQuery] string? language,
            [FromQuery] string? tags)
        {
            int userId = GetCurrentUserId();

            List<string>? tagList = null;
            if (!string.IsNullOrWhiteSpace(tags))
            {
                tagList = tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                             .Select(t => t.Trim())
                             .ToList();
            }

            var snippets = await _snippetService.GetUserSnippetsAsync(userId, search, language, tagList);
            return Ok(snippets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSnippet(int id)
        {
            int userId = GetCurrentUserId();
            var snippet = await _snippetService.GetSnippetByIdAsync(id, userId);

            if (snippet == null)
                return NotFound(new { message = "Snippet not found" });

            return Ok(snippet);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSnippet([FromBody] CreateSnippetDto createDto)
        {
            int userId = GetCurrentUserId();

            try
            {
                var snippet = await _snippetService.CreateSnippetAsync(createDto, userId);
                return CreatedAtAction(nameof(GetSnippet), new { id = snippet.Id }, snippet);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSnippet(int id, [FromBody] UpdateSnippetDto updateDto)
        {
            int userId = GetCurrentUserId();

            try
            {
                var snippet = await _snippetService.UpdateSnippetAsync(id, updateDto, userId);

                if (snippet == null)
                    return NotFound(new { message = "Snippet not found" });

                return Ok(snippet);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSnippet(int id)
        {
            int userId = GetCurrentUserId();
            var deleted = await _snippetService.DeleteSnippetAsync(id, userId);

            if (!deleted)
                return NotFound(new { message = "Snippet not found" });

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
    }
}