using MyORM;
using Persistence.Entities;

namespace Persistence.Repositories;

public class ArtworkRepository(QueryMapper queryMapper)
{
    public async Task<Artwork?> InsertArtworkAsync(Artwork artwork, List<string> tags, CancellationToken cancellationToken)
    {
        FormattableString insertArtworkQuery = $"""
                                              INSERT INTO artworks (title, category, description, artwork_path, user_id)
                                              VALUES ({artwork.Title}, {artwork.Category}, {artwork.Description},
                                               {artwork.ArtworkPath}, {artwork.UserId})
                                              RETURNING artwork_id, title, category, description, artwork_path, user_id;
                                              """;
        
        var queryResult =await queryMapper.ExecuteAndReturnAsync<Artwork?>(insertArtworkQuery, cancellationToken);
        
        if (queryResult == null)
            return null;
        
        var metrics = await CreateArtMetricsAsync(queryResult.ArtworkId, cancellationToken);
        
        if (metrics == null)
            return null;
        
        await SaveTagsAsync(tags, queryResult!.ArtworkId, cancellationToken);
        return queryResult;
    }

    private async Task<ArtMetrics?> CreateArtMetricsAsync(int artworkId, CancellationToken cancellationToken)
    {
        FormattableString insertQuery = $"""
                                                INSERT INTO artmetrics (artwork_id)
                                                VALUES ({artworkId})
                                                RETURNING artwork_id, likes_count, views_count;
                                                """;
        return await queryMapper.ExecuteAndReturnAsync<ArtMetrics?>(insertQuery, cancellationToken);
    }
    
    public async Task<Artwork?> DeleteOwnArtworkAsync(int userId, int artworkId, CancellationToken cancellationToken)
    {
        FormattableString sqlQuery = $"""
                                      DELETE from artworks
                                      WHERE artwork_id = {artworkId} AND user_id = {userId}
                                      RETURNING artwork_id, user_id;
                                      """;
        return await queryMapper.ExecuteAndReturnAsync<Artwork?>(sqlQuery, cancellationToken);
    }


    public async Task<List<Artwork>?> GetGalleryArtworksAsync(CancellationToken cancellationToken)
    {
        FormattableString sqlQuery = $"""
                                                     SELECT artwork_id, title, artwork_path, category, artworks.user_id, users.profile_name 
                                                     FROM artworks
                                                     JOIN users ON artworks.user_id = users.user_id
                                                     """;
        var artworks = 
            await queryMapper.ExecuteAndReturnListAsync<Artwork?>(sqlQuery, cancellationToken);
        
        return artworks;
    }
    
    public async Task<List<Artwork>?> GetProfileArtworksAsync(int userId, CancellationToken cancellationToken)
    {
        FormattableString sqlQuery = $"""
                                                   SELECT artwork_id, artwork_path
                                                   FROM artworks        
                                                   WHERE user_id = {userId};
                                                   """;
        var artworks = 
            await queryMapper.ExecuteAndReturnListAsync<Artwork?>(sqlQuery, cancellationToken);
        
        return artworks;
    }
    
    public async Task<List<ArtMetrics>?> GetProfileArtMetricsAsync(List<int> artworksId, CancellationToken cancellationToken)
    {
        var artMetrics = new List<ArtMetrics>();
        
        foreach (var artworkId in artworksId)
        {
            FormattableString sqlQuery = $"""
                                          SELECT artwork_id, likes_count, views_count
                                          FROM artMetrics        
                                          WHERE artwork_id = {artworkId};
                                          """;
            var artMetric = await queryMapper.ExecuteAndReturnAsync<ArtMetrics?>(sqlQuery, cancellationToken);   
            
            if (artMetric != null)
                artMetrics.Add(artMetric);
        }
        return artMetrics;
    }

    public async Task<Artwork?> GetArtworkByIdAsync(int artworkId, CancellationToken cancellationToken)
    {
        FormattableString sqlQuery = $"""
                                        SELECT artwork_id, title, category, description, artwork_path, user_id
                                        FROM artworks
                                        WHERE artwork_id = {artworkId};
                                      """;
        
        return await queryMapper.ExecuteAndReturnAsync<Artwork?>(sqlQuery, cancellationToken);
    }
    
