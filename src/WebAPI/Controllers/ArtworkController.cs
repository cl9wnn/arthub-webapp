using System.Net;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using Persistence.Entities;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers;

public class ArtworkController(ArtworkService artworkService, FileService fileService): MyBaseController
{
    [HttpGet("/new/artwork")]
    public IMyActionResult ShowAddArtworkPage(HttpListenerContext context, CancellationToken cancellationToken)
    {
        const string path = "public/AddArtworkPage/index.html";
        return new ResourceResult(path);
    }
    
    [Authorize("artist")]
    [HttpPost("/api/add-artwork")]
    public async Task<IMyActionResult> AddArtworkAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        if (!context.TryGetItem<int>("userId", out var userId))
            return new ErrorResult(400, "Not authorized");
        
        var artModel = await WebHelper.ReadBodyAsync<ArtworkModel>(context, cancellationToken);
        
        if (artModel == null)
            return new ErrorResult(400, "Invalid request");
        
        var artResult = await artworkService.SaveArtAsync(artModel, userId, cancellationToken);
        
        if (!artResult.IsSuccess)
        {
            var deleteResult = await fileService.DeleteFileAsync(artResult.Data!.ArtworkPath!, "arts", cancellationToken);
            return !deleteResult.IsSuccess 
                ? new ErrorResult(deleteResult.StatusCode, deleteResult.ErrorMessage!)
                : new ErrorResult(artResult.StatusCode, artResult.ErrorMessage!);
        }
        return new JsonResult<string>("success");
    }
}