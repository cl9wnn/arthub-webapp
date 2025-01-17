using System.Net;
using Application.Models;
using Application.Services;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using MyFramework.Views;
using Persistence.Entities;
using WebAPI.Models;

namespace WebAPI.Controllers;

public class ArtworkController(ArtworkService artworkService): MyBaseController
{
    [HttpGet("/new/artwork")]
    public IMyActionResult ShowAddArtworkPage()
    {
        const string path = "public/AddArtworkPage/index.html";
        return new ResourceResult(path);
    }

    [HttpGet("/artwork/{id}")]
    public IMyActionResult ShowArtworkPage()
    {
        const string path = "public/ArtworkPage/index.html";
        return new ResourceResult(path);
    }
    
    
    [Authorize("user","artist")]
    [HttpGet("/api/artwork-rewards/{artworkId}")]
    public async Task<IMyActionResult> GetArtworkRewardsAsync(int artworkId, HttpListenerContext context, CancellationToken cancellationToken)
    {
        var isArtworkExistsResult = await artworkService.IsArtworkExistsAsync(artworkId, cancellationToken);
        
        if (!isArtworkExistsResult.IsSuccess)
            return new ErrorResult(isArtworkExistsResult.StatusCode, isArtworkExistsResult.ErrorMessage!);

        if (!context.TryGetItem<int>("userId", out var visitorId))
            return new ErrorResult(400, "Not authorized");
        
        var artworkRewardsResult = await artworkService.GetArtworkRewardsAsync(artworkId, cancellationToken);
        
        return artworkRewardsResult.IsSuccess
            ? new JsonResult<List<ArtworkReward>>(artworkRewardsResult.Data)
            : new ErrorResult(artworkRewardsResult.StatusCode, artworkRewardsResult.ErrorMessage!);
    }
    
    [Authorize("user","artist")]
    [HttpGet("/api/artwork/{artworkId}")]
    public async Task<IMyActionResult> GetArtworkData(int artworkId, HttpListenerContext context, CancellationToken cancellationToken)
    {
        var isArtworkExistsResult = await artworkService.IsArtworkExistsAsync(artworkId, cancellationToken);
        
        if (!isArtworkExistsResult.IsSuccess)
            return new ErrorResult(isArtworkExistsResult.StatusCode, isArtworkExistsResult.ErrorMessage!);

        if (!context.TryGetItem<int>("userId", out var visitorId))
            return new ErrorResult(400, "Not authorized");
        
        var artworkResult = await artworkService.GetArtworkPostAsync(artworkId, visitorId, cancellationToken);
        
        return artworkResult.IsSuccess
            ? new JsonResult<ArtworkPostModel>(artworkResult.Data)
            : new ErrorResult(artworkResult.StatusCode, artworkResult.ErrorMessage!);
    }
    
    [Authorize("user","artist")]
    [HttpGet("/api/like-artwork/{artworkId}")]
    public async Task<IMyActionResult> LikeArtworkPost(int artworkId, HttpListenerContext context, CancellationToken cancellationToken)
    {
        if (!context.TryGetItem<int>("userId", out var userId))
            return new ErrorResult(400, "Not authorized");
        
        var likeResult = await artworkService.LikeArtworkAsync(artworkId,  userId, cancellationToken);
        
        return likeResult.IsSuccess
            ? new JsonResult<int>(likeResult.Data)
            : new ErrorResult(likeResult.StatusCode, likeResult.ErrorMessage!);
    }
    
    [Authorize("artist")]
    [HttpPost("/api/add-artwork")]
    public async Task<IMyActionResult> AddArtwork([FromBody] ArtworkRequest? artworkRequest,
        HttpListenerContext context, CancellationToken cancellationToken)
    {
        if (!context.TryGetItem<int>("userId", out var userId))
            return new ErrorResult(400, "Not authorized");
        
        if (artworkRequest == null || artworkRequest.ArtFile == null)
            return new ErrorResult(400, "Invalid request");

        var artworkModel = new ArtworkModel
        {
            Title = artworkRequest.Title,
            Category = artworkRequest.Category,
            Description = artworkRequest.Description,
            Tags = artworkRequest.Tags,
            UserId = userId
        };
        var artFile = artworkRequest.ArtFile;
        
        var artResult = await artworkService.SaveArtAsync(artworkModel, artFile.FileData!, artFile.ContentType!, cancellationToken);

        return artResult.IsSuccess
            ? new Ok()
            : new ErrorResult(artResult.StatusCode, artResult.ErrorMessage!);
    }
   
    
    [Authorize("artist")]
    [HttpDelete("api/delete-own-artwork")]
    public async Task<IMyActionResult> DeleteOwnArtwork([FromBody] int artworkId, HttpListenerContext context, CancellationToken cancellationToken)
    {
        if (!context.TryGetItem<int>("userId", out var userId))
            return new ErrorResult(400, "Not authorized");
        
        var isArtworkOwnedResult = await artworkService.IsArtworkOwnedByUserAsync(artworkId,  userId, cancellationToken);
        
        if (!isArtworkOwnedResult.IsSuccess)
            return new ErrorResult(isArtworkOwnedResult.StatusCode, isArtworkOwnedResult.ErrorMessage!);
        
        var deletedResult  = await artworkService.DeleteOwnArtworkAsync(userId, artworkId, cancellationToken);
        
        return deletedResult.IsSuccess
            ? new Ok()
            : new ErrorResult(deletedResult.StatusCode, deletedResult.ErrorMessage!);
    }
}