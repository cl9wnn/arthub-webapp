using MyORM;
using Persistence.Entities;
namespace Persistence.Repositories;

public class UserRepository(QueryMapper queryMapper)
{
    public async Task<User?> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        FormattableString sqlQuery = $"""
                                      INSERT INTO "users" (login, password, profile_name, country, avatar_path) 
                                      VALUES ({user.Login}, {user.Password}, {user.ProfileName}, {user.Country}, {user.AvatarPath})
                                      RETURNING *;
                                      """;
        
          var queryResult =await queryMapper.ExecuteAndReturnAsync<User>(sqlQuery, cancellationToken);

          if (queryResult == null)
              return null;
          
          var userBalance = await CreateUserBalanceAsync(queryResult.UserId, cancellationToken);
          return userBalance == null ? null : queryResult;
    }
    
    private async Task<UserBalance?> CreateUserBalanceAsync(int userId, CancellationToken cancellationToken)
    {
        FormattableString insertQuery = $"""
                                         INSERT INTO userBalance (user_id)
                                         VALUES ({userId})
                                         RETURNING user_id, balance;
                                         """;
        return await queryMapper.ExecuteAndReturnAsync<UserBalance?>(insertQuery, cancellationToken);
    }
    
    public async Task<User?> GetUserAsyncByLogin(string login, CancellationToken cancellationToken = default)
    {
        FormattableString sqlQuery = $"SELECT * FROM users WHERE login = {login};";
            
        var queryResult = await queryMapper.ExecuteAndReturnAsync<User>(sqlQuery, cancellationToken);
       
        return queryResult;
    }
    
    public async Task<User?> GetUserAsyncById(int id, CancellationToken cancellationToken = default)
    {
        FormattableString sqlQuery = $"""
                                      SELECT profile_name, country, avatar_path
                                      FROM users 
                                      WHERE user_id = {id};
                                      """;
            
        var queryResult = await queryMapper.ExecuteAndReturnAsync<User>(sqlQuery, cancellationToken);
       
        return queryResult;
    }

    public async Task<string?> GetUserRoleAsync(int userId, CancellationToken cancellationToken = default)
    {
        FormattableString sqlQuery = $"SELECT role FROM users WHERE user_id = {userId};";
        
        return await queryMapper.ExecuteAndReturnAsync<string?>(sqlQuery, cancellationToken);
        
    }
    
    public async Task<UpgradeUser?> GetUpgradeUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        FormattableString sqlQuery = $"""
                                      SELECT profile_name, fullname, contact_info, country, avatar_path, summary
                                      FROM users 
                                      INNER JOIN artists ON users.user_id = artists.user_id
                                      WHERE users.user_id = {id};
                                      """;
            
        var queryResult = await queryMapper.ExecuteAndReturnAsync<UpgradeUser>(sqlQuery, cancellationToken);
       
        return queryResult;
    }
}