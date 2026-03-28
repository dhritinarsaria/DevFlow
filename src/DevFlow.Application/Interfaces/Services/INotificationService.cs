using DevFlow.Application.DTOs.Tasks;

namespace DevFlow.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task NotifyTaskCreatedAsync(int projectId, TaskDto task);
        Task NotifyTaskUpdatedAsync(int projectId, TaskDto task);
        Task NotifyTaskDeletedAsync(int projectId, int taskId);
    }
}