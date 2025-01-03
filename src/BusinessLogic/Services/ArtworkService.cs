using BusinessLogic.Models;
using BusinessLogic.Validators;
using Persistence.Entities;
using Persistence.Repositories;

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

    public async Task<Result<List<GalleryArtworkModel>>> GetArtworksInfoAsync(CancellationToken cancellationToken)
    {
        var responseArtworks = new List<GalleryArtworkModel>();
        
        var artworks = await artworkRepository.GetGalleryArtworksAsync(cancellationToken);
        
        if (artworks == null)
            return Result<List<GalleryArtworkModel>>.Failure(400, "Could not retrieve artworks")!;
        
        foreach (var artwork in artworks)
        {
            var author = await userRepository.GetUserAsyncById(artwork.UserId, cancellationToken);

            var artModel = new GalleryArtworkModel
            {
                ArtworkId = artwork.ArtworkId,
                Title = artwork.Title,
                ArtworkPath = artwork.ArtworkPath,
                ProfileName = author!.ProfileName,
                AvatarPath = author.AvatarPath,
            };
            responseArtworks.Add(artModel);
        }
        
        return Result<List<GalleryArtworkModel>>.Success(responseArtworks)!;
    }

    public async Task<Result<ArtworkPostModel>> GetArtworkPostAsync(int artworkId, int visitorId, CancellationToken cancellationToken)
    {
        var artwork = await artworkRepository.GetArtworkByIdAsync(artworkId, cancellationToken);
        
        if (artwork == null)
            return Result<ArtworkPostModel>.Failure(400, "Could not retrieve artwork")!;

        var tags = await artworkRepository.GetTagsByIdAsync(artworkId, cancellationToken);
        
        if (tags == null)
            return Result<ArtworkPostModel>.Failure(400, "Could not retrieve tags")!;
        
        var author = await userRepository.GetUpgradeUserAsyncById(artwork.UserId, cancellationToken);
        
        if (author == null)
            return Result<ArtworkPostModel>.Failure(400, "Could not retrieve author")!;
        
        var isLikedByUser = await artworkRepository.IsArtworkLikedByUserAsync(artworkId, visitorId, cancellationToken);
        
        var artworkPost = new ArtworkPostModel
        {
            ArtworkId = artwork.ArtworkId,
            Title = artwork.Title,
            Category = artwork.Category,
            Description = artwork.Description,
            ArtworkPath = artwork.ArtworkPath,
            Tags = tags.Select(t => t.Name).ToList()!,
            ProfileName = author.ProfileName,
            Fullname = author.Fullname,
            AvatarPath = author.AvatarPath,
            LikeCount = artwork.LikeCount,
            IsLiked = isLikedByUser
        };
        
        return Result<ArtworkPostModel>.Success(artworkPost);
    }

    
    public async Task<Result<int>> LikeArtworkAsync(int artworkId, int userId, CancellationToken cancellationToken)
    {
        if (artworkId == 0 || userId == 0)
            return Result<int>.Failure(400, "Not found art or user");
        
        var isUserLiked = await artworkRepository.IsArtworkLikedByUserAsync(artworkId, userId, cancellationToken);

        if (isUserLiked)
        {
            var countAfterRemove = await artworkRepository.RemoveLikeAsync(artworkId, userId, cancellationToken);
            return countAfterRemove == -1
                ? Result<int>.Failure(400, "Could not like artwork")
                : Result<int>.Success(countAfterRemove);
        }
        
        var countAfterAdd = await artworkRepository.AddLikeAsync(artworkId, userId, cancellationToken);
        return countAfterAdd == -1
            ? Result<int>.Failure(400, "Could not like artwork")
            : Result<int>.Success(countAfterAdd);
    }
    
}
    