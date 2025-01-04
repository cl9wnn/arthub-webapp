using MyORM;
using Persistence.Entities;
namespace Persistence.Repositories;

public class ArtistRepository(QueryMapper queryMapper)
{
    public async Task CreateArtistAsync(int userId, string fullname, string summary, string contactInfo,
        CancellationToken cancellationToken)
    {
        FormattableString sqlQuery = $"""
                                        INSERT INTO "artists"
                                        VALUES ({userId}, {fullname},
                                        {contactInfo}, {summary});
                                      """;
        await queryMapper.ExecuteAsync(sqlQuery, cancellationToken);
    }
    
    public async Task<User?> UpdateUserToArtistAsync(int id, CancellationToken cancellationToken = default)
    {
        const string newRole = "artist";
        
        FormattableString sqlQuery = $"""
                                      UPDATE users
                                      SET role = {newRole}
                                      WHERE user_id = {id}
                                      RETURNING *;
                                      """;
        
        var newUser = await queryMapper.ExecuteAndReturnAsync<User>(sqlQuery, cancellationToken);
        return newUser;
    }

    public async Task<UpgradeUser?> SelectArtistByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        FormattableString sqlQuery = $"""
                                        SELECT * 
                                        FROM artists
                                        WHERE user_id = {userId};
                                      """;
        
        return await queryMapper.ExecuteAndReturnAsync<UpgradeUser>(sqlQuery, cancellationToken);
    }
}