using Microsoft.EntityFrameworkCore;
using DevFlow.Application.Interfaces;
using DevFlow.Domain.Entities;
using DevFlow.Infrastructure.Data;

namespace DevFlow.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly DevFlowDbContext _context;

        public TaskRepository(DevFlowDbContext context)
        {
            _context = context;
        }

        public async Task<ProjectTask?> GetByIdAsync(int id)
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<ProjectTask>> GetByProjectIdAsync(int projectId)
        {
            return await _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .OrderBy(t => t.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ProjectTask> AddAsync(ProjectTask task)
        {
            task.CreatedAt = DateTime.UtcNow;
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task UpdateAsync(ProjectTask task)
        {
            task.UpdatedAt = DateTime.UtcNow;
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }
    }
}