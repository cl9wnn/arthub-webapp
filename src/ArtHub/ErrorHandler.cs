using System.Net;
using System.Text;

namespace ArtHub;

public static class ErrorHandler
{
    public static async Task Show404(HttpListenerContext context, CancellationToken cancellationToken)
    {
        context.Response.StatusCode = 404;
        context.Response.ContentType = "text/html; charset=utf-8"; 
        const string message = "Такая страница не найдена!";
        await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(message), cancellationToken);
    }
}