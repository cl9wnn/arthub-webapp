using System.Net;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using WebAPI.Services;

namespace WebAPI.Controllers;

public class MarketController: MyBaseController
{
    [HttpGet("/market")]
    public IMyActionResult ShowMarketPage(HttpListenerContext context, CancellationToken cancellationToken)
    {
        const string path = "public/MarketPage/index.html";
        return new ResourceResult(path);
    }

    [Authorize("user", "artist")]
    [HttpPost("/api/buy")]
    public IMyActionResult TestBuy(HttpListenerContext context, CancellationToken cancellationToken)
    {
        return new JsonResult<string>("Удачная покупка (вы авторизованы)");
    }
}