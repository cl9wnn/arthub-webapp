using System.Net;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using Persistence.Entities;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers;

public class AuthController(AccountService accountService, AvatarService avatarService) : MyBaseController
{
    
    [HttpPost("/auth/signup")]
    public async Task<IMyActionResult> Register(HttpListenerContext context, CancellationToken cancellationToken)
    {
        var userModel = await WebHelper.ReadBodyAsync<SignUpModel>(context, cancellationToken);
        
        if (userModel == null)
            return new ErrorResult(400, "Invalid request");
        
        var avatarResult = await avatarService.SaveAvatarAsync(userModel!.Avatar!, cancellationToken);
        
        if (!avatarResult.IsSuccess)
            return new ErrorResult(500, "Failed to save avatar: " + avatarResult.ErrorMessage);
        
        userModel!.User!.Avatar = avatarResult.Data;
        
        var userResult = await accountService.RegisterUserAsync(userModel!.User!, cancellationToken);
        
          return userResult.IsSuccess
               ? new JsonResult<JwtTokenModel>(userResult!.Data)
               : new ErrorResult(userResult.StatusCode, userResult.ErrorMessage!);
    }
    

    [HttpPost("/auth/signin")]
    public async Task<IMyActionResult> Login(HttpListenerContext context, CancellationToken cancellationToken)
    {
        var userModel = await WebHelper.ReadBodyAsync<LoginModel>(context, cancellationToken);

        var result = await accountService.LoginUserAsync(userModel!, cancellationToken);

        return result.IsSuccess
            ? new JsonResult<JwtTokenModel>(result!.Data)
            : new ErrorResult(result.StatusCode, result.ErrorMessage!);
    }
}