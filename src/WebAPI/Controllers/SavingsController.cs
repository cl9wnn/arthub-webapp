using System.Net;
using BusinessLogic.Models;
using BusinessLogic.Services;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using MyFramework.Views;

namespace WebAPI.Controllers;

public class SavingsController(ArtworkService artworkService): MyBaseController
{
    [HttpGet("/savings")]
    public IMyActionResult ShowMarket()
    {
        const string path = "public/SavingsPage/index.html";
        return new ResourceResult(path);
    }

    [Authorize("user","artist")]
    [HttpGet("/api/get-savings")]
    public async Task<IMyActionResult> GetSavings(HttpListenerContext context, CancellationToken cancellationToken)
    {
        if (!context.TryGetItem<int>("userId", out var userId))
            return new ErrorResult(400, "Not authorized");
        
        var savingListResult = await artworkService.GetSavingArtworksAsync(userId, cancellationToken);
        
        return savingListResult.IsSuccess 
            ? new JsonResult<List<SavingArtModel>>(savingListResult.Data)
            : new ErrorResult(savingListResult.StatusCode, savingListResult.ErrorMessage!);
    }
    
    [Authorize("user","artist")]
    [HttpDelete("/api/delete-saving")]
    public async Task<IMyActionResult> DeleteSaving([FromBody] int artworkId, HttpListenerContext context, CancellationToken cancellationToken)
    {
        if (!context.TryGetItem<int>("userId", out var userId))
            return new ErrorResult(400, "Not authorized");
        
        var deletedResult  = await artworkService.DeleteSavingArt(userId, artworkId, cancellationToken);
        
        return deletedResult.IsSuccess
            ? new Ok()
            : new ErrorResult(deletedResult.StatusCode, deletedResult.ErrorMessage!);
    }
}