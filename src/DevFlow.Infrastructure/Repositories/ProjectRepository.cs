using Microsoft.EntityFrameworkCore;
using DevFlow.Application.Interfaces;
using DevFlow.Domain.Entities;
using DevFlow.Infrastructure.Data;

namespace DevFlow.Infrastructure.Repositories
{

    /// Implementation of IProjectRepository
    /// All database operations for Project entity
    /// Uses Entity Framework Core for data access
    public class ProjectRepository : IProjectRepository
    {
        private readonly DevFlowDbContext _context;


        /// Constructor - DbContext is injected by DI container
        public ProjectRepository(DevFlowDbContext context)
        {
            _context = context;
        }

    
        /// Get project by ID with related data
        /// Include() performs SQL JOIN to load related entities
        /// AsNoTracking() improves performance for read-only queries
        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _context.Projects
                .Include(p => p.Owner)        // JOIN with Users table
                .Include(p => p.Tasks)        // JOIN with Tasks table
                .AsNoTracking()               // Don't track changes (read-only)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
                // -- What Include() generates:
                //    SELECT * FROM Projects p
                //    LEFT JOIN Users u ON p.OwnerId = u.Id
                //    LEFT JOIN Tasks t ON t.ProjectId = p.Id
                //    WHERE p.Id = @id
       


        /// Get all projects for a specific user
        /// Used in dashboard to show "My Projects"
        public async Task<IEnumerable<Project>> GetByUserIdAsync(int userId)
        {
            return await _context.Projects
                .Include(p => p.Tasks)        // Load tasks with projects
                .Where(p => p.OwnerId == userId)  // Filter by owner
                .OrderByDescending(p => p.CreatedAt)  // Newest first
                .AsNoTracking()
                .ToListAsync();
        }

      
        /// Get all projects (admin view)
        /// Rarely used, but needed for admin dashboard
        public async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.Tasks)
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }


        /// Add new project to database
        /// EF tracks the entity and generates INSERT SQL on SaveChangesAsync
        public async Task<Project> AddAsync(Project project)
        {
            // Set timestamps
            project.CreatedAt = DateTime.UtcNow;
            
            // Add to DbContext (not yet saved to DB)
            await _context.Projects.AddAsync(project);
            
            // Save to database (executes INSERT)
            await _context.SaveChangesAsync();
            
            // ID is now populated by database
            return project;
        }

      
        /// Update existing project
        /// EF generates UPDATE SQL for changed properties only
        public async Task UpdateAsync(Project project)
        {
            // Set update timestamp
            project.UpdatedAt = DateTime.UtcNow;
            
            // Mark entity as modified (EF will generate UPDATE)
            _context.Projects.Update(project);
            
            // Execute UPDATE SQL
            await _context.SaveChangesAsync();
        }


        /// Delete project by ID
        /// Cascade delete removes all related tasks automatically
        /// (configured in DbContext OnModelCreating)
        public async Task DeleteAsync(int id)
        {
            // Find the project
            var project = await _context.Projects.FindAsync(id);
            
            // If exists, remove it
            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
        }

     
        /// Check if project exists
        /// Uses AnyAsync which generates: SELECT EXISTS (SELECT 1 FROM Projects WHERE Id = @id)
        /// More efficient than fetching the whole entity
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Projects.AnyAsync(p => p.Id == id);
        }
    }
}