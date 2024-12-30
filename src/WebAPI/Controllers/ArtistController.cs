using System.Net;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers;

public class ArtistController(ArtistService artistService) : MyBaseController
{
    [Authorize("user")]
    [HttpPost("/api/upgrade-user")]
    public async Task<IMyActionResult> RegisterArtistAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        if (!context.TryGetItem<int>("userId", out var userId))
            return new ErrorResult(400, "Not authorized");
        
        var artistModel = await WebHelper.ReadBodyAsync<SignUpArtistModel>(context, cancellationToken);
        
        if (artistModel == null)
            return new ErrorResult(400, "Invalid request");
        
        var userResult = await artistService.RegisterArtistAsync(artistModel, userId, cancellationToken);
        
        return userResult.IsSuccess 
            ?  new JsonResult<JwtTokenModel>(userResult.Data)
            :  new ErrorResult(userResult.StatusCode, userResult.ErrorMessage!);
    }
}