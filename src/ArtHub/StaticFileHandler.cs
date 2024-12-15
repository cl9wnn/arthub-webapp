using System.Net;

namespace ArtHub;

public static class StaticFileHandler
{
    public static async Task ShowStatic(HttpListenerContext context, CancellationToken token)
    {
        if (context.Request.Url?.LocalPath == null)
        {
            await ErrorHandler.Show404(context, token);
            return; 
        }
    
        var path = context.Request.Url.LocalPath.Split('/').Last();
        var filePath = $"public/{path}";

        if (!File.Exists(filePath))
        {
            await ErrorHandler.Show404(context,token);
            return;
        }
        await ShowResourceFile(path,context, token);
    }

    public static async Task ShowIndex(HttpListenerContext context, CancellationToken token)
    {
        const string path = "index.html";
        await ShowResourceFile(path, context, token);
    }
    private static async Task ShowResourceFile(string path, HttpListenerContext context, CancellationToken token)
    {
        context.Response.StatusCode = 200;
        context.Response.ContentType = path!.Split('.').Last() switch
        {
            "html" => "text/html",
            "css" => "text/css",
            "js" => "text/js",
            "svg" => "image/svg+xml",
            "ico" => "image/x-icon",
            _ => "application/octet-stream"
        };
        var filePath = $"public/{path}";
        var file = await File.ReadAllBytesAsync(filePath, token);
        await context.Response.OutputStream.WriteAsync(file, token);
    }
}