using System.Net;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using BusinessLogic.Services;
using MyFramework.Views;
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
            ? new JsonResult<ArtworkReward>(boughtRewardResult.Data)
            : new ErrorResult(boughtRewardResult.StatusCode, boughtRewardResult.ErrorMessage!);
    }
    
    [Authorize("artist")]
    [HttpPost("/api/buy-decoration")]
    public async Task<IMyActionResult> BuyItemAsync([FromBody]int decorationId, HttpListenerContext context, CancellationToken cancellationToken)
    {
        if (!context.TryGetItem<int>("userId", out var userId))
            return new ErrorResult(400, "Not authorized");
        
        if (decorationId == 0)
            return new ErrorResult(400, "Bad request");
        
        var isUserHasDecorationResult = await marketService.CheckDecorationExistenceAsync(decorationId, userId, cancellationToken);

        if (!isUserHasDecorationResult.Data)
            return new ErrorResult(isUserHasDecorationResult.StatusCode, isUserHasDecorationResult.ErrorMessage!);
        
        var boughtDecorationResult = await marketService.BuyDecorationAsync(decorationId,  userId, cancellationToken);
        
        return boughtDecorationResult.IsSuccess
            ? new Ok()
            : new ErrorResult(boughtDecorationResult.StatusCode, boughtDecorationResult.ErrorMessage!);
    }
    
    [HttpGet("/api/get-rewards")]
    public async Task<IMyActionResult> GetAllRewardsAsync(CancellationToken cancellationToken)
    {
        var rewardsResult = await marketService.GetRewardListAsync(cancellationToken);

        return rewardsResult.IsSuccess
            ? new JsonResult<List<Reward>>(rewardsResult.Data)
            : new ErrorResult(rewardsResult.StatusCode, rewardsResult.ErrorMessage!);
    }
    
    [Authorize("user", "artist")]
    [HttpGet("/api/get-balance")]
    public async Task<IMyActionResult> GetUserBalanceAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        if (!context.TryGetItem<int>("userId", out var userId))
            return new ErrorResult(400, "Not authorized");
        
        var balanceResult = await marketService.GetUserBalanceByIdAsync(userId, cancellationToken);

        return balanceResult.IsSuccess
            ? new JsonResult<int>(balanceResult.Data)
            : new ErrorResult(balanceResult.StatusCode, balanceResult.ErrorMessage!);
    }
    
    [Authorize("artist")]
    [HttpGet("/api/give-decorations")]
    public async Task<IMyActionResult> GetDecorationsAsync(CancellationToken cancellationToken)
    {
        var decorationsResult = await marketService.GetDecorationListAsync(cancellationToken);
        
        return decorationsResult.IsSuccess
            ? new JsonResult<List<Decoration>>(decorationsResult.Data)
            : new ErrorResult(decorationsResult.StatusCode, decorationsResult.ErrorMessage!);
    }
    
}