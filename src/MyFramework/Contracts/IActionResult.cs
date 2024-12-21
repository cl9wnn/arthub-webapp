using System.Net;
namespace MyFramework.Contracts;

public interface IActionResult
{ 
    Task ExecuteAsync(HttpListenerContext context, CancellationToken cancellationToken);
}