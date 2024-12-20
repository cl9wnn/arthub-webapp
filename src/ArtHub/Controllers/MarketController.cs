using System.Net;
using System.Security.Authentication;
using ArtHub.Attributes;
using ArtHub.Services;

namespace ArtHub.Controllers;

public class MarketController
{
    [Authorize("user")]
    [Route("/market", "GET")]
    public async Task LoadMarket(HttpListenerContext context, CancellationToken token)
    {
        try
        {
            await WebHelper.WriteJsonAsync("OK", context, token);
        }
        catch (AuthenticationException ex)
        {
            await WebHelper.ShowError(401, ex.Message, context, token);
        }
    }
}