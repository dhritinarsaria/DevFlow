using DevFlow.Domain.Entities;

namespace DevFlow.Application.Interfaces
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(string username, string email, string password);
        Task<User?> ValidateCredentialsAsync(string email, string password);
    }
}