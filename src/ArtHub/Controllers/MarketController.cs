using System.Net;
using System.Security.Authentication;
using ArtHub.Attributes;
using ArtHub.Services;

namespace ArtHub.Controllers;

public class MarketController: BaseController
{
    [Authorize("user")]
    [Route("/market", "GET")]
    public IActionResult ShowMarketPage(HttpListenerContext context, CancellationToken cancellationToken)
    {
        const string path = "public/MarketPage/index.html";
        return new JsonResult<string>(path);
    }
}