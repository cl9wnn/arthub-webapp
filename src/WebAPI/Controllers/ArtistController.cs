using System.Net;
using Application.Models;
using Application.Services;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using WebAPI.Models;
namespace WebAPI.Controllers;

public class ArtistController(ArtistService artistService) : MyBaseController
{
    [Authorize("user")]
    [HttpPost("/api/upgrade-user")]
    public async Task<IMyActionResult> RegisterArtistAsync([FromBody] ArtistSignupDto? artistRequest,
        HttpListenerContext context, CancellationToken cancellationToken)
    {
        if (!context.TryGetItem<int>("userId", out var userId))
            return new ErrorResult(400, "Not authorized");
        
        var isUserUpgraded = await artistService.CheckProfileForUpgrade(userId, cancellationToken);
        
        if (isUserUpgraded.Data)
            return new ErrorResult(400, "User already upgraded");
        
        if (artistRequest == null)
            return new ErrorResult(400, "Invalid request");

        var artistModel = new ArtistModel
        {
            Fullname = artistRequest.Fullname,
            ContactInfo = artistRequest.ContactInfo,
            Summary = artistRequest.Summary,
            UserId = userId
        };
        
        var userResult = await artistService.RegisterArtistAsync(artistModel, cancellationToken);
        
        return userResult.IsSuccess 
            ?  new JsonResult<JwtTokenModel>(userResult.Data)
            :  new ErrorResult(userResult.StatusCode, userResult.ErrorMessage!);
    }
}