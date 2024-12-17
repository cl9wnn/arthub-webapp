using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using ArtHub.Entities;
using ArtHub.Handlers;
using ArtHub.Models;
using ArtHub.Services;

namespace ArtHub.Controllers;

public class AuthController(AuthService authService)
{
    
    [Route("/auth/signup", "POST")]
    public async Task Register(HttpListenerContext context, CancellationToken cancellationToken)
    {
        try
        {
            var userModel = await WebHelper.ReadBodyAsync<User>(context, cancellationToken);
            
            var registeredUser = await authService.RegisterUserAsync(userModel!, cancellationToken);
            
            await WebHelper.WriteJsonAsync(registeredUser, context, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            await WebHelper.ShowError(409, ex.Message, context, cancellationToken);
        }
        catch (AuthenticationException ex)
        {
            await WebHelper.ShowError(401, ex.Message, context, cancellationToken);
        }
    }

    [Route("/auth/signin", "POST")]
    public async Task Login(HttpListenerContext context, CancellationToken cancellationToken)
    {
        try
        {
            var userModel = await WebHelper.ReadBodyAsync<UserLoginModel>(context, cancellationToken);

            var authUser = await authService.LoginUserAsync(userModel!, cancellationToken);

            await WebHelper.WriteJsonAsync(authUser, context, cancellationToken);
        }
        catch (ArgumentException ex)
        {
            await WebHelper.ShowError(400, ex.Message, context, cancellationToken);
        }
        catch (KeyNotFoundException ex)
        {
            await WebHelper.ShowError(404, ex.Message, context, cancellationToken);
        }
        catch (AuthenticationException ex)
        {
            await WebHelper.ShowError(401, ex.Message, context, cancellationToken);
        }
    }
}