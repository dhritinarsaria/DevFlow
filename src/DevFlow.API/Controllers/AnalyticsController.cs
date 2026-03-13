using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevFlow.Application.Interfaces;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace DevFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserAnalytics()
        {
            int userId = GetCurrentUserId();
            var analytics = await _analyticsService.GetUserAnalyticsAsync(userId);
            return Ok(analytics);
        }

        [HttpGet("projects/{projectId}")]
        public async Task<IActionResult> GetProjectAnalytics(int projectId)
        {
            int userId = GetCurrentUserId();
            var analytics = await _analyticsService.GetProjectAnalyticsAsync(projectId, userId);

            if (analytics == null)
                return NotFound(new { message = "Project not found" });

            return Ok(analytics);
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