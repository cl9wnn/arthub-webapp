using BusinessLogic.Models;
using BusinessLogic.Validators;
using Persistence.interfaces;
using Persistence.Repositories;

namespace BusinessLogic.Services;

public class ArtistService(IArtistRepository repository)
{
    public async Task<Result<JwtTokenModel>> RegisterArtistAsync(ArtistModel artistModel,
        CancellationToken cancellationToken)
    {
        var validator = new ArtistValidator();
        var validationResult = await  validator.ValidateAsync(artistModel, cancellationToken);

        if (!validationResult.IsValid)
            return Result<JwtTokenModel>.Failure(401, validationResult.ToString())!;
        
        await repository.CreateArtistAsync(artistModel.UserId, artistModel.Fullname!, artistModel.Summary!, 
            artistModel.ContactInfo!, cancellationToken);
        
        var updatedUser = await repository.UpdateUserToArtistAsync(artistModel.UserId, cancellationToken);
        
        var authResult =  new JwtTokenModel
        {
            Token = JwtService.GenerateJwtToken(updatedUser)
        };
        return Result<JwtTokenModel>.Success(authResult);
    }

    public async Task<Result<bool>> CheckProfileForUpgrade(int userId, CancellationToken cancellationToken)
    {
        var artist = await repository.SelectArtistByIdAsync(userId, cancellationToken);
        
        return artist != null
            ? Result<bool>.Success(true)
            : Result<bool>.Success(false);
    }
}