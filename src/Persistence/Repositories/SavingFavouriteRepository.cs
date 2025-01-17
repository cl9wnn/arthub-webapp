using MyORM;
using MyORM.interfaces;
using Persistence.Entities;
using Persistence.interfaces;

namespace Persistence.Repositories;

public class SavingFavouriteRepository(IQueryMapper queryMapper): ISavingFavouriteRepository
{
    public async Task<Artwork?> DeleteSavingArtworkAsync(int userId, int artworkId, CancellationToken cancellationToken)
    {
        FormattableString sqlQuery = $"""
                                      DELETE from savings
                                      WHERE artwork_id = {artworkId} AND user_id = {userId}
                                      RETURNING artwork_id, user_id;
                                      """;
        return await queryMapper.ExecuteAndReturnAsync<Artwork?>(sqlQuery, cancellationToken);
    }
    
    public async Task<SavingArt?> AddSavingArtAsync(int artworkId, int userId, CancellationToken cancellationToken)
    {
        FormattableString addSaving = $"""
                                       INSERT INTO savings (artwork_id, user_id)
                                       VALUES ({artworkId}, {userId})
                                       RETURNING artwork_id, user_id;
                                       """;

        return await queryMapper.ExecuteAndReturnAsync<SavingArt>(addSaving, cancellationToken);
    }
    
    public async Task<List<Artwork?>?> GetSavingArtworksAsync(int userId, CancellationToken cancellationToken)
    {
        FormattableString sqlQuery = $"""
                                      SELECT artworks.artwork_id, title, artwork_path
                                        FROM artworks
                                        JOIN savings ON artworks.artwork_id = savings.artwork_id 
                                        WHERE savings.user_id = {userId};
                                      """;
        var savings = 
            await queryMapper.ExecuteAndReturnListAsync<Artwork?>(sqlQuery, cancellationToken);
        
        return savings;
    }
    
    public async Task<bool> IsArtworkSavedByUserAsync(int artworkId, int userId, CancellationToken cancellationToken)
    {
        FormattableString sqlQuery = $"""
                                        SELECT COUNT(*) FROM savings
                                        WHERE artwork_id = {artworkId} AND user_id = {userId};
                                      """;

        var count = await queryMapper.ExecuteAndReturnAsync<int>(sqlQuery, cancellationToken);
        return count > 0
            ? true
            : false;
    }
    
    public async Task<SavingArt?> RemoveSavingAsync(int artworkId, int userId, CancellationToken cancellationToken)
    {
        FormattableString removeSaving = $"""
                                            DELETE FROM savings
                                            WHERE artwork_id = {artworkId} AND user_id = {userId}
                                            RETURNING artwork_id, user_id;
                                          """;
        
        return await queryMapper.ExecuteAndReturnAsync<SavingArt>(removeSaving, cancellationToken);
    }
}