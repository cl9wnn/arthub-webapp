using MyORM;
using Persistence.Entities;
namespace Persistence.Repositories;

public class ArtistRepository(QueryMapper queryMapper)
{
    public async Task CreateArtistAsync(int id, string fullname, string contactInfo, string summary, 
        CancellationToken cancellationToken = default)
    {
        FormattableString sqlQuery = $"""
                                        INSERT INTO "artists"
                                        VALUES ({id}, {fullname}, {contactInfo}, {summary});
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
}