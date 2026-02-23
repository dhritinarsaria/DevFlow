using DevFlow.Domain.Entities;
using DevFlow.Domain.Enums;

namespace DevFlow.Infrastructure.Data
{
    /// <summary>
    /// Seeds initial data into database for testing
    /// </summary>
    public static class DbSeeder
    {
        public static void SeedData(DevFlowDbContext context)
        {
            // Check if we already have users (don't seed twice)
            if (context.Users.Any())
                return;

            // Create test user
            var testUser = new User
            {
                Username = "testuser",
                Email = "test@devflow.com",
                PasswordHash = "dummy_hash_for_now", // We'll do real auth later
                CreatedAt = DateTime.UtcNow,
                Role = UserRole.User
            };

            context.Users.Add(testUser);
            context.SaveChanges();

            Console.WriteLine("✅ Seeded test user (ID: 1)");
        }
    }
}