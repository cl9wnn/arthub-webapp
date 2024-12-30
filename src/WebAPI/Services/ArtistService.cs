using Persistence.Repositories;
using WebAPI.Models;

namespace WebAPI.Services;

public class ArtistService(ArtistRepository repository)
{
    public async Task<Result<JwtTokenModel>> RegisterArtistAsync(SignUpArtistModel artist, int userId,
        CancellationToken cancellationToken)
    {
        var validator = new ArtistValidator();
        var validationResult = await  validator.ValidateAsync(artist, cancellationToken);

        if (!validationResult.IsValid)
            return Result<JwtTokenModel>.Failure(401, validationResult.ToString())!;
        
        await repository.CreateArtistAsync(userId, 
            artist.Fullname!, artist.ContactInfo!, artist.Summary!, cancellationToken);
        
        var updatedUser = await repository.UpdateUserToArtistAsync(userId, cancellationToken);
        
        var authResult =  new JwtTokenModel
        {
            Token = JwtService.GenerateJwtToken(updatedUser)
        };
        return Result<JwtTokenModel>.Success(authResult);
    }
}