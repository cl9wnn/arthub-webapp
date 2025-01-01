using System.Net;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using Persistence.Entities;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers;

public class AuthController(UserService userService, FileService fileService) : MyBaseController
{
    
    [HttpPost("/auth/signup")]
    public async Task<IMyActionResult> Register([FromBody] SignUpModel? signUpModel, CancellationToken cancellationToken)
    {
        if (signUpModel == null)
            return new ErrorResult(400, "Invalid request");
        
        var avatarResult = await fileService.SaveFileAsync(signUpModel!.Avatar!, "avatars", cancellationToken);
        
        if (!avatarResult.IsSuccess)
            return new ErrorResult(500, "Failed to save avatar: " + avatarResult.ErrorMessage);
        
        signUpModel!.User!.AvatarPath = avatarResult.Data;
        
        var userResult = await userService.RegisterUserAsync(signUpModel!.User!, cancellationToken);
        
        if (!userResult.IsSuccess)
        {
            var deleteResult = await fileService.DeleteFileAsync(avatarResult.Data, "avatars",cancellationToken);
            return !deleteResult.IsSuccess 
                ? new ErrorResult(deleteResult.StatusCode, deleteResult.ErrorMessage!)
                : new ErrorResult(userResult.StatusCode, userResult.ErrorMessage!);
        }
        return new JsonResult<JwtTokenModel>(userResult.Data);
    }
    

    [HttpPost("/auth/signin")]
    public async Task<IMyActionResult> Login([FromBody] LoginModel? loginModel, CancellationToken cancellationToken)
    {
        if (loginModel == null)
            return new ErrorResult(400, "Invalid request");
        
        var result = await userService.LoginUserAsync(loginModel!, cancellationToken);

        return result.IsSuccess
            ? new JsonResult<JwtTokenModel>(result!.Data)
            : new ErrorResult(result.StatusCode, result.ErrorMessage!);
    }
}