using DevFlow.Application.Interfaces;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enums;

namespace DevFlow.Application.Services
{
 
    /// Service for authentication operations
    /// Handles user registration and password validation
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        
        /// Register a new user with hashed password
        public async Task<User> RegisterAsync(string username, string email, string password)
        {
            // Validation: Email already exists?
            if (await _userRepository.EmailExistsAsync(email))
                throw new InvalidOperationException("Email already registered");

            // Validation: Username already exists?
            if (await _userRepository.UsernameExistsAsync(username))
                throw new InvalidOperationException("Username already taken");

            // Validation: Password strength
            if (password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters");

            // Hash password using BCrypt
            // Work factor 11 = 2^11 iterations (good balance of security & speed)
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);

            // Create user entity
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow,
                Role = UserRole.User
            };

            // Save to database
            return await _userRepository.AddAsync(user);
        }

     
        /// Validate user credentials
        public async Task<User?> ValidateCredentialsAsync(string email, string password)
        {
            // Find user by email
            var user = await _userRepository.GetByEmailAsync(email);
            
            if (user == null)
                return null;

            // Verify password against hash
            // BCrypt.Verify handles salt extraction and comparison
            bool isValidPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            return isValidPassword ? user : null;
        }
    }
}