using System.Net;
namespace ArtHub.Controllers;

public class StaticFileController
{
    [Route("/", "GET")]
    public async Task ShowIndexAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        const string path = "index.html";
        await WebHelper.ShowResourceFile(path, context, cancellationToken);
    }
    
    [Route("/style.css", "GET")]
    [Route("/favicon.ico", "GET")]
    [Route("/script.js", "GET")]
    public  async Task ShowStaticFileAsync(HttpListenerContext context, CancellationToken token)
    {
        if (context.Request.Url?.LocalPath == null)
        {
            await WebHelper.ShowError(404, "Такой страницы нет!", context, token);
            return; 
        }
        
        var path = context.Request.Url.LocalPath.Split('/').Last();
        var filePath = $"public/{path}";

        if (!File.Exists(filePath))
        {
            await WebHelper.ShowError(404, "Такой страницы нет!", context,token);
            return;
        }
        await WebHelper.ShowResourceFile(path,context, token);
    }
}