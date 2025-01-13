using System.Net;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using MyFramework.Views;

namespace WebAPI.Controllers;

public class StaticFileController: MyBaseController
{
    [HttpGet("/static/*")]
    public IMyActionResult ShowStaticFileAsync(HttpListenerContext context)
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