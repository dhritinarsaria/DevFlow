using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
//using DevFlow.Application.Interfaces.Repositories;
using DevFlow.Application.DTOs.Analytics;
using DevFlow.Infrastructure.Data;
using DevFlow.Domain.Enums;
using DevFlow.Application.Interfaces;

namespace DevFlow.Infrastructure.Repositories
{
    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly DevFlowDbContext _context;
        private readonly IMemoryCache _cache;

        public AnalyticsRepository(DevFlowDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<UserAnalyticsDto> GetUserAnalyticsAsync(int userId)
        {
            // Try to get from cache
            string cacheKey = $"analytics:user:{userId}";
            
            if (_cache.TryGetValue(cacheKey, out UserAnalyticsDto? cachedAnalytics))
            {
                return cachedAnalytics!;
            }

            // Cache miss - compute analytics
            var projects = await _context.Projects
                .Include(p => p.Tasks)
                .Where(p => p.OwnerId == userId)
                .ToListAsync();

            var projectAnalytics = projects.Select(p => new ProjectAnalyticsDto
            {
                ProjectId = p.Id,
                ProjectName = p.Name,
                TotalTasks = p.Tasks.Count,
                CompletedTasks = p.Tasks.Count(t => t.Status == ProjectTaskStatus.Done),
                InProgressTasks = p.Tasks.Count(t => t.Status == ProjectTaskStatus.InProgress),
                TodoTasks = p.Tasks.Count(t => t.Status == ProjectTaskStatus.Todo),
                CompletionPercentage = p.Tasks.Any()
                    ? Math.Round((double)p.Tasks.Count(t => t.Status == ProjectTaskStatus.Done) / p.Tasks.Count * 100, 2)
                    : 0,
                OldestTaskDate = p.Tasks.Any() ? p.Tasks.Min(t => t.CreatedAt) : null,
                NewestTaskDate = p.Tasks.Any() ? p.Tasks.Max(t => t.CreatedAt) : null
            }).ToList();

            var totalTasks = projectAnalytics.Sum(p => p.TotalTasks);
            var completedTasks = projectAnalytics.Sum(p => p.CompletedTasks);

            var analytics = new UserAnalyticsDto
            {
                TotalProjects = projects.Count,
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                OverallCompletionRate = totalTasks > 0
                    ? Math.Round((double)completedTasks / totalTasks * 100, 2)
                    : 0,
                ProjectBreakdown = projectAnalytics
            };

            // Store in cache (5 minutes)
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
            _cache.Set(cacheKey, analytics, cacheOptions);

            return analytics;
        }

        public async Task<ProjectAnalyticsDto?> GetProjectAnalyticsAsync(int projectId, int userId)
        {
            // Try to get from cache
            string cacheKey = $"analytics:project:{projectId}";
            
            if (_cache.TryGetValue(cacheKey, out ProjectAnalyticsDto? cachedAnalytics))
            {
                return cachedAnalytics!;
            }

            // Cache miss - compute analytics
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == projectId && p.OwnerId == userId);

            if (project == null)
                return null;

            var analytics = new ProjectAnalyticsDto
            {
                ProjectId = project.Id,
                ProjectName = project.Name,
                TotalTasks = project.Tasks.Count,
                CompletedTasks = project.Tasks.Count(t => t.Status == ProjectTaskStatus.Done),
                InProgressTasks = project.Tasks.Count(t => t.Status == ProjectTaskStatus.InProgress),
                TodoTasks = project.Tasks.Count(t => t.Status == ProjectTaskStatus.Todo),
                CompletionPercentage = project.Tasks.Any()
                    ? Math.Round((double)project.Tasks.Count(t => t.Status == ProjectTaskStatus.Done) / project.Tasks.Count * 100, 2)
                    : 0,
                OldestTaskDate = project.Tasks.Any() ? project.Tasks.Min(t => t.CreatedAt) : null,
                NewestTaskDate = project.Tasks.Any() ? project.Tasks.Max(t => t.CreatedAt) : null
            };

            // Store in cache (5 minutes)
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
            _cache.Set(cacheKey, analytics, cacheOptions);

            return analytics;
        }
    }
}