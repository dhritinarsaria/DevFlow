using DevFlow.Application.Interfaces;
using DevFlow.Application.DTOs.Analytics;

namespace DevFlow.Application.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IAnalyticsRepository _analyticsRepository;

        public AnalyticsService(IAnalyticsRepository analyticsRepository)
        {
            _analyticsRepository = analyticsRepository;
        }

        public async Task<UserAnalyticsDto> GetUserAnalyticsAsync(int userId)
        {
            return await _analyticsRepository.GetUserAnalyticsAsync(userId);
        }

        public async Task<ProjectAnalyticsDto?> GetProjectAnalyticsAsync(int projectId, int userId)
        {
            return await _analyticsRepository.GetProjectAnalyticsAsync(projectId, userId);
        }
    }
}