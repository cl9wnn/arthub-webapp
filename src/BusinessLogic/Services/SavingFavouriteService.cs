using BusinessLogic.Models;
using Persistence.Entities;
using Persistence.Repositories;

namespace BusinessLogic.Services;

public class SavingFavouriteService(SavingFavouriteRepository savingFavouriteRepository)
{
    public async Task<Result<List<SavingArtModel>>> GetSavingFavouritesAsync(int userId, CancellationToken cancellationToken)
    {
        var responseSavings = new List<SavingArtModel>();
        
        var artworks = await savingFavouriteRepository.GetSavingArtworksAsync(userId, cancellationToken);
        
        if (artworks == null)
            return Result<List<SavingArtModel>>.Failure(400, "Could not retrieve artworks")!;
        
        foreach (var artwork in artworks)
        {
            var artModel = new SavingArtModel
            {
                ArtworkId = artwork!.ArtworkId,
                Title = artwork.Title,
                ArtworkPath = artwork.ArtworkPath,
            };
            responseSavings.Add(artModel);
        }
        
        return Result<List<SavingArtModel>>.Success(responseSavings)!;
    }
    
    public async Task<Result<bool>> DeleteSavingFavouriteAsync(int userId, int artworkId, CancellationToken cancellationToken)
    {
        var deletedArtwork = await savingFavouriteRepository.DeleteSavingArtworkAsync(userId, artworkId, cancellationToken);
        
        return deletedArtwork == null
            ? Result<bool>.Failure(400, "Could not retrieve artworks")!
            : Result<bool>.Success(true);
    }
    
    public async Task<Result<SavingArt>> SaveFavouriteArtAsync(int artworkId, int userId, CancellationToken cancellationToken)
    {
        if (artworkId == 0 || userId == 0)
            return Result<SavingArt>.Failure(400, "Not found art or user")!;
        
        var isUserSaved = await savingFavouriteRepository.IsArtworkSavedByUserAsync(artworkId, userId, cancellationToken);

        if (isUserSaved)
        {
            var removedSavingArt = await savingFavouriteRepository.RemoveSavingAsync(artworkId, userId, cancellationToken);
            return removedSavingArt == null
                ? Result<SavingArt>.Failure(400, "Could not like artwork")!
                : Result<SavingArt>.Success(removedSavingArt);
        }
        
        var addSavingArt = await savingFavouriteRepository.AddSavingArtAsync(artworkId, userId, cancellationToken);
        return addSavingArt == null
            ? Result<SavingArt>.Failure(400, "Could not like artwork")!
            : Result<SavingArt>.Success(addSavingArt);
    }
}