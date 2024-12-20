using System.Net;
using System.Text;
using System.Text.Json;

namespace ArtHub;

public class ErrorResult(int statusCode, string message): IActionResult
{
    public async Task ExecuteAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json"; 
        
            var responseBody = JsonSerializer.Serialize(message,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                
                }
            );
            var bytes = Encoding.UTF8.GetBytes(responseBody);
            await context.Response.OutputStream.WriteAsync(bytes, cancellationToken);
    }
}