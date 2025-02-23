using System.Net;
using Application.Models;
using Application.Services;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using MyFramework.Views;
using Persistence.Entities;

namespace WebAPI.Controllers;

public class SavingsController(SavingFavouriteService savingFavouriteService): MyBaseController
{
    [Authorize("user","artist")]
    [HttpGet("/api/get-savings")]
    public async Task<IMyActionResult> GetSavings(HttpListenerContext context, CancellationToken cancellationToken)
    {
        if (!context.TryGetItem<int>("userId", out var userId))
            return new ErrorResult(400, "Not authorized");
        
        var savingListResult = await savingFavouriteService.GetSavingFavouritesAsync(userId, cancellationToken);
        
        return savingListResult.IsSuccess 
            ? new JsonResult<List<SavingArtModel>>(savingListResult.Data)
            : new ErrorResult(savingListResult.StatusCode, savingListResult.ErrorMessage!);
    }
    
    [Authorize("user","artist")]
    [HttpGet("/api/save-artwork/{artworkId}")]
    public async Task<IMyActionResult> SaveArtworkPost(int artworkId, HttpListenerContext context, CancellationToken cancellationToken)
    {
        if (!context.TryGetItem<int>("userId", out var userId))
            return new ErrorResult(400, "Not authorized");
        
        var saveResult = await savingFavouriteService.SaveFavouriteArtAsync(artworkId, userId, cancellationToken);
        
        return saveResult.IsSuccess
            ? new JsonResult<SavingArt>(saveResult.Data)
            : new ErrorResult(saveResult.StatusCode, saveResult.ErrorMessage!);
    }
    
    [Authorize("user","artist")]
    [HttpDelete("/api/delete-saving")]
    public async Task<IMyActionResult> DeleteSavingArt([FromBody] int artworkId, HttpListenerContext context, CancellationToken cancellationToken)
    {
        if (!context.TryGetItem<int>("userId", out var userId))
            return new ErrorResult(400, "Not authorized");
        
        var deletedResult  = await savingFavouriteService.DeleteSavingFavouriteAsync(userId, artworkId, cancellationToken);
        
        return deletedResult.IsSuccess
            ? new Ok()
            : new ErrorResult(deletedResult.StatusCode, deletedResult.ErrorMessage!);
    }
}