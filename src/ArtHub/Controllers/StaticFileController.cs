using System.Net;
namespace ArtHub.Controllers;

public class StaticFileController
{
    
    [Route("/static/*", "GET")]
    public  async Task ShowStaticFileAsync(HttpListenerContext context, CancellationToken token)
    {
        if (context.Request.Url?.LocalPath == null)
        {
            await WebHelper.ShowError(404, "Такой страницы нет!", context, token);
            return; 
        }
        
        var path = context.Request.Url.LocalPath["/static".Length..];
        var filePath = $"public/{path}";

        if (!File.Exists(filePath))
        {
            await WebHelper.ShowError(404, "Такой страницы нет!", context,token);
            return;
        }
        await WebHelper.ShowResourceFile(path, context, token);
    }
}