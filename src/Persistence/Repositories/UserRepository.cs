using System.Data;
using MyORM;
using Npgsql;
using Persistence.Entities;
namespace Persistence.Repositories;

public class UserRepository(QueryMapper queryMapper)
{
    public async Task<User?> CreateUserAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        FormattableString sqlQuery = $"""
                                      INSERT INTO "users" (login, password) VALUES ({login}, {password})
                                      RETURNING *;
                                      """;
        
          var queryResult =await queryMapper.ExecuteAndReturnAsync<User>(sqlQuery, cancellationToken);
          return queryResult;
    }
    
    public async Task<User?> GetUserAsync(string login, CancellationToken cancellationToken = default)
    {
        FormattableString sqlQuery = $"SELECT * FROM users WHERE login = {login}";
            
        var queryResult = await queryMapper.ExecuteAndReturnAsync<User>(sqlQuery, cancellationToken);
       
        return queryResult;
    }
    
    public async Task DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
        FormattableString sqlQuery = $"DELETE FROM users WHERE user_id = {id}";
        
        await queryMapper.ExecuteAsync(sqlQuery, cancellationToken);
    }
    
    public async Task<User?> UpdateUserAsync(int id, string newLogin, CancellationToken cancellationToken = default)
    {
        FormattableString sqlQuery = $"""
                                      UPDATE users
                                      SET login = {newLogin}
                                      WHERE user_id = {id}
                                      RETURNING *;
                                      """;
        
         var newUser = await queryMapper.ExecuteAndReturnAsync<User>(sqlQuery, cancellationToken);
         return newUser;
    }
}