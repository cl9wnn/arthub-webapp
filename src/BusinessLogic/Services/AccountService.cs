using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using Persistence.Repositories;

namespace BusinessLogic.Services;

public class AccountService(UserRepository userRepository, ArtworkRepository artworkRepository)
{
    
    public async Task<Result<UserProfileModel>> GetUserDataAsync(int id, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserAsyncById(id, cancellationToken);

        if (user == null)
            return Result<UserProfileModel>.Failure(404, "User dont found")!;
        
        var profileData = new UserProfileModel
        {
            ProfileName = user.ProfileName,
            AvatarPath = user.AvatarPath,
            Country = user.Country,
            Role = "user"
        };
        
        return Result<UserProfileModel>.Success(profileData);
    }
    
    public async Task<Result<ArtistProfileModel>> GetUpgradeUserDataAsync(int id, CancellationToken cancellationToken)
    {
        var artist = await userRepository.GetUpgradeUserByIdAsync(id, cancellationToken);

        if (artist == null)
            return Result<ArtistProfileModel>.Failure(404, "User dont found")!;

        var rewards = await artworkRepository.GetAccountRewardListAsync(id, cancellationToken);
        var userArts = await artworkRepository.GetProfileArtworksAsync(id, cancellationToken);
        var artsMetrics = await artworkRepository.GetProfileArtMetricsAsync(
            userArts!.Select(u => u.ArtworkId).ToList(), cancellationToken);
        
        
        var profileArts = userArts!.Join(
            artsMetrics!, 
            userArt => userArt.ArtworkId, 
            artMetric => artMetric.ArtworkId, 
            (userArt, artMetric) => new ProfileArtworkModel 
            {
                ArtworkId = userArt.ArtworkId,
                ArtworkPath = userArt.ArtworkPath,
                LikesCount = artMetric.LikesCount,
                ViewsCount = artMetric.ViewsCount
            }
        ).ToList();
        
        var profileData = new ArtistProfileModel
        {
            ProfileName = artist.ProfileName,
            Fullname = artist.Fullname,
            ContactInfo = artist.ContactInfo,
            AvatarPath = artist.AvatarPath,
            Country = artist.Country,
            Summary = artist.Summary,
            Role = "artist",
            ProfileArts = profileArts,
            Rewards = rewards
        };
        
        return Result<ArtistProfileModel>.Success(profileData);
    }
    
    public async Task<Result<bool>> CheckProfileForExist(int userId, CancellationToken cancellationToken)
    {
        var account = await userRepository.GetUserAsyncById(userId, cancellationToken);
        
        return account != null
            ? Result<bool>.Success(true)
            : Result<bool>.Failure(400, "Account does not exist");
    }
    
    public async Task<Result<string?>> GetAccountRoleAsync(int userId, CancellationToken cancellationToken)
    {
        var accountRole = await userRepository.GetUserRoleAsync(userId, cancellationToken);
        
        return (accountRole == null
            ? Result<string?>.Failure(404, "User dont found")!
            : Result<string>.Success(accountRole))!;
    }
}