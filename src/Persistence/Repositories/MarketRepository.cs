using System.Transactions;
using MyORM;
using Persistence.Entities;

namespace Persistence.Repositories;

public class MarketRepository(QueryMapper queryMapper)
{
    public async Task AddPointsToBalanceAsync(int userId, int pointsCount, CancellationToken cancellationToken = default)
    {
        FormattableString addPointsQuery = $"""
                                                    UPDATE userBalance
                                                    SET balance = balance + {pointsCount}
                                                    WHERE user_id = {userId}
                                                    RETURNING user_id, balance;
                                                """;
        
         await queryMapper.ExecuteAndReturnAsync<UserBalance?>(addPointsQuery, cancellationToken);
    }
    
    public async Task RemovePointsToBalanceAsync(int userId, int pointsCount, CancellationToken cancellationToken = default)
    {
        FormattableString removePointsQuery = $"""
                                                UPDATE userBalance
                                                SET balance = balance - {pointsCount}
                                                WHERE user_id = {userId} AND balance >= {pointsCount}
                                                RETURNING user_id, balance;
                                            """;
        
        await queryMapper.ExecuteAndReturnAsync<UserBalance?>(removePointsQuery, cancellationToken);
    }

    public async Task<Reward?> GetRewardByIdAsync(int rewardId, CancellationToken cancellationToken = default)
    {
        FormattableString getReward = $"""
                                           SELECT *
                                           FROM rewards
                                           WHERE reward_id = {rewardId};
                                       """;
        
        return await queryMapper.ExecuteAndReturnAsync<Reward?>(getReward, cancellationToken);
    }

    public async Task<List<Reward?>?> GetRewardsAsync(CancellationToken cancellationToken = default)
    {
        FormattableString selectRewardList = $"""
                                                    SELECT *
                                                    FROM rewards
                                              """;
       return await queryMapper.ExecuteAndReturnListAsync<Reward?>(selectRewardList, cancellationToken);
    }

    public async Task<bool> BuyRewardAsync(int userId, int artistId, int points, CancellationToken cancellationToken = default)
    {
        try
        {
            FormattableString removePointsQuery = $"""
                                                       UPDATE userBalance
                                                       SET balance = balance - {points}
                                                       WHERE user_id = {userId}
                                                   """;

            FormattableString addPointsQuery = $"""
                                                    UPDATE userBalance
                                                    SET balance = balance + {points}
                                                    WHERE user_id = {artistId}
                                                """;

            await queryMapper.ExecuteTransactionAsync(async transaction =>
            {
                await queryMapper.ExecuteWithTransactionAsync(removePointsQuery, transaction, cancellationToken);

                await queryMapper.ExecuteWithTransactionAsync(addPointsQuery, transaction, cancellationToken);

            }, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    
    public async Task<int> ReturnDailyLikeCountAsync(int userId, CancellationToken cancellationToken = default)
    {
        FormattableString selectCountQuery = $"""
                                                  SELECT COUNT(*)
                                                  FROM likes 
                                                  WHERE user_id = {userId} AND created_at = CURRENT_DATE;
                                              """;

        return await queryMapper.ExecuteAndReturnAsync<int>(selectCountQuery, cancellationToken);
    }
}