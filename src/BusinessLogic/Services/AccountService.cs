using BusinessLogic.Interfaces;
using Persistence.Repositories;
using WebAPI.Models;

namespace BusinessLogic.Services;

public class AccountService(UserRepository userRepository)
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
        var user = await userRepository.GetUpgradeUserAsyncById(id, cancellationToken);

        if (user == null)
            return Result<ArtistProfileModel>.Failure(404, "User dont found")!;
        
        var profileData = new ArtistProfileModel()
        {
            ProfileName = user.ProfileName,
            Fullname = user.Fullname,
            ContactInfo = user.ContactInfo,
            AvatarPath = user.AvatarPath,
            Country = user.Country,
            Role = "artist"
        };
        
        return Result<ArtistProfileModel>.Success(profileData);
    }
}