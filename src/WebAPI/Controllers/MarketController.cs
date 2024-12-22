using System.Net;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;

namespace WebAPI.Controllers;

public class MarketController: MyBaseController
{
    [Authorize("user")]
    [HttpGet("/market")]
    public IMyActionResult ShowMarketPage(HttpListenerContext context, CancellationToken cancellationToken)
    {
        const string path = "public/MarketPage/index.html";
        return new JsonResult<string>(path);
    }
}