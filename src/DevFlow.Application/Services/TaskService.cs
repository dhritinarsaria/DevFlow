using DevFlow.Application.Interfaces;
using DevFlow.Application.DTOs.Tasks;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enums;
using DevFlow.Application.Interfaces.Services;


namespace DevFlow.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly INotificationService _notificationService;
        
        public TaskService(
            ITaskRepository taskRepository,
            IProjectRepository projectRepository,INotificationService notificationService) 
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<TaskDto>> GetProjectTasksAsync(int projectId, int userId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null || project.OwnerId != userId)
                return Enumerable.Empty<TaskDto>();

            var tasks = await _taskRepository.GetByProjectIdAsync(projectId);
            return tasks.Select(t => MapToTaskDto(t, project.Name));
        }

        public async Task<TaskDto?> GetTaskByIdAsync(int taskId, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
                return null;

            var project = await _projectRepository.GetByIdAsync(task.ProjectId);
            if (project == null || project.OwnerId != userId)
                return null;

            return MapToTaskDto(task, project.Name);
        }

        public async Task<TaskDto> CreateTaskAsync(int projectId, CreateTaskDto createDto, int userId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null || project.OwnerId != userId)
                throw new UnauthorizedAccessException("Project not found");

            if (string.IsNullOrWhiteSpace(createDto.Title))
                throw new ArgumentException("Title is required");

            if (createDto.Priority < 1 || createDto.Priority > 4)
                throw new ArgumentException("Priority must be 1-4");

            var task = new ProjectTask
            {
                Title = createDto.Title,
                Description = createDto.Description,
                ProjectId = projectId,
                Status = ProjectTaskStatus.Todo,
                Priority = (TaskPriority)createDto.Priority,
                DueDate = createDto.DueDate
            };

            var createdTask = await _taskRepository.AddAsync(task);
             var taskDto = MapToTaskDto(createdTask, project.Name);
             await _notificationService.NotifyTaskCreatedAsync(projectId, taskDto);
            return MapToTaskDto(createdTask, project.Name);
        }

        public async Task<TaskDto?> UpdateTaskAsync(int projectId, int taskId, UpdateTaskDto updateDto, int userId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null || project.OwnerId != userId)
                return null;

            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null || task.ProjectId != projectId)
                return null;

            if (string.IsNullOrWhiteSpace(updateDto.Title))
                throw new ArgumentException("Title is required");

            task.Title = updateDto.Title;
            task.Description = updateDto.Description;
            task.Status = (ProjectTaskStatus)updateDto.Status;
            task.Priority = (TaskPriority)updateDto.Priority;
            task.DueDate = updateDto.DueDate;

            await _taskRepository.UpdateAsync(task);
             var taskDto = MapToTaskDto(task, project.Name);
    
    await _notificationService.NotifyTaskUpdatedAsync(projectId, taskDto);
            return MapToTaskDto(task, project.Name);
        }

        public async Task<bool> DeleteTaskAsync(int projectId, int taskId, int userId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null || project.OwnerId != userId)
                return false;

            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null || task.ProjectId != projectId)
                return false;

            await _taskRepository.DeleteAsync(taskId);
             await _notificationService.NotifyTaskDeletedAsync(projectId, taskId); 
            return true;
        }

        private TaskDto MapToTaskDto(ProjectTask task, string projectName)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                DueDate = task.DueDate,
                ProjectId = task.ProjectId,
                ProjectName = projectName,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };
        }
    }
}