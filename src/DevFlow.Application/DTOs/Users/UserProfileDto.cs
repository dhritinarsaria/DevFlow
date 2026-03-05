namespace DevFlow.Application.DTOs.Users
{
    /// <summary>
    /// DTO for user profile information
    /// Contains public info that can be shared
    /// </summary>
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        
        // Statistics
        public int ProjectCount { get; set; }
        public int SnippetCount { get; set; }
    }
}