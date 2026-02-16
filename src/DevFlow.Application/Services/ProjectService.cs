using DevFlow.Application.DTOs.Projects;
using DevFlow.Application.Interfaces;
using DevFlow.Domain.Entities;
using System.Linq;

namespace DevFlow.Application.Services
{
    
    /// Implementation of project business logic
    /// Handles validation, authorization, and orchestration
    
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;

        public ProjectService(
            IProjectRepository projectRepository,
            IUserRepository userRepository)
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
        }

    
        /// Get project by ID with permission check
        public async Task<ProjectDto?> GetProjectByIdAsync(int id, int userId)
        {
            // Get project from repository
            var project = await _projectRepository.GetByIdAsync(id);
            
            // Not found?
            if (project == null)
                return null;
            
            // Permission check: User can only see their own projects
            // (Later we might add admin or sharing features)
            if (project.OwnerId != userId)
                return null;
            
            // Map domain entity to DTO
            return MapToProjectDto(project);
        }

        
        /// Get all projects for a user
        public async Task<IEnumerable<ProjectListDto>> GetUserProjectsAsync(int userId)
        {
            // Get user's projects from repository
            var projects = await _projectRepository.GetByUserIdAsync(userId);
            
            // Map to list DTOs (lighter than full DTO)
            return projects.Select(p => new ProjectListDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                TaskCount = p.Tasks?.Count ?? 0,  // Count tasks (if loaded)
                CreatedAt = p.CreatedAt
            });
        }


        /// Create new project with validation
        public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto createDto, int userId)
        {
            // Validation: Name required
            if (string.IsNullOrWhiteSpace(createDto.Name))
                throw new ArgumentException("Project name is required");
            
            // Validation: Name length
            if (createDto.Name.Length > 200)
                throw new ArgumentException("Project name cannot exceed 200 characters");
            
            // Business rule: Check if user exists
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");
            
            // Business rule: User cannot have more than 50 projects
            // (Reasonable limit to prevent abuse)
            var userProjects = await _projectRepository.GetByUserIdAsync(userId);
            if (userProjects.Count() >= 50)
                throw new InvalidOperationException("Maximum 50 projects per user");
            
            // Create domain entity
            var project = new Project
            {
                Name = createDto.Name.Trim(),
                Description = createDto.Description?.Trim() ?? string.Empty,
                OwnerId = userId,
                Tags = createDto.Tags != null && createDto.Tags.Any()
                    ? string.Join(",", createDto.Tags)  // Convert list to comma-separated
                    : string.Empty,
                CreatedAt = DateTime.UtcNow
            };
            
            // Save via repository
            var savedProject = await _projectRepository.AddAsync(project);
            
            // Load owner for DTO mapping
            savedProject.Owner = user;
            
            // Map to DTO and return
            return MapToProjectDto(savedProject);
        }

      
        /// Update project with permission check
        public async Task<ProjectDto?> UpdateProjectAsync(int id, UpdateProjectDto updateDto, int userId)
        {
            // Get existing project
            var project = await _projectRepository.GetByIdAsync(id);
            
            // Not found?
            if (project == null)
                return null;
            
            // Permission check: Only owner can update
            if (project.OwnerId != userId)
                return null;
            
            // Validation
            if (string.IsNullOrWhiteSpace(updateDto.Name))
                throw new ArgumentException("Project name is required");
            
            if (updateDto.Name.Length > 200)
                throw new ArgumentException("Project name cannot exceed 200 characters");
            
            // Update fields
            project.Name = updateDto.Name.Trim();
            project.Description = updateDto.Description?.Trim() ?? string.Empty;
            project.Tags = updateDto.Tags != null && updateDto.Tags.Any()
                ? string.Join(",", updateDto.Tags)
                : string.Empty;
            project.UpdatedAt = DateTime.UtcNow;
            
            // Save changes
            await _projectRepository.UpdateAsync(project);
            
            // Return updated DTO
            return MapToProjectDto(project);
        }

      
        /// Delete project with permission check
        public async Task<bool> DeleteProjectAsync(int id, int userId)
        {
            // Get project
            var project = await _projectRepository.GetByIdAsync(id);
            
            // Not found?
            if (project == null)
                return false;
            
            // Permission check: Only owner can delete
            if (project.OwnerId != userId)
                return false;
            
            // Delete via repository
            await _projectRepository.DeleteAsync(id);
            
            return true;
        }


        /// Helper: Map domain entity to DTO
        /// Centralizes mapping logic in one place       
        private ProjectDto MapToProjectDto(Project project)
        {
            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                OwnerUsername = project.Owner?.Username ?? "Unknown",  // Safe navigation
                OwnerId = project.OwnerId,
                Tags = !string.IsNullOrEmpty(project.Tags)
                    ? project.Tags.Split(',').ToList()  // Convert comma-separated to list
                    : new List<string>(),
                TaskCount = project.Tasks?.Count ?? 0,  // Count tasks if loaded
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt
            };
        }
    }
}