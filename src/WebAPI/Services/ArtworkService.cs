using Persistence.Entities;
using Persistence.Repositories;
using WebAPI.Models;
using WebAPI.Validators;

namespace WebAPI.Services;

public class ArtworkService(FileService service, ArtworkRepository artworkRepository, UserRepository userRepository)
{
    public async Task<Result<Artwork>> SaveArtAsync(RequestArtworkModel model, int userId, CancellationToken cancellationToken)
    {
        var saveFileResult = await service.SaveFileAsync(model.ArtFile, "arts", cancellationToken);

        if (!saveFileResult.IsSuccess)
            return Result<Artwork>.Failure(saveFileResult.StatusCode, saveFileResult.ErrorMessage!)!;

        var artwork = new Artwork
        {
            Title = model.Title,
            Category = model.Category,
            Description = model.Description,
            ArtworkPath = saveFileResult.Data,
            UserId = userId
        };
        
        var validator = new ArtworkValidator();
        var validationResult = await validator.ValidateAsync(artwork, cancellationToken);

        if (!validationResult.IsValid)
            return Result<Artwork>.Failure(400, validationResult.ToString())!;

        var createdArt = await artworkRepository.InsertArtworkAsync(artwork, model.Tags!, cancellationToken);
        
        return createdArt == null
            ? Result<Artwork>.Failure(400, "Could not save artwork")!
            : Result<Artwork>.Success(createdArt);
    }

    public async Task<Result<List<ResponseArtworkModel>>> GetArtworksInfoAsync(CancellationToken cancellationToken)
    {
        var responseArtworks = new List<ResponseArtworkModel>();
        
        var artworks = await artworkRepository.GetArtworksWithUserDetailsAsync(cancellationToken);
        
        if (artworks == null)
            return Result<List<ResponseArtworkModel>>.Failure(400, "Could not retrieve artworks")!;
        
        foreach (var artwork in artworks)
        {
            var author = await userRepository.GetUserAsyncById(artwork.UserId, cancellationToken);

            var artModel = new ResponseArtworkModel
            {
                Title = artwork.Title,
                ArtworkPath = artwork.ArtworkPath,
                ProfileName = author!.ProfileName,
                AvatarPath = author.AvatarPath,
            };
            responseArtworks.Add(artModel);
        }
        
        return Result<List<ResponseArtworkModel>>.Success(responseArtworks)!;
    }
}
    