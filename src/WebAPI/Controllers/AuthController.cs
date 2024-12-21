using System.Net;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using Persistence.Entities;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers;

public class AuthController(AuthService authService) : BaseController
{
    
    [Route("/auth/signup", "POST")]
    public async Task<IActionResult> Register(HttpListenerContext context, CancellationToken cancellationToken)
    {
        var userModel = await WebHelper.ReadBodyAsync<User>(context, cancellationToken);

        var result = await authService.RegisterUserAsync(userModel!, cancellationToken);

        return result.IsSuccess
            ? new JsonResult<AuthToken>(result!.Data)
            : new ErrorResult(result.StatusCode, result.ErrorMessage!);
    }

    [Route("/auth/signin", "POST")]
    public async Task<IActionResult> Login(HttpListenerContext context, CancellationToken cancellationToken)
    {
        var userModel = await WebHelper.ReadBodyAsync<UserLoginModel>(context, cancellationToken);

        var result = await authService.LoginUserAsync(userModel!, cancellationToken);

        return result.IsSuccess
            ? new JsonResult<AuthToken>(result!.Data)
            : new ErrorResult(result.StatusCode, result.ErrorMessage!);
    }
}