namespace DevFlow.Application.DTOs.Users
{
    /// <summary>
    /// DTO for updating user profile
    /// Only allows changing username (email is immutable for security)
    /// </summary>
    public class UpdateProfileDto
    {
        public string Username { get; set; } = string.Empty;
    }
}