using System.Net;
namespace MyFramework.Contracts;

public interface IMyActionResult
{ 
    Task ExecuteAsync(HttpListenerContext context, CancellationToken cancellationToken);
}