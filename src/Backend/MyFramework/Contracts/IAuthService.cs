using System.Net;

namespace MyFramework.Contracts;

public interface IAuthService
{
    Task<IAuthEntity?> AuthorizeUserAsync(HttpListenerContext context, CancellationToken cancellationToken);
}