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
        
        await SaveTagsAsync(tags, queryResult!.ArtworkId, cancellationToken);
        return queryResult;
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

    public async Task<List<Artwork>?> GetArtworksWithUserDetailsAsync(CancellationToken cancellationToken)
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
}

