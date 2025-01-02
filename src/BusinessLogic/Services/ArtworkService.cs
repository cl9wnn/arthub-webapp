using BusinessLogic.Models;
using BusinessLogic.Validators;
using Persistence.Entities;
using Persistence.Repositories;
using WebAPI.Models;

namespace BusinessLogic.Services;

public class ArtworkService(FileService fileService, ArtworkRepository artworkRepository, UserRepository userRepository)
{
    public async Task<Result<Artwork>> SaveArtAsync(ArtworkModel artworkModel,  string fileData, string contentType,
        CancellationToken cancellationToken)
    {
        var saveFileResult = await fileService.SaveFileAsync(fileData, contentType,"arts", cancellationToken);

        if (!saveFileResult.IsSuccess)
            return Result<Artwork>.Failure(saveFileResult.StatusCode, saveFileResult.ErrorMessage!)!;
        
        var validator = new ArtworkValidator();
        var validationResult = await validator.ValidateAsync(artworkModel, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            await fileService.DeleteFileAsync(saveFileResult.Data, "arts", cancellationToken);
            return Result<Artwork>.Failure(400, validationResult.ToString())!;
        }
        
        var artwork = new Artwork
        {
            Title = artworkModel.Title,
            Category = artworkModel.Category,
            Description = artworkModel.Description,
            ArtworkPath = saveFileResult.Data,
            UserId = artworkModel.UserId,
        };
      
        var createdArt = await artworkRepository.InsertArtworkAsync(artwork, artworkModel.Tags!, cancellationToken);
        
        return createdArt == null
            ? Result<Artwork>.Failure(400, "Could not save artwork")!
            : Result<Artwork>.Success(createdArt);
    }

    public async Task<Result<List<GalleryArtwork>>> GetArtworksInfoAsync(CancellationToken cancellationToken)
    {
        var responseArtworks = new List<GalleryArtwork>();
        
        var artworks = await artworkRepository.GetArtworksWithUserDetailsAsync(cancellationToken);
        
        if (artworks == null)
            return Result<List<GalleryArtwork>>.Failure(400, "Could not retrieve artworks")!;
        
        foreach (var artwork in artworks)
        {
            var author = await userRepository.GetUserAsyncById(artwork.UserId, cancellationToken);

            var artModel = new GalleryArtwork
            {
                Title = artwork.Title,
                ArtworkPath = artwork.ArtworkPath,
                ProfileName = author!.ProfileName,
                AvatarPath = author.AvatarPath,
            };
            responseArtworks.Add(artModel);
        }
        
        return Result<List<GalleryArtwork>>.Success(responseArtworks)!;
    }
}
    