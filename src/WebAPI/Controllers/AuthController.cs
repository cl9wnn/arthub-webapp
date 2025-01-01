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
    public async Task<IMyActionResult> Register(HttpListenerContext context, CancellationToken cancellationToken)
    {
        var userModel = await WebHelper.ReadBodyAsync<SignUpModel>(context, cancellationToken);
        
        if (userModel == null)
            return new ErrorResult(400, "Invalid request");
        
        var avatarResult = await fileService.SaveFileAsync(userModel!.Avatar!, "avatars", cancellationToken);
        
        if (!avatarResult.IsSuccess)
            return new ErrorResult(500, "Failed to save avatar: " + avatarResult.ErrorMessage);
        
        userModel!.User!.Avatar = avatarResult.Data;
        
        var userResult = await userService.RegisterUserAsync(userModel!.User!, cancellationToken);
        
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
    public async Task<IMyActionResult> Login(HttpListenerContext context, CancellationToken cancellationToken)
    {
        var userModel = await WebHelper.ReadBodyAsync<LoginModel>(context, cancellationToken);

        var result = await userService.LoginUserAsync(userModel!, cancellationToken);

        return result.IsSuccess
            ? new JsonResult<JwtTokenModel>(result!.Data)
            : new ErrorResult(result.StatusCode, result.ErrorMessage!);
    }
}