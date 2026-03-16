using System.Text.Json;
using DevFlow.Application.DTOs.Users;
using DevFlow.Application.Interfaces;

namespace DevFlow.Application.Services
{

    /// Service for user profile operations
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;

        public UserService(
            IUserRepository userRepository,
            IProjectRepository projectRepository)
        {
            _userRepository = userRepository;
            _projectRepository = projectRepository;
        }


        /// Get user profile with statistics
        public async Task<UserProfileDto?> GetProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return null;

            // Get user's projects for statistics
            var projects = await _projectRepository.GetByUserIdAsync(userId);

            return new UserProfileDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                ProjectCount = projects.Count(),
                SnippetCount = 0  // TODO: Implement when we have snippets
            };
        }


        /// Update user profile (username only)
        /// Email cannot be changed for security
        public async Task<UserProfileDto?> UpdateProfileAsync(int userId, UpdateProfileDto updateDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return null;

            // Validation: Username required
            if (string.IsNullOrWhiteSpace(updateDto.Username))
                throw new ArgumentException("Username is required");

            // Validation: Username length
            if (updateDto.Username.Length < 3 || updateDto.Username.Length > 50)
                throw new ArgumentException("Username must be between 3 and 50 characters");

            // Check if new username is already taken (by different user)
            if (updateDto.Username != user.Username)
            {
                if (await _userRepository.UsernameExistsAsync(updateDto.Username))
                    throw new InvalidOperationException("Username already taken");
            }

            // Update username
            user.Username = updateDto.Username;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            // Return updated profile
            return await GetProfileAsync(userId);
        }


        /// Change user password
        /// Verifies current password before allowing change
        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return false;

            // Verify current password
            bool isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(
                changePasswordDto.CurrentPassword,
                user.PasswordHash);

            if (!isCurrentPasswordValid)
                throw new UnauthorizedAccessException("Current password is incorrect");

            // Validation: New password strength
            if (changePasswordDto.NewPassword.Length < 6)
                throw new ArgumentException("New password must be at least 6 characters");

            // Can't use same password
            if (BCrypt.Net.BCrypt.Verify(changePasswordDto.NewPassword, user.PasswordHash))
                throw new ArgumentException("New password must be different from current password");

            // Hash and update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword, workFactor: 11);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            return true;
        }



        // Implement in UserService
        public async Task<UserPreferencesDto> GetPreferencesAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            if (string.IsNullOrWhiteSpace(user.Preferences))
            {
                // Return default preferences
                return new UserPreferencesDto();
            }

            return JsonSerializer.Deserialize<UserPreferencesDto>(user.Preferences)
                   ?? new UserPreferencesDto();
        }

        public async Task<UserPreferencesDto> UpdatePreferencesAsync(int userId, UpdatePreferencesDto updateDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            // Get current preferences or defaults
            var preferences = string.IsNullOrWhiteSpace(user.Preferences)
                ? new UserPreferencesDto()
                : JsonSerializer.Deserialize<UserPreferencesDto>(user.Preferences)
                  ?? new UserPreferencesDto();

            // Update only provided fields
            if (updateDto.Theme != null) preferences.Theme = updateDto.Theme;
            if (updateDto.EmailNotifications.HasValue) preferences.EmailNotifications = updateDto.EmailNotifications.Value;
            if (updateDto.TaskReminders.HasValue) preferences.TaskReminders = updateDto.TaskReminders.Value;
            if (updateDto.DefaultProjectView != null) preferences.DefaultProjectView = updateDto.DefaultProjectView;

            // Save back to user
            user.Preferences = JsonSerializer.Serialize(preferences);
            await _userRepository.UpdateAsync(user);

            return preferences;
        }
    }
}