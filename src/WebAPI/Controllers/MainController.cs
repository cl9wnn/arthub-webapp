using BusinessLogic.Models;
using BusinessLogic.Services;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using MyFramework.Views;
using WebAPI.Models;


namespace WebAPI.Controllers;

public class MainController(ArtworkService artworkService): MyBaseController
{
    [HttpGet("/api/get-artworks")]
    public async Task<IMyActionResult> GetArtworksForGalleryAsync(CancellationToken cancellationToken)
    {
        var artworksResult = await artworkService.GetArtworksInfoAsync(cancellationToken);

        return artworksResult.IsSuccess
            ? new JsonResult<List<GalleryArtworkModel>>(artworksResult.Data)
            : new ErrorResult(artworksResult.StatusCode, artworksResult.ErrorMessage!);
    }
}