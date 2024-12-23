﻿using System.Net;
using System.Text;
using System.Text.Json;
namespace MyFramework;

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
    public static async Task ShowError(int code, string message, HttpListenerContext context, CancellationToken token)
    {
        context.Response.StatusCode = code;
        context.Response.ContentType = "application/json"; 
        
        var responseBody = JsonSerializer.Serialize(message,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                
            }
        );
        var bytes = Encoding.UTF8.GetBytes(responseBody);
        await context.Response.OutputStream.WriteAsync(bytes, token);
    }
 }