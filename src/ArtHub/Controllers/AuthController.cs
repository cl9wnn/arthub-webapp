using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using ArtHub.Entities;
using ArtHub.Services;

namespace ArtHub.Controllers;

public class AuthController(AuthService authService)
{
    
    [Route("/auth/signup", "POST")]
    public async Task Register(HttpListenerContext context, CancellationToken cancellationToken)
    {
        try
        {
            using var sr = new StreamReader(context.Request.InputStream);
            var userModel = JsonSerializer.Deserialize<User>(
                await sr.ReadToEndAsync(cancellationToken).ConfigureAwait(false),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            var registeredUser = await authService.RegisterUserAsync(userModel!, cancellationToken);

            await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(
                    JsonSerializer.Serialize(registeredUser,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        })),
                cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            await ErrorHandler.ShowError(409, ex.Message, context, cancellationToken);
        }
        catch (AuthenticationException ex)
        {
            await ErrorHandler.ShowError(401, ex.Message, context, cancellationToken);
        }
    }
}