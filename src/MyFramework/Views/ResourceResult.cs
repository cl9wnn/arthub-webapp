using System.Net;
using MyFramework.Contracts;

namespace MyFramework;

public class ResourceResult(string path): IMyActionResult
{
    public async Task ExecuteAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        context.Response.StatusCode = 200;
        context.Response.ContentType = path!.Split('.').Last() switch
        {
            "html" => "text/html",
            "css" => "text/css",
            "js" => "text/javascript",
            "svg" => "image/svg+xml",
            "ico" => "image/x-icon",
            _ => "application/octet-stream"
        };
        var file = await File.ReadAllBytesAsync(path, cancellationToken);
        await context.Response.OutputStream.WriteAsync(file, cancellationToken);
    }
}