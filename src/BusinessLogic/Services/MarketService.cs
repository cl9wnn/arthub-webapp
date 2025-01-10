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
    
    public async Task<Result<ArtworkReward>> GiveRewardAsync(int rewardId, int artworkId,  int userId, CancellationToken cancellationToken)
    {
        var reward = await marketRepository.GetRewardByIdAsync(rewardId, cancellationToken);
        
        if (reward == null )
            return Result<ArtworkReward>.Failure(400, "Reward id is invalid")!;
        
        var artwork = await artworkRepository.GetArtworkByIdAsync(artworkId, cancellationToken);
        
        if (artwork == null || artwork.UserId == userId)
            return Result<ArtworkReward>.Failure(400, "Art work id is invalid")!;

        var isBought = await marketRepository.BuyRewardAsync(userId, artwork.UserId, reward.Cost, cancellationToken);

        if (!isBought) return Result<ArtworkReward>.Failure(400, "Not enough money")!;
        
        var artworkReward =  await marketRepository.AddArtworkRewardAsync(rewardId, artworkId, cancellationToken);
        return artworkReward != null
            ? Result<ArtworkReward>.Success(artworkReward)
            : Result<ArtworkReward>.Failure(400, "Artwork id is invalid")!;

    }
    
    public async Task<Result<ArtistDecoration>> BuyDecorationAsync(int decorationId, int userId,
        CancellationToken cancellationToken)
    {
        var decoration = await marketRepository.GetDecorationByIdAsync(decorationId, cancellationToken);
            
        if (decoration == null)
            return Result<ArtistDecoration>.Failure(400, "Decoration id is invalid")!;
        
        var isBought = await marketRepository.RemovePointsFromBalanceAsync(userId, decoration.Cost, cancellationToken);
        
        if (!isBought) return Result<ArtistDecoration>.Failure(400, "Not enough money")!;
        
        var artistDecoration =  await marketRepository.AddDecorationAsync(decorationId, userId, cancellationToken);
        
        return artistDecoration != null
            ? Result<ArtistDecoration>.Success(artistDecoration)
            : Result<ArtistDecoration>.Failure(400, "Decoration id is invalid")!;
    }

    public async Task<Result<int>> GetUserBalanceByIdAsync(int userId, CancellationToken cancellationToken)
    {
        var userBalance = await marketRepository.ReturnUserBalanceAsync(userId, cancellationToken);
        
        return userBalance == null
            ? Result<int>.Failure(400, "User balance is invalid")
            : Result<int>.Success(userBalance.Balance);
    }

    public async Task<Result<List<Decoration>>> GetDecorationListAsync(CancellationToken cancellationToken)
    {
        var decorationList = await marketRepository.GetDecorationsAsync(cancellationToken);
        
        if (decorationList == null || decorationList.Count == 0)
            return Result<List<Decoration>>.Failure(400, "Decoration list is empty")!;
        
        return Result<List<Decoration>>.Success(decorationList!);
    }

    public async Task<Result<bool>> CheckDecorationExistenceAsync(int decorationId, int userId,
        CancellationToken cancellationToken)
    {
        var userDecoration = await marketRepository.GetUserDecorationAsync(decorationId, userId,cancellationToken);

        return userDecoration != null
            ? Result<bool>.Failure(400, "You are already have this decoration!")
            : Result<bool>.Success(true);
    }
}