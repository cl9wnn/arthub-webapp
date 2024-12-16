using System.Net;
using System.Text;

namespace ArtHub;

public static class ErrorHandler
{
    public static async Task ShowError(int code, string message, HttpListenerContext context, CancellationToken token)
    {
        context.Response.StatusCode = code;
        context.Response.ContentType = "text/html; charset=utf-8"; 
        await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(message), token);
    }
}