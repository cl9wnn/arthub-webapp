﻿using Persistence;
using Persistence.Entities;
using Persistence.Repositories;
using WebAPI.Models;

namespace WebAPI.Services;

public class AccountService(UserRepository userRepository)
{
    public async Task<Result<JwtTokenModel>> RegisterUserAsync(User user, CancellationToken cancellationToken)
    {
        if (await userRepository.GetUserAsync(user.Login, cancellationToken) != null)
            return Result<JwtTokenModel>.Failure(409,$"User with login {user.Login} already exists.")!; 
        
        var validator = new UserValidator();
        var validationResult = await  validator.ValidateAsync(user, cancellationToken);

        if (!validationResult.IsValid)
            return Result<JwtTokenModel>.Failure(401, validationResult.ToString())!;

        user!.Password = MyPasswordHasher.HashPassword(user.Password);
        
        var createdUser = await userRepository.CreateUserAsync(user, cancellationToken);
        
        var authResult =  new JwtTokenModel
        {
            Token = JwtService.GenerateJwtToken(createdUser)
        };
        return Result<JwtTokenModel>.Success(authResult);
    }

    public async Task<Result<JwtTokenModel>> LoginUserAsync(LoginModel model, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(model.Login) || string.IsNullOrWhiteSpace(model.Password))
            return Result<JwtTokenModel>.Failure(400,"Login or Password cannot be null or empty")!;
        
        var user = await userRepository.GetUserAsync(model.Login, cancellationToken);
        
        if (user == null)
            return Result<JwtTokenModel>.Failure(404, $"User with login '{model.Login}' was not found.")!;
                
        var validationResult = MyPasswordHasher.ValidatePassword(user!.Password!, model!.Password!);

        if (!validationResult)
            return Result<JwtTokenModel>.Failure(401,"Invalid password.")!;
        
        var authResult =  new JwtTokenModel
        {
            Token = JwtService.GenerateJwtToken(user)
        };
        
        return Result<JwtTokenModel>.Success(authResult);
    }
}