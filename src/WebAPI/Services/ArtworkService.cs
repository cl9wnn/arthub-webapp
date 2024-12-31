using Persistence.Entities;
using Persistence.Repositories;
using WebAPI.Models;
using WebAPI.Validators;

namespace WebAPI.Services;

public class ArtworkService(IS3Storage<string> storage, ArtworkRepository repository)
{
    public async Task<Result<Artwork>> SaveArtAsync(string title, string category, string description,
        List<string> tags, FileModel? artFile, int userId, CancellationToken cancellationToken)
    {
        if (artFile == null)
            return Result<Artwork>.Failure(404, "File not uploaded!")!;

        if (!artFile.ContentType!.StartsWith("image/"))
            return Result<Artwork>.Failure(404, "File is not an image!")!;

        var fileBytes = Convert.FromBase64String(artFile.FileData!);
        var fileType = artFile.ContentType!.Split('/')[1];
        var fileName = $"{artFile.FileName}.{fileType}";

        var fileUrl = await storage.UploadFileAsync(fileBytes, $"arts/{fileName}", artFile.ContentType!, cancellationToken);

        if (fileUrl == null)
            return Result<Artwork>.Failure(404, "Ошибка сохранения файла!")!;

        var artwork = new Artwork
        {
            Title = title,
            Category = category,
            Description = description,
            ArtworkPath = fileName,
            UserId = userId
        };
        
        var createdArt = await CreateArtworkAsync(artwork, tags, cancellationToken);
        return Result<Artwork>.Success(createdArt!);
    }

    private async Task<Artwork?> CreateArtworkAsync(Artwork artwork, List<string> tags, CancellationToken cancellationToken)
    {
        var validator = new ArtworkValidator();
        var validationResult = await validator.ValidateAsync(artwork, cancellationToken);

        if (!validationResult.IsValid)
            return null;

        return await repository.InsertArtworkAsync(artwork, tags, cancellationToken);
    }

    public async Task<Result<bool>> DeleteArtAsync(string fileName, CancellationToken cancellationToken)
    {
        var objectName = $"arts/{fileName}";

        var isDeleted = await storage.DeleteFileAsync(objectName, cancellationToken);

        return isDeleted
            ? Result<bool>.Success(true)
            : Result<bool>.Failure(400, "Ошибка удаления файла!");
    }
}
    