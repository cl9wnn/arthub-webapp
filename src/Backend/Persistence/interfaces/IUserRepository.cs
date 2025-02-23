using Persistence.Entities;
namespace Persistence.interfaces;

public interface IUserRepository
{
        Task<User?> CreateUserAsync(User user, CancellationToken cancellationToken = default);
        Task<User?> GetUserAsyncByLogin(string login, CancellationToken cancellationToken = default);
        Task<User?> GetUserAsyncById(int id, CancellationToken cancellationToken = default);
        Task<string?> GetUserRoleAsync(int userId, CancellationToken cancellationToken = default);
        Task<UpgradeUser?> GetUpgradeUserByIdAsync(int id, CancellationToken cancellationToken = default);
}
