using Persistence.Entities;
namespace Persistence.interfaces;

public interface ISavingFavouriteRepository
{
    Task<Artwork?> DeleteSavingArtworkAsync(int userId, int artworkId, CancellationToken cancellationToken);
        
    Task<SavingArt?> AddSavingArtAsync(int artworkId, int userId, CancellationToken cancellationToken);
        
    Task<List<Artwork?>?> GetSavingArtworksAsync(int userId, CancellationToken cancellationToken);
        
    Task<bool> IsArtworkSavedByUserAsync(int artworkId, int userId, CancellationToken cancellationToken);
        
    Task<SavingArt?> RemoveSavingAsync(int artworkId, int userId, CancellationToken cancellationToken);
}