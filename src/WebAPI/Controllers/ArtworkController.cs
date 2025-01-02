using System.Net;
using BusinessLogic.Models;
using BusinessLogic.Services;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using MyFramework.Views;
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
    
    
    [Authorize("artist")]
    [HttpPost("/api/add-artwork")]
    public async Task<IMyActionResult> AddArtworkAsync([FromBody] ArtworkRequest? artworkRequest,
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
}