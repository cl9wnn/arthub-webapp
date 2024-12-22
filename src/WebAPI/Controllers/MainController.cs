using System.Net;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;


namespace WebAPI.Controllers;

public class MainController: MyBaseController
{
    [HttpGet("/")]
    public IMyActionResult ShowIndexAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        const string path = "public/MainPage/index.html";
        return new ResourceResult(path);
    }
}