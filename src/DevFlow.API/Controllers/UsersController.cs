using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevFlow.Application.Interfaces;
using DevFlow.Application.DTOs.Users;
using System.Security.Claims;

namespace DevFlow.API.Controllers
{

    /// Controller for user profile operations
    /// All endpoints require authentication
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }


        /// GET /api/users/me
        /// Get current user's profile
        [HttpGet("me")]
        [ProducesResponseType(typeof(UserProfileDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetMyProfile()
        {
            int userId = GetCurrentUserId();

            var profile = await _userService.GetProfileAsync(userId);

            if (profile == null)
                return NotFound(new { message = "User not found" });

            return Ok(profile);
        }


        /// PUT /api/users/me
        /// Update current user's profile
        [HttpPut("me")]
        [ProducesResponseType(typeof(UserProfileDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileDto updateDto)
        {
            int userId = GetCurrentUserId();

            try
            {
                var profile = await _userService.UpdateProfileAsync(userId, updateDto);

                if (profile == null)
                    return NotFound(new { message = "User not found" });

                return Ok(profile);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }


        /// POST /api/users/me/change-password
        /// Change current user's password
        [HttpPost("me/change-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            int userId = GetCurrentUserId();

            try
            {
                bool success = await _userService.ChangePasswordAsync(userId, changePasswordDto);

                if (!success)
                    return NotFound(new { message = "User not found" });

                return Ok(new { message = "Password changed successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// GET /api/users/me/preferences
        /// Get current user's preferences
        /// </summary>
        [HttpGet("me/preferences")]
        public async Task<IActionResult> GetMyPreferences()
        {
            int userId = GetCurrentUserId();
            var preferences = await _userService.GetPreferencesAsync(userId);
            return Ok(preferences);
        }

        /// <summary>
        /// PUT /api/users/me/preferences
        /// Update user preferences
        /// </summary>
        [HttpPut("me/preferences")]
        public async Task<IActionResult> UpdateMyPreferences([FromBody] UpdatePreferencesDto updateDto)
        {
            int userId = GetCurrentUserId();

            try
            {
                var preferences = await _userService.UpdatePreferencesAsync(userId, updateDto);
                return Ok(preferences);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
        /// GET /api/users/{id}
        /// Get any user's public profile
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserProfileDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserProfile(int id)
        {
            var profile = await _userService.GetProfileAsync(id);

            if (profile == null)
                return NotFound(new { message = "User not found" });

            return Ok(profile);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim!.Value);
        }
    }
}