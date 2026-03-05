using DevFlow.Domain.Entities;

namespace DevFlow.Application.Interfaces
{
    public interface ITaskRepository
    {
        Task<ProjectTask?> GetByIdAsync(int id);
        Task<IEnumerable<ProjectTask>> GetByProjectIdAsync(int projectId);
        Task<ProjectTask> AddAsync(ProjectTask task);
        Task UpdateAsync(ProjectTask task);
        Task DeleteAsync(int id);
    }
}