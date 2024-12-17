using System.Net;
using System.Security.Authentication;
using ArtHub.Services;

namespace ArtHub.Controllers;

public class TestController(AuthService authService)
{
    [Route("/test", "GET")]
    public async Task TestAuth(HttpListenerContext context, CancellationToken token)
    {
        try
        {
            var auth = await authService.AuthorizeUserAsync(context, token);
        }
        catch (AuthenticationException ex)
        {
            await WebHelper.ShowError(401, ex.Message, context, token);
        }
    }
}