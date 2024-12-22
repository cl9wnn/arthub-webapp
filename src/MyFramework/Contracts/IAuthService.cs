using System.Net;
using MyFramework.Contracts;
namespace MyFramework;

public interface IAuthService
{
    Task<IAuthEntity> AuthorizeUserAsync(HttpListenerContext context, CancellationToken cancellationToken);
}