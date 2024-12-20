﻿using System.Net;
using Persistence;
using Persistence.Entities;
using WebAPI.Models;

namespace WebAPI.Services;

public class AuthService(DbContext dbContext)
{
    public async Task<Result<AuthToken>> RegisterUserAsync(User user, CancellationToken cancellationToken)
    {
        if (await dbContext.GetUserAsync(user.Login, cancellationToken) != null)
            return Result<AuthToken>.Failure(409,$"User with login {user.Login} already exists.")!; 
        
        var validator = new UserValidator();
        var validationResult = await  validator.ValidateAsync(user, cancellationToken);

        if (!validationResult.IsValid)
            return Result<AuthToken>.Failure(401, validationResult.ToString())!;

        user!.Password = MyPasswordHasher.HashPassword(user.Password);
        
        var createdUser = await dbContext.CreateUserAsync(user.Login!, user.Password, cancellationToken);
        
        var authResult =  new AuthToken
        {
            Token = JwtService.GenerateJwtToken(createdUser)
        };
        return Result<AuthToken>.Success(authResult);
    }

    public async Task<Result<AuthToken>> LoginUserAsync(UserLoginModel userModel, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userModel.Login) || string.IsNullOrWhiteSpace(userModel.Password))
            return Result<AuthToken>.Failure(400,"Login or Password cannot be null or empty")!;
        
        var user = await dbContext.GetUserAsync(userModel.Login, cancellationToken);
        
        if (user == null)
          return Result<AuthToken>.Failure(404, $"User with login '{userModel.Login}' was not found.")!;
                
        var validationResult = MyPasswordHasher.ValidatePassword(user!.Password!, userModel!.Password!);

        if (!validationResult)
            return Result<AuthToken>.Failure(401,"Invalid password.")!;
        
        var authResult =  new AuthToken
        {
            Token = JwtService.GenerateJwtToken(user)
        };
        
        return Result<AuthToken>.Success(authResult);
    }

    public Task<User?> AuthorizeUserAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        var authHeader = context.Request.Headers["Authorization"];
        
        if (authHeader == null)
        {
            return null;
        }
        var token = authHeader!.Split()[1];
        var tokenValidationResult = JwtService.ValidateJwtToken(token);
        
        return Task.FromResult(tokenValidationResult.isSuccess ? tokenValidationResult.user : null);
    }
}