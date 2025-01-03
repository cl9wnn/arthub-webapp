﻿using MyORM;
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
        
        await SaveTagsAsync(tags, queryResult!.ArtworkId, cancellationToken);
        return queryResult;
    }
    

    public async Task<List<Artwork>?> GetGalleryArtworksAsync(CancellationToken cancellationToken)
    {
        FormattableString insertArtworkTagQuery = $"""
                                                     SELECT artwork_id, title, artwork_path, artworks.user_id, users.profile_name 
                                                     FROM artworks
                                                     JOIN users ON artworks.user_id = users.user_id
                                                     """;
        var artworks = 
            await queryMapper.ExecuteAndReturnListAsync<Artwork?>(insertArtworkTagQuery, cancellationToken);
        
        return artworks;
    }

    public async Task<Artwork?> GetArtworkByIdAsync(int artworkId, CancellationToken cancellationToken)
    {
        FormattableString sqlQuery = $"""
                                        SELECT artwork_id, title, category, description, artwork_path, user_id, like_count
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
        FormattableString addLike = $"""
                                      INSERT INTO likes (artwork_id, user_id)
                                      VALUES ({artworkId}, {userId})
                                      RETURNING artwork_id, user_id;
                                      """;
        
        FormattableString incrementLikeCount = $"""
                                                    UPDATE artworks
                                                    SET like_count = like_count + 1
                                                    WHERE artwork_id = {artworkId}
                                                    RETURNING like_count;
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
                                                    UPDATE artworks
                                                    SET like_count = like_count - 1
                                                    WHERE artwork_id = {artworkId}
                                                    RETURNING like_count;
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
}

