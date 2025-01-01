using Persistence.Entities;
using Persistence.Repositories;
using WebAPI.Models;
using WebAPI.Validators;

namespace WebAPI.Services;

public class ArtworkService(IS3Storage<string> storage, FileService service, ArtworkRepository repository)
{
    public async Task<Result<Artwork>> SaveArtAsync(ArtworkModel model, int userId, CancellationToken cancellationToken)
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

        var createdArt = await repository.InsertArtworkAsync(artwork, model.Tags!, cancellationToken);
        
        return createdArt == null
            ? Result<Artwork>.Failure(400, "Could not save artwork")!
            : Result<Artwork>.Success(createdArt);
    }
}
    