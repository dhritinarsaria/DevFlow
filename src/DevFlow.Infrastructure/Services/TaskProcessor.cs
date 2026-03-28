using DevFlow.Application.DTOs.Tasks;
using DevFlow.Application.Interfaces.Services;
using System.Threading.Tasks;

namespace DevFlow.Infrastructure.Services
{
    public class TaskProcessor
    {
        private readonly INotificationService _notificationService;

        public TaskProcessor(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task ProcessTask(TaskDto task)
        {
            // do some processing

            // Notify without knowing Hub
            await _notificationService.NotifyTaskCreatedAsync(task.ProjectId, task);
        }
    }
}