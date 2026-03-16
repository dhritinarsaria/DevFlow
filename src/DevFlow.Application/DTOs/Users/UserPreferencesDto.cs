namespace DevFlow.Application.DTOs.Users
{
    public class UserPreferencesDto
    {
        public string Theme { get; set; } = "light"; // light, dark
        public bool EmailNotifications { get; set; } = true;
        public bool TaskReminders { get; set; } = true;
        public string DefaultProjectView { get; set; } = "grid"; // grid, list
    }
    
    public class UpdatePreferencesDto
    {
        public string? Theme { get; set; }
        public bool? EmailNotifications { get; set; }
        public bool? TaskReminders { get; set; }
        public string? DefaultProjectView { get; set; }
    }
}