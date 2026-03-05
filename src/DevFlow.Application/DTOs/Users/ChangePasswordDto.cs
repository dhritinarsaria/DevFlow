namespace DevFlow.Application.DTOs.Users
{
    /// <summary>
    /// DTO for changing password
    /// Requires old password for verification
    /// </summary>
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}