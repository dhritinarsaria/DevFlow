using Microsoft.EntityFrameworkCore;
using DevFlow.Application.Interfaces;
using DevFlow.Domain.Entities;
using DevFlow.Infrastructure.Data;

namespace DevFlow.Infrastructure.Repositories
{
    
    /// Implementation of IUserRepository
    /// Handles user authentication and profile data access
    public class UserRepository : IUserRepository
    {
        private readonly DevFlowDbContext _context;

        public UserRepository(DevFlowDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }


        /// Get user by email - used for login
        /// Email has unique index, so this is fast (O(log n))
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

      
        /// Get user by username - used for profile lookup
        /// Username has unique index
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);
        }

      
        /// Register new user
        /// Password should already be hashed before calling this
        public async Task<User> AddAsync(User user)
        {
            user.CreatedAt = DateTime.UtcNow;
            
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            
            return user;
        }


        /// Update user profile
        public async Task UpdateAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        
        /// Check if email exists - validation during registration
        /// Prevents duplicate email errors
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

    
        /// Check if username exists - validation during registration
        /// Prevents duplicate username errors
        /// AnyAsync() stops at first match
        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
    }
}