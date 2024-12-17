using System.Security.Authentication;
using ArtHub.Entities;
using ArtHub.Models;

namespace ArtHub.Services;

public class AuthService(DbContext dbContext)
{
    public async Task<User> RegisterUserAsync(User user, CancellationToken cancellationToken)
    {
        if (await dbContext.GetUserAsync(user.Login, cancellationToken) != null)
            throw new InvalidOperationException($"User with login {user.Login} already exists."); 
        
        var validator = new UserValidator();
        var validationResult = await  validator.ValidateAsync(user, cancellationToken);

        if (!validationResult.IsValid)
            throw new AuthenticationException("Invalid login or password.");

        user!.Password = MyPasswordHasher.HashPassword(user.Password);
        
        return await dbContext.CreateUserAsync(user.Login!, user.Password, cancellationToken);
    }

    public async Task<AuthResult> LoginUserAsync(UserLoginModel userModel, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userModel.Login) || string.IsNullOrWhiteSpace(userModel.Password))
            throw new ArgumentException("Login or Password cannot be null or empty");
        
        var user = await dbContext.GetUserAsync(userModel.Login, cancellationToken);
        
        if (user == null)
            throw new KeyNotFoundException($"User with login '{userModel.Login}' was not found.");;
                
        var validationResult = MyPasswordHasher.ValidatePassword(user!.Password!, userModel!.Password!);

        if (!validationResult)
            throw new AuthenticationException("Invalid password.");

        return new AuthResult
        {
            Token = JwtService.GenerateJwtToken(user)
        };
    }
}