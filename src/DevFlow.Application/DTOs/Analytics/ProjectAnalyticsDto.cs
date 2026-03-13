namespace DevFlow.Application.DTOs.Analytics
{
    public class ProjectAnalyticsDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int TodoTasks { get; set; }
        public double CompletionPercentage { get; set; }
        public DateTime? OldestTaskDate { get; set; }
        public DateTime? NewestTaskDate { get; set; }
    }
    
    public class UserAnalyticsDto
    {
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public double OverallCompletionRate { get; set; }
        public List<ProjectAnalyticsDto> ProjectBreakdown { get; set; } = new();
    }
}