using System.Net;

namespace ArtHub.Controllers;

public class MainController
{
    [Route("/", "GET")]
    public async Task ShowIndexAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        const string path = "MainPage/index.html";
        await WebHelper.ShowResourceFile(path, context, cancellationToken);
    }
}