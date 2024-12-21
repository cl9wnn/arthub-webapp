using System.Net;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;

namespace WebAPI.Controllers;

public class AccountController: BaseController
{
    [Route("/account", "GET")]
    public IActionResult ShowAccountAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        const string path = "public/AccountPage/index.html";
        return new ResourceResult(path);
    }
}