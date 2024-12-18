using System.Net;
using System.Security.Authentication;
using System.Text;
using ArtHub.Entities;
using ArtHub.Models;

namespace ArtHub.Services;

public class AuthService(DbContext dbContext)
{
    public async Task<Result<User>> RegisterUserAsync(User user, CancellationToken cancellationToken)
    {
        if (await dbContext.GetUserAsync(user.Login, cancellationToken) != null)
            return Result<User>.Failure(409,$"User with login {user.Login} already exists.")!; 
        
        var validator = new UserValidator();
        var validationResult = await  validator.ValidateAsync(user, cancellationToken);

        if (!validationResult.IsValid)
            return Result<User>.Failure(401, validationResult.ToString())!;

        user!.Password = MyPasswordHasher.HashPassword(user.Password);
        
        var createdUser = await dbContext.CreateUserAsync(user.Login!, user.Password, cancellationToken);
        return Result<User>.Success(createdUser);
    }

    public async Task<Result<AuthResult>> LoginUserAsync(UserLoginModel userModel, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userModel.Login) || string.IsNullOrWhiteSpace(userModel.Password))
            return Result<AuthResult>.Failure(400,"Login or Password cannot be null or empty")!;
        
        var user = await dbContext.GetUserAsync(userModel.Login, cancellationToken);
        
        if (user == null)
          return Result<AuthResult>.Failure(404, $"User with login '{userModel.Login}' was not found.")!;
                
        var validationResult = MyPasswordHasher.ValidatePassword(user!.Password!, userModel!.Password!);

        if (!validationResult)
            return Result<AuthResult>.Failure(401,"Invalid password.")!;
        
        var authResult =  new AuthResult
        {
            Token = JwtService.GenerateJwtToken(user)
        };
        
        return Result<AuthResult>.Success(authResult);
    }

    public Task<User?> AuthorizeUserAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        var authHeader = context.Request.Headers["Authorization"];
        
        if (authHeader == null)
        {
            return Task.FromResult<User?>(null);
        }
        var token = authHeader!.Split()[1];
        var tokenValidationResult = JwtService.ValidateJwtToken(token);
        
        return Task.FromResult(tokenValidationResult.isSuccess ? tokenValidationResult.user : null);
    }
}