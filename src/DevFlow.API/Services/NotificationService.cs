using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using DevFlow.Application.DTOs.Tasks;
using DevFlow.Application.Interfaces.Services;
using DevFlow.API.Hubs;
using System;
using System.Threading.Tasks;

namespace DevFlow.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<TaskHub> _hubContext;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IHubContext<TaskHub> hubContext,
                                   ILogger<NotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task NotifyTaskCreatedAsync(int projectId, TaskDto task)
        {
            try
            {
                await _hubContext.Clients.Group($"project_{projectId}")
                    .SendAsync("TaskCreated", task);
                _logger.LogInformation("TaskCreated notification sent for task {TaskId}", task.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send TaskCreated notification for task {TaskId}", task.Id);
            }
        }

        public async Task NotifyTaskUpdatedAsync(int projectId, TaskDto task)
        {
            try
            {
                await _hubContext.Clients.Group($"project_{projectId}")
                    .SendAsync("TaskUpdated", task);
                _logger.LogInformation("TaskUpdated notification sent for task {TaskId}", task.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send TaskUpdated notification for task {TaskId}", task.Id);
            }
        }

        public async Task NotifyTaskDeletedAsync(int projectId, int taskId)
        {
            try
            {
                await _hubContext.Clients.Group($"project_{projectId}")
                    .SendAsync("TaskDeleted", taskId);
                _logger.LogInformation("TaskDeleted notification sent for task {TaskId}", taskId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send TaskDeleted notification for task {TaskId}", taskId);
            }
        }
    }
}