using BusinessLogic.Models;
using BusinessLogic.Validators;
using Persistence.Entities;
using Persistence.Repositories;

namespace BusinessLogic.Services;

public class ArtworkService(FileService fileService, ArtworkRepository artworkRepository, 
   SavingFavouriteRepository savingFavouriteRepository, UserRepository userRepository)
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
            var artMetrics = await artworkRepository.GetMetricsByIdAsync(artwork.ArtworkId, cancellationToken);
            var tags = await artworkRepository.GetTagsByIdAsync(artwork.ArtworkId, cancellationToken);

            var artModel = new GalleryArtworkModel
            {
                ArtworkId = artwork.ArtworkId,
                Category = artwork.Category,
                Title = artwork.Title,
                ArtworkPath = artwork.ArtworkPath,
                Tags = tags.Select(t => t.Name).ToList(),
                ProfileName = author!.ProfileName,
                AvatarPath = author.AvatarPath,
                LikesCount = artMetrics!.LikesCount,
                ViewsCount = artMetrics.ViewsCount 
            };
            responseArtworks.Add(artModel);
        }
        
        return Result<List<GalleryArtworkModel>>.Success(responseArtworks)!;
    }
    
    public async Task<Result<bool>> DeleteOwnArtworkAsync(int userId, int artworkId, CancellationToken cancellationToken)
    {
        var deletedArtwork = await artworkRepository.DeleteOwnArtworkAsync(userId, artworkId, cancellationToken);
        
        return deletedArtwork == null
            ? Result<bool>.Failure(400, "Could not retrieve artworks")!
            : Result<bool>.Success(true);
    }
  
    public async Task<Result<ArtworkPostModel>> GetArtworkPostAsync(int artworkId, int visitorId, CancellationToken cancellationToken)
    {
        var artwork = await artworkRepository.GetArtworkByIdAsync(artworkId, cancellationToken);
        
        if (artwork == null)
            return Result<ArtworkPostModel>.Failure(400, "Could not retrieve artwork")!;

        var tags = await artworkRepository.GetTagsByIdAsync(artworkId, cancellationToken);
        
        if (tags == null)
            return Result<ArtworkPostModel>.Failure(400, "Could not retrieve tags")!;
        
        var author = await userRepository.GetUpgradeUserByIdAsync(artwork.UserId, cancellationToken);
        
        if (author == null)
            return Result<ArtworkPostModel>.Failure(400, "Could not retrieve author")!;
        
        var artMetrics = await artworkRepository.GetMetricsByIdAsync(artworkId, cancellationToken);
        
        if (artMetrics == null)
            return Result<ArtworkPostModel>.Failure(400, "Could not retrieve metrics")!;

        var isLikedByUser = await artworkRepository.IsArtworkLikedByUserAsync(artworkId, visitorId, cancellationToken);
        var isSavedByUser = await savingFavouriteRepository.IsArtworkSavedByUserAsync(artworkId, visitorId, cancellationToken);
        
        var artworkPost = new ArtworkPostModel
        {
            ArtworkId = artwork.ArtworkId,
            Title = artwork.Title,
            Category = artwork.Category,
            Description = artwork.Description,
            ArtworkPath = artwork.ArtworkPath,
            Tags = tags.Select(t => t.Name).ToList()!,
            AuthorId = artwork.UserId,
            ProfileName = author.ProfileName,
            Fullname = author.Fullname,
            AvatarPath = author.AvatarPath,
            LikesCount = artMetrics.LikesCount,
            ViewsCount = artMetrics.ViewsCount,
            IsLiked = isLikedByUser,
            IsSaved = isSavedByUser
        };

        var visitorView = artworkRepository.UpdateViewCounterAsync(artworkId, cancellationToken);
        if (visitorView == null)
            return Result<ArtworkPostModel>.Failure(400, "Could not update view counter")!;
        
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
    
    public async Task<Result<bool>> IsArtworkExistsAsync(int artworkId, CancellationToken cancellationToken)
    {
        var artwork = await artworkRepository.GetArtworkByIdAsync(artworkId, cancellationToken);
        
        return artwork != null
            ? Result<bool>.Success(true)
            : Result<bool>.Failure(400, "Artwork does not exist");
    }
    
    public async Task<Result<bool>> IsArtworkOwnedByUserAsync(int artworkId, int userId, CancellationToken cancellationToken)
    {
        var artwork = await artworkRepository.GetArtworkByIdAsync(artworkId, cancellationToken);
        
        if (artwork == null)
            return Result<bool>.Failure(400, "Artwork does not exist");
        
        return artwork!.UserId == userId
            ? Result<bool>.Success(true)
            : Result<bool>.Failure(400, "This art is not yours!");
    }
}
    