using System.Security.Authentication;
using ArtHub.Entities;

namespace ArtHub.Services;

public class AuthService(DbContext dbContext)
{
    public async Task<User> RegisterUserAsync(User user, CancellationToken cancellationToken)
    {
        if (await dbContext.GetUserAsync(user.Login, cancellationToken) != null)
        {
            throw new InvalidOperationException($"User with login {user.Login} already exists."); 
        }
        
        var validator = new UserValidator();
        var validationResult = await  validator.ValidateAsync(user, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new AuthenticationException("Invalid login or password.");
        }
        user!.Password = MyPasswordHasher.HashPassword(user.Password);
        
        return await dbContext.CreateUserAsync(user.Login!, user.Password, cancellationToken);
    }
}