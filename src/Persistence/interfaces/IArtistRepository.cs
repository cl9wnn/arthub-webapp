using Persistence.Entities;

namespace Persistence.interfaces;

public interface IArtistRepository
{
    Task CreateArtistAsync(int userId, string fullname, string summary, string contactInfo, CancellationToken cancellationToken);
    Task<User?> UpdateUserToArtistAsync(int id, CancellationToken cancellationToken = default);
    Task<UpgradeUser?> SelectArtistByIdAsync(int userId, CancellationToken cancellationToken = default);
}