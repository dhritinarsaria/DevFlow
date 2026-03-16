using DevFlow.Application.DTOs.Users;

namespace DevFlow.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDto?> GetProfileAsync(int userId);
        Task<UserProfileDto?> UpdateProfileAsync(int userId, UpdateProfileDto updateDto);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
        Task<UserPreferencesDto> GetPreferencesAsync(int userId);
        Task<UserPreferencesDto> UpdatePreferencesAsync(int userId, UpdatePreferencesDto updateDto);
    }
}