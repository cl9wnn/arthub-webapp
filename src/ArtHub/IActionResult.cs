using System.Net;

namespace ArtHub;

public interface IActionResult
{
     Task ExecuteAsync(HttpListenerContext context, CancellationToken cancellationToken);
}