using System.Net;
using MyFramework;
using MyFramework.Contracts;
using Persistence.Entities;

namespace BusinessLogic.Services;

public class AuthorizationService: IAuthService
{
    public Task<IAuthEntity?> AuthorizeUserAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        var authHeader = context.Request.Headers["Authorization"];

        if (authHeader == null)
        {
            return Task.FromResult<IAuthEntity?>(null);
        }

        var token = authHeader.Split()[1];
        var tokenValidationResult = JwtService.ValidateJwtToken(token);

        return Task.FromResult<IAuthEntity?>(!tokenValidationResult.isSuccess ? null : tokenValidationResult.user)!;
    }
}