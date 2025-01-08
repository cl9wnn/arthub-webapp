using Persistence.Entities;
using Persistence.Repositories;

namespace BusinessLogic.Services;

public class MarketService(MarketRepository marketRepository, ArtworkRepository artworkRepository)
{
    public async Task<Result<List<Reward>>> GetRewardListAsync(CancellationToken cancellationToken)
    {
        var rewardsList = await marketRepository.GetRewardsAsync(cancellationToken);

        if (rewardsList == null || rewardsList.Count == 0)
            return Result<List<Reward>>.Failure(400, "Reward list is empty")!;
        
        return Result<List<Reward>>.Success(rewardsList!);
    }

    public async Task<Result<Reward>> GiveRewardAsync(int rewardId, int artworkId,  int userId, CancellationToken cancellationToken)
    {
        var reward = await marketRepository.GetRewardByIdAsync(rewardId, cancellationToken);
        
        if (reward == null)
            return Result<Reward>.Failure(400, "Reward id is invalid")!;
        
        var artwork = await artworkRepository.GetArtworkByIdAsync(artworkId, cancellationToken);
        
        if (artwork == null)
            return Result<Reward>.Failure(400, "Art work id is invalid")!;

        var isBought = await marketRepository.BuyRewardAsync(userId, artwork.UserId, reward.Cost, cancellationToken);
        
        return isBought == true
            ? Result<Reward>.Success(reward)
            : Result<Reward>.Failure(400, "Not enough money")!;
    }
}