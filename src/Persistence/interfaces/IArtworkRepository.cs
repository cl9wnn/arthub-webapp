using Persistence.Entities;

namespace Persistence.interfaces;

public interface IArtworkRepository
{
    Task<Artwork?> InsertArtworkAsync(Artwork artwork, List<string> tags, CancellationToken cancellationToken);

    Task<Artwork?> DeleteOwnArtworkAsync(int userId, int artworkId, CancellationToken cancellationToken);

    Task<List<Artwork>?> GetGalleryArtworksAsync(CancellationToken cancellationToken);

    Task<List<Artwork>?> GetProfileArtworksAsync(int userId, CancellationToken cancellationToken);

    Task<List<ArtMetrics>?> GetProfileArtMetricsAsync(List<int> artworksId, CancellationToken cancellationToken);

    Task<Artwork?> GetArtworkByIdAsync(int artworkId, CancellationToken cancellationToken);

    Task<List<Tag>?> GetTagsByIdAsync(int artworkId, CancellationToken cancellationToken);

    Task<int> AddLikeAsync(int artworkId, int userId, CancellationToken cancellationToken);

    Task<int> RemoveLikeAsync(int artworkId, int userId, CancellationToken cancellationToken);

    Task<bool> IsArtworkLikedByUserAsync(int artworkId, int userId, CancellationToken cancellationToken);

    Task<ArtMetrics?> UpdateViewCounterAsync(int artworkId, CancellationToken cancellationToken);

    Task<ArtMetrics?> GetMetricsByIdAsync(int artworkId, CancellationToken cancellationToken);

    Task<List<ArtworkReward>?> GetArtworkRewardListAsync(int artworkId, CancellationToken cancellationToken);

    Task<List<AccountReward>> GetAccountRewardListAsync(int userId, CancellationToken cancellationToken);

    Task<int> GetArtworkDecorationAsync(string typeName, int userId, CancellationToken cancellationToken);
}