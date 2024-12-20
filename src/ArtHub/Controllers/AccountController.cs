using System.Net;

namespace ArtHub.Controllers;

public class AccountController: BaseController
{
    [Route("/account", "GET")]
    public IActionResult ShowAccountAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        const string path = "public/AccountPage/index.html";
        return new ResourceResult(path);
    }
}