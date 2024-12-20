﻿using System.Net;
namespace ArtHub.Controllers;

public class StaticFileController: BaseController
{
    [Route("/static/*", "GET")]
    public IActionResult ShowStaticFileAsync(HttpListenerContext context, CancellationToken token)
    {
        if (context.Request.Url?.LocalPath == null)
        {
            return new ErrorResult(404, "Такой страницы нет!");
        }
        
        var path = context.Request.Url!.LocalPath["/static".Length..];
        var filePath = $"public/{path}";

        if (!File.Exists(filePath))
        {
            return new ErrorResult(404, "Такой страницы нет!");
        } 
        return new ResourceResult(filePath);
    }
}