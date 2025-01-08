using System.Net;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using BusinessLogic.Services;
using Persistence.Entities;
using WebAPI.Models;

namespace WebAPI.Controllers;

public class MarketController(MarketService marketService): MyBaseController
{
    [HttpGet("/market")]
    public IMyActionResult ShowMarket()
    {
        const string path = "public/MarketPage/index.html";
        return new ResourceResult(path);
    }

    [Authorize("user", "artist")]
    [HttpPost("/api/give-reward")]
    public async Task<IMyActionResult> GiveRewardToArtworkAsync([FromBody]RewardDto? reward, 
        HttpListenerContext context ,CancellationToken cancellationToken)
    {
        if (!context.TryGetItem<int>("userId", out var userId))
            return new ErrorResult(400, "Not authorized");
        
        if (reward == null)
            return new ErrorResult(400, "Bad request");
        
        var boughtRewardResult = await marketService.GiveRewardAsync(reward.RewardId, reward.ArtworkId,  userId, cancellationToken); 
       
        return boughtRewardResult.IsSuccess 
            ? new JsonResult<Reward>(boughtRewardResult.Data)
            : new ErrorResult(boughtRewardResult.StatusCode, boughtRewardResult.ErrorMessage!);

    }
    
    
    [HttpGet("/api/get-rewards")]
    public async Task<IMyActionResult> GetAllRewardsAsync(CancellationToken cancellationToken)
    {
        var rewardsResult = await marketService.GetRewardListAsync(cancellationToken);

        return rewardsResult.IsSuccess
            ? new JsonResult<List<Reward>>(rewardsResult.Data)
            : new ErrorResult(rewardsResult.StatusCode, rewardsResult.ErrorMessage!);
    }
}