    public async Task<List<Tag>?> GetTagsByIdAsync(int artworkId, CancellationToken cancellationToken)
    {
        FormattableString sqlQuery = $"""
                                      SELECT name
                                      FROM tags
                                      JOIN artworktags ON tags.tag_id = artworktags.tag_id 
                                      WHERE artwork_id = {artworkId};
                                      """;
        
        return await queryMapper.ExecuteAndReturnListAsync<Tag>(sqlQuery, cancellationToken)!;
    }

    private async Task SaveTagsAsync(List<string> tags, int artworkId, CancellationToken cancellationToken)
    {
        foreach (var tag in tags)
        {
            FormattableString selectTagSql = $"""
                                                  SELECT tag_id FROM tags 
                                                  WHERE name = {tag};
                                              """;

            var tagId = await queryMapper.ExecuteAndReturnAsync<int>(selectTagSql, cancellationToken);
            
            if (tagId == -1)
            {
                throw new InvalidDataException("Такого тега не существует!");
            }

            FormattableString insertArtworkTagQuery = $"""
                                                           INSERT INTO artworkTags (artwork_id, tag_id)
                                                           VALUES ({artworkId}, {tagId});
                                                       """;

            await queryMapper.ExecuteAsync(insertArtworkTagQuery, cancellationToken);
        }
    }

    public async Task<int> AddLikeAsync(int artworkId, int userId, CancellationToken cancellationToken)
    {
        var currentDate = DateTime.UtcNow.Date;
        
        FormattableString addLike = $"""
                                      INSERT INTO likes (artwork_id, user_id, created_at)
                                      VALUES ({artworkId}, {userId}, {currentDate})
                                      RETURNING artwork_id, user_id, created_at;
                                      """;
        
        FormattableString incrementLikeCount = $"""
                                                    UPDATE artMetrics
                                                    SET likes_count = likes_count + 1
                                                    WHERE artwork_id = {artworkId}
                                                    RETURNING likes_count;
                                                """;
        
        var like = await queryMapper.ExecuteAndReturnAsync<Like>(addLike, cancellationToken);
        
        if (like == null)
            return -1;
        
        return await queryMapper.ExecuteAndReturnAsync<int>(incrementLikeCount, cancellationToken);
    }

    public async Task<int> RemoveLikeAsync(int artworkId, int userId, CancellationToken cancellationToken)
    {
        FormattableString removeLike = $"""
                                        DELETE FROM likes
                                        WHERE artwork_id = {artworkId} AND user_id = {userId}
                                        RETURNING artwork_id, user_id;
                                      """;
        
        FormattableString decrementLikeCount = $"""
                                                    UPDATE artMetrics
                                                    SET likes_count = likes_count - 1
                                                    WHERE artwork_id = {artworkId}
                                                    RETURNING likes_count;
                                                """;
        
        var like = await queryMapper.ExecuteAndReturnAsync<Like>(removeLike, cancellationToken);
        
        if (like == null)
            return -1;
        
        return await queryMapper.ExecuteAndReturnAsync<int>(decrementLikeCount, cancellationToken);
    }
    
    public async Task<bool> IsArtworkLikedByUserAsync(int artworkId, int userId, CancellationToken cancellationToken)
    {
        FormattableString sqlQuery = $"""
                                        SELECT COUNT(*) FROM likes
                                        WHERE artwork_id = {artworkId} AND user_id = {userId};
                                      """;

        var count = await queryMapper.ExecuteAndReturnAsync<int>(sqlQuery, cancellationToken);
        return count > 0
            ? true
            : false;
    }

    public async Task<ArtMetrics?> UpdateViewCounterAsync(int artworkId, CancellationToken cancellationToken)
    {
        FormattableString sqlQuery = $"""
                                      UPDATE artMetrics
                                      SET views_count = views_count + 1
                                      WHERE artwork_id = {artworkId}
                                      RETURNING likes_count;
                                      """;
        return  await queryMapper.ExecuteAndReturnAsync<ArtMetrics?>(sqlQuery, cancellationToken);
    }
    
    public async Task<ArtMetrics?> GetMetricsByIdAsync(int artworkId, CancellationToken cancellationToken = default)
    {
        FormattableString sqlQuery = $"""
                                         SELECT likes_count, views_count
                                         FROM artMetrics
                                         WHERE artwork_id = {artworkId};
                                      """;
        
        return await queryMapper.ExecuteAndReturnAsync<ArtMetrics?>(sqlQuery, cancellationToken);
    }
}

