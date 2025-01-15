using System.Transactions;
using MyORM;
using Persistence.Entities;
using Persistence.interfaces;

namespace Persistence.Repositories;

public class MarketRepository(QueryMapper queryMapper): IMarketRepository
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
    
    public async Task<bool> RemovePointsFromBalanceAsync(int userId, int pointsCount, CancellationToken cancellationToken = default)
    {
        try
        {
            FormattableString removePointsQuery = $"""
                                                       UPDATE userBalance
                                                       SET balance = balance - {pointsCount}
                                                       WHERE user_id = {userId}
                                                       RETURNING user_id, balance;
                                                   """;

            await queryMapper.ExecuteAndReturnAsync<UserBalance?>(removePointsQuery, cancellationToken);
        }
        catch (Exception ex)
        {
            return false;
        }

        return true;
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

    public async Task<ArtworkReward?> GetArtworkRewardAsync(int rewardId, int artworkId,
        CancellationToken cancellationToken = default)
    {
        FormattableString selectRewardQuery = $"""
                                                   SELECT *
                                                   FROM artworkRewards
                                                   WHERE artwork_id = {artworkId} AND reward_id = {rewardId};
                                               """;
        
        return await queryMapper.ExecuteAndReturnAsync<ArtworkReward?>(selectRewardQuery, cancellationToken);
    }
    
    public async Task<ArtworkReward?> AddArtworkRewardAsync(int rewardId, int artworkId, CancellationToken cancellationToken = default)
    {
         FormattableString addFirstRewardToArtworkQuery = $"""
                                                              INSERT INTO artworkRewards
                                                              VALUES ({rewardId}, {artworkId}, 1)
                                                              RETURNING *;
                                                              """;
         FormattableString updateRewardCountQuery = $"""
                                                        UPDATE artworkRewards
                                                        SET reward_count = reward_count + 1
                                                        WHERE artwork_id = {artworkId} and reward_id = {rewardId}
                                                        RETURNING *;
                                                     """;

        if (await GetArtworkRewardAsync(rewardId, artworkId, cancellationToken) == null)
        {
           return await queryMapper.ExecuteAndReturnAsync<ArtworkReward?>(addFirstRewardToArtworkQuery, cancellationToken);
        }

        return await queryMapper.ExecuteAndReturnAsync<ArtworkReward?>(updateRewardCountQuery, cancellationToken);
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

    public async Task<UserBalance?> ReturnUserBalanceAsync(int userId, CancellationToken cancellationToken = default)
    {
        FormattableString selectBalanceQuery = $"""
                                                  SELECT *
                                                  FROM userBalance 
                                                  WHERE user_id = {userId};
                                              """;

        return await queryMapper.ExecuteAndReturnAsync<UserBalance?>(selectBalanceQuery, cancellationToken);
    }

    public async Task<List<Decoration?>?> GetDecorationsAsync(CancellationToken cancellationToken = default)
    {
        FormattableString selectDecorationsQuery = $"""
                                                        SELECT decoration_id, name, cost, type_name
                                                        FROM decorations
                                                        INNER JOIN decorationTypes
                                                        ON decorations.type_id = decorationTypes.type_id
                                                        ORDER BY decoration_id ASC;
                                                    """;
        return await queryMapper.ExecuteAndReturnListAsync<Decoration?>(selectDecorationsQuery, cancellationToken);
    }
    
    public async Task<Decoration?> GetDecorationByIdAsync(int decorationId, CancellationToken cancellationToken = default)
    {
        FormattableString selectDecorationsQuery = $"""
                                                        SELECT d.decoration_id, d.name, d.cost, dt.type_name
                                                        FROM decorations d
                                                        INNER JOIN decorationTypes dt
                                                        ON d.type_id = dt.type_id
                                                        WHERE d.decoration_id = {decorationId};
                                                    """;
        
        return await queryMapper.ExecuteAndReturnAsync<Decoration?>(selectDecorationsQuery, cancellationToken);
    }

    public async Task<ArtistDecoration?> AddDecorationAsync(int decorationId, int userId,
        CancellationToken cancellationToken = default)
    {
        FormattableString insertQuery = $"""
                                            INSERT INTO userDecorations 
                                            VALUES ({decorationId}, {userId})
                                            RETURNING *;
                                         """;
        
        return await queryMapper.ExecuteAndReturnAsync<ArtistDecoration?>(insertQuery, cancellationToken);
    }

    public async Task<ArtistDecoration?> GetUserDecorationAsync(int decorationId, int userId,
        CancellationToken cancellationToken = default)
    {
        FormattableString selectDecorationsQuery = $"""
                                                        SELECT *
                                                        FROM userDecorations
                                                        WHERE decoration_id = {decorationId} AND user_id = {userId};
                                                    """;
        
        return await queryMapper.ExecuteAndReturnAsync<ArtistDecoration?>(selectDecorationsQuery, cancellationToken);
    }
    
    public async Task<List<ArtistDecoration?>> GetUserDecorationsAsync(int userId,
        CancellationToken cancellationToken = default)
    {
        FormattableString selectDecorationsQuery = $"""
                                                        SELECT *
                                                        FROM userDecorations
                                                        WHERE user_id = {userId};
                                                    """;
        
        return await queryMapper.ExecuteAndReturnListAsync<ArtistDecoration?>(selectDecorationsQuery, cancellationToken);
    }

    public async Task<bool> CheckSelectedStatusOfDecorationAsync(int decorationId, int userId
        , CancellationToken cancellationToken = default)
    {
        FormattableString selectDecorationQuery = $"""
                                                        SELECT *
                                                        FROM userDecorations
                                                        WHERE user_id = {userId} and decoration_id = {decorationId} and is_selected = true;
                                                    """;

        var decoration = await queryMapper.ExecuteAndReturnAsync<Decoration?>(selectDecorationQuery, cancellationToken);
        
        return decoration != null;
    }
    
    public async Task<bool> SelectMainDecorationAsync(int decorationId, int userId, string typeName,
        CancellationToken cancellationToken)
    {
        try
        {
            FormattableString updateSelectedFlagsQuery = $"""
                                                           UPDATE userDecorations
                                                           SET is_selected = false
                                                           FROM decorations d
                                                           INNER JOIN decorationTypes dt ON d.type_id = dt.type_id
                                                           WHERE userDecorations.decoration_id = d.decoration_id
                                                             AND userDecorations.user_id = {userId}
                                                             AND dt.type_name = {typeName};
                                                           """;

            FormattableString setSelectedFlagQuery = $"""
                                                      UPDATE userDecorations
                                                      SET is_selected = true
                                                      WHERE user_id = {userId} and  decoration_id = {decorationId}; 
                                                      """; 
            
                
            await queryMapper.ExecuteTransactionAsync(async transaction =>
            {
                await queryMapper.ExecuteWithTransactionAsync(updateSelectedFlagsQuery, transaction, cancellationToken);

                await queryMapper.ExecuteWithTransactionAsync(setSelectedFlagQuery, transaction, cancellationToken);

            }, cancellationToken);
            
        }
        catch (Exception ex)
        {
            return false;
        }

        return true;
    }

    public async Task<ArtistDecoration?> GetSelectedDecorationAsync(int userId, string typeName,
        CancellationToken cancellationToken)
    {
        FormattableString currentDecoration = $"""
                                                 SELECT *
                                                FROM userDecorations ud
                                                INNER JOIN decorations d ON ud.decoration_id = d.decoration_id
                                                INNER JOIN decorationTypes dt ON d.type_id = dt.type_id
                                                WHERE ud.user_id = {userId}
                                                AND dt.type_name = {typeName}
                                                AND ud.is_selected = true;                      
                                                
                                                """;
        return await queryMapper.ExecuteAndReturnAsync<ArtistDecoration?>(currentDecoration, cancellationToken);
    }
    
    
}

