using DevFlow.Application.DTOs.Analytics;

namespace DevFlow.Application.Interfaces
{
    public interface IAnalyticsService
    {
        Task<UserAnalyticsDto> GetUserAnalyticsAsync(int userId);
        Task<ProjectAnalyticsDto?> GetProjectAnalyticsAsync(int projectId, int userId);
    }
}