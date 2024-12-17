using System.Net;
using System.Text;
using System.Text.Json;
using ArtHub.Entities;

namespace ArtHub;

public static class WebHelper
{
    public static async Task<T?> ReadBodyAsync<T>(HttpListenerContext context, CancellationToken cancellationToken)
    {
        using var sr = new StreamReader(context.Request.InputStream);
        var body = await sr.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        
        return JsonSerializer.Deserialize<T>(body,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }
    public static async Task WriteJsonAsync<T>(T data, HttpListenerContext context, CancellationToken cancellationToken)
    {
        context.Response.StatusCode = 200;
        context.Response.ContentType = "application/json";

        var responseBody = JsonSerializer.Serialize(data,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                
            }
        );
        var bytes = Encoding.UTF8.GetBytes(responseBody);
        await context.Response.OutputStream.WriteAsync(bytes, cancellationToken);
    }
    public static async Task ShowError(int code, string message, HttpListenerContext context, CancellationToken token)
    {
        context.Response.StatusCode = code;
        context.Response.ContentType = "text/html; charset=utf-8"; 
        await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(message), token);
    }
    public static async Task ShowResourceFile(string path, HttpListenerContext context, CancellationToken token)
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