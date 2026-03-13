using DevFlow.Application.DTOs.Tasks;

namespace DevFlow.Application.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetProjectTasksAsync(int projectId, int userId);
        Task<TaskDto?> GetTaskByIdAsync(int taskId, int userId);
        Task<TaskDto> CreateTaskAsync(int projectId, CreateTaskDto createDto, int userId);
        Task<TaskDto?> UpdateTaskAsync(int projectId, int taskId, UpdateTaskDto updateDto, int userId);
        Task<bool> DeleteTaskAsync(int projectId, int taskId, int userId);
    }
}
