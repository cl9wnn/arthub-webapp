﻿using System.Data;
using MyORM;
using Npgsql;
using Persistence.Entities;
namespace Persistence.Repositories;

public class UserRepository(QueryMapper queryMapper)
{
    public async Task<User?> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        FormattableString sqlQuery = $"""
                                      INSERT INTO "users" (login, password, profile_name, country, avatar) 
                                      VALUES ({user.Login}, {user.Password}, {user.ProfileName}, {user.Country}, {user.Avatar})
                                      RETURNING *;
                                      """;
        
          var queryResult =await queryMapper.ExecuteAndReturnAsync<User>(sqlQuery, cancellationToken);
          return queryResult;
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
                                      SELECT profile_name, country, avatar 
                                      FROM users 
                                      WHERE user_id = {id};
                                      """;
            
        var queryResult = await queryMapper.ExecuteAndReturnAsync<User>(sqlQuery, cancellationToken);
       
        return queryResult;
    }
    
    public async Task<UpgradeUser?> GetUpgradeUserAsyncById(int id, CancellationToken cancellationToken = default)
    {
        FormattableString sqlQuery = $"""
                                      SELECT profile_name, fullname, contact_info, country, avatar 
                                      FROM users 
                                      INNER JOIN artists ON users.user_id = artists.user_id
                                      WHERE users.user_id = {id};
                                      """;
            
        var queryResult = await queryMapper.ExecuteAndReturnAsync<UpgradeUser>(sqlQuery, cancellationToken);
       
        return queryResult;
    }
    
    public async Task DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
        FormattableString sqlQuery = $"DELETE FROM users WHERE user_id = {id};";
        
        await queryMapper.ExecuteAsync(sqlQuery, cancellationToken);
    }
}