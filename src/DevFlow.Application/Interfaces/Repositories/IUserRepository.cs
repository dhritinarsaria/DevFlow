using DevFlow.Domain.Entities;

namespace DevFlow.Application.Interfaces
{
   
    /// Repository interface for User entity
    /// Handles user authentication and profile operations
       public interface IUserRepository
    {
       
        /// Get user by ID
   
        Task<User?> GetByIdAsync(int id);
        
        
        /// Get user by email (used for login)
        /// Email is unique, so this returns single user
       
        Task<User?> GetByEmailAsync(string email);
        
       
        /// Get user by username (used for profile lookup)
        /// Username is unique
             Task<User?> GetByUsernameAsync(string username);
        
     
        /// Add new user (registration)
        Task<User> AddAsync(User user);
        
     
        /// Update user profile
        Task UpdateAsync(User user);

        /// Check if email already exists (validation during registration)
        Task<bool> EmailExistsAsync(string email);
        
   
        /// Check if username already exists (validation during registration)
        Task<bool> UsernameExistsAsync(string username);
    }
}