using Persistence.Entities;

namespace Persistence.interfaces;

public interface IMarketRepository
{
     Task AddPointsToBalanceAsync(int userId, int pointsCount, CancellationToken cancellationToken = default);
        Task<bool> RemovePointsFromBalanceAsync(int userId, int pointsCount, CancellationToken cancellationToken = default);
        Task<Reward?> GetRewardByIdAsync(int rewardId, CancellationToken cancellationToken = default);
        Task<ArtworkReward?> GetArtworkRewardAsync(int rewardId, int artworkId, CancellationToken cancellationToken = default);
        Task<ArtworkReward?> AddArtworkRewardAsync(int rewardId, int artworkId, CancellationToken cancellationToken = default);
        Task<List<Reward?>?> GetRewardsAsync(CancellationToken cancellationToken = default);
        Task<bool> BuyRewardAsync(int userId, int artistId, int points, CancellationToken cancellationToken = default);
        Task<int> ReturnDailyLikeCountAsync(int userId, CancellationToken cancellationToken = default);
        Task<UserBalance?> ReturnUserBalanceAsync(int userId, CancellationToken cancellationToken = default);
        Task<List<Decoration?>?> GetDecorationsAsync(CancellationToken cancellationToken = default);
        Task<Decoration?> GetDecorationByIdAsync(int decorationId, CancellationToken cancellationToken = default);
        Task<ArtistDecoration?> AddDecorationAsync(int decorationId, int userId, CancellationToken cancellationToken = default);
        Task<ArtistDecoration?> GetUserDecorationAsync(int decorationId, int userId, CancellationToken cancellationToken = default);
        Task<List<ArtistDecoration?>> GetUserDecorationsAsync(int userId, CancellationToken cancellationToken = default);
        Task<bool> CheckSelectedStatusOfDecorationAsync(int decorationId, int userId, CancellationToken cancellationToken = default);
        Task<bool> SelectMainDecorationAsync(int decorationId, int userId, string typeName, CancellationToken cancellationToken = default);
        Task<ArtistDecoration?> GetSelectedDecorationAsync(int userId, string typeName, CancellationToken cancellationToken = default);
}