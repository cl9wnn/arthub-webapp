using Application;
using Application.Interfaces;
using Application.Models;
using Application.Validators;
using Persistence.Entities;
using Persistence.interfaces;
using Persistence.Repositories;

namespace Application.Services;

public class UserService(IUserRepository userRepository, FileService fileService, IPasswordHasher passwordHasher)
{
    public async Task<Result<JwtTokenModel>> RegisterUserAsync(UserModel userModel, string fileData, string contentType,
        CancellationToken cancellationToken)
    {
        var avatarResult = await fileService.SaveFileAsync(fileData, contentType, "avatars", cancellationToken);

        if (!avatarResult.IsSuccess)
        {
            await fileService.DeleteFileAsync(avatarResult.Data, "avatars",cancellationToken);
            return Result<JwtTokenModel>.Failure(500, "Failed to save avatar: " + avatarResult.ErrorMessage)!;
        }

        if (await userRepository.GetUserAsyncByLogin(userModel.Login!, cancellationToken) != null)
        {
            await fileService.DeleteFileAsync(avatarResult.Data, "avatars",cancellationToken);
            return Result<JwtTokenModel>.Failure(409, $"User with login {userModel.Login} already exists.")!;
        }

        var validator = new UserValidator();
        var validationResult = await validator.ValidateAsync(userModel, cancellationToken);

        if (!validationResult.IsValid)
        {
            await fileService.DeleteFileAsync(avatarResult.Data, "avatars",cancellationToken);
            return Result<JwtTokenModel>.Failure(401, validationResult.ToString())!;
        }

        var user = new User
        {
            Login = userModel.Login,
            Password = passwordHasher.HashPassword(userModel.Password),
            ProfileName = userModel.ProfileName,
            Country = userModel.Country,
            AvatarPath = avatarResult.Data
        };

        var createdUser = await userRepository.CreateUserAsync(user, cancellationToken);

        var authResult = new JwtTokenModel
        {
            Token = JwtService.GenerateJwtToken(createdUser)
        };
        return Result<JwtTokenModel>.Success(authResult);
    }

    public async Task<Result<JwtTokenModel>> LoginUserAsync(string login, string password, CancellationToken cancellationToken=default)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            return Result<JwtTokenModel>.Failure(400, "Login or Password cannot be null or empty")!;

        var user = await userRepository.GetUserAsyncByLogin(login, cancellationToken);

        if (user == null)
            return Result<JwtTokenModel>.Failure(404, $"User with login '{login}' was not found.")!;

        var validationResult = passwordHasher.ValidatePassword(user!.Password!, password);

        if (!validationResult)
            return Result<JwtTokenModel>.Failure(401, "Invalid password.")!;

        var authResult = new JwtTokenModel
        {
            Token = JwtService.GenerateJwtToken(user)
        };

        return Result<JwtTokenModel>.Success(authResult);
    }
}