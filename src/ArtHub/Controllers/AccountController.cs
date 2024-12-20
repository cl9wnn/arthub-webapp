using System.Net;

namespace ArtHub.Controllers;

public class AccountController
{
    [Route("/account", "GET")]
    public async Task ShowIndexAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        const string path = "AccountPage/index.html";
        await WebHelper.ShowResourceFile(path, context, cancellationToken);
    }
}