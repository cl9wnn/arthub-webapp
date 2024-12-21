using System.Net;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;

namespace WebAPI.Controllers;

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