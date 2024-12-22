using System.Net;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using Persistence.Entities;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers;

public class AuthController(AccountService accountService) : MyBaseController
{
    
    [HttpPost("/auth/signup")]
    public async Task<IMyActionResult> Register(HttpListenerContext context, CancellationToken cancellationToken)
    {
        var userModel = await WebHelper.ReadBodyAsync<User>(context, cancellationToken);

        var result = await accountService.RegisterUserAsync(userModel!, cancellationToken);

        return result.IsSuccess
            ? new JsonResult<AuthToken>(result!.Data)
            : new ErrorResult(result.StatusCode, result.ErrorMessage!);
    }

    [HttpPost("/auth/signin")]
    public async Task<IMyActionResult> Login(HttpListenerContext context, CancellationToken cancellationToken)
    {
        var userModel = await WebHelper.ReadBodyAsync<UserLoginModel>(context, cancellationToken);

        var result = await accountService.LoginUserAsync(userModel!, cancellationToken);

        return result.IsSuccess
            ? new JsonResult<AuthToken>(result!.Data)
            : new ErrorResult(result.StatusCode, result.ErrorMessage!);
    }
}