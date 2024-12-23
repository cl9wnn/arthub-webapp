﻿using System.Net;
using System.Text;
using System.Text.Json;
using MyFramework.Contracts;

namespace MyFramework;

public class ErrorResult(int statusCode, string message): IMyActionResult
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