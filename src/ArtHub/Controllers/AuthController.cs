using System.Net;
using ArtHub.Entities;
using ArtHub.Models;
using ArtHub.Services;

namespace ArtHub.Controllers;

public class AuthController(AuthService authService)
{
    
    [Route("/auth/signup", "POST")]
    public async Task Register(HttpListenerContext context, CancellationToken cancellationToken)
    {
            var userModel = await WebHelper.ReadBodyAsync<User>(context, cancellationToken);
            
            var result = await authService.RegisterUserAsync(userModel!, cancellationToken);
            
            await (result.IsSuccess
                ? WebHelper.WriteJsonAsync(result.Data, context, cancellationToken)
                : WebHelper.ShowError(result.StatusCode, result.ErrorMessage!, context, cancellationToken));
    }

    [Route("/auth/signin", "POST")]
    public async Task Login(HttpListenerContext context, CancellationToken cancellationToken)
    {
            var userModel = await WebHelper.ReadBodyAsync<UserLoginModel>(context, cancellationToken);

            var result = await authService.LoginUserAsync(userModel!, cancellationToken);

            await (result.IsSuccess
                ? WebHelper.WriteJsonAsync(result.Data, context, cancellationToken)
                : WebHelper.ShowError(result.StatusCode, result.ErrorMessage!, context, cancellationToken));
    }
}