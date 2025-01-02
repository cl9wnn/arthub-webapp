using System.Net;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using BusinessLogic.Services;

namespace WebAPI.Controllers;

public class MarketController: MyBaseController
{
    [HttpGet("/market")]
    public IMyActionResult ShowMarket()
    {
        const string path = "public/MarketPage/index.html";
        return new ResourceResult(path);
    }

    [Authorize("user", "artist")]
    [HttpPost("/api/buy")]
    public IMyActionResult TestBuy()
    {
        return new JsonResult<string>("Удачная покупка (вы авторизованы)");
    }
}