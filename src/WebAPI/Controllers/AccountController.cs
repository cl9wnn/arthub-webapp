using System.Net;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;

namespace WebAPI.Controllers;

public class AccountController: MyBaseController
{
    [HttpGet("/account")]
    public IMyActionResult ShowAccountAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        const string path = "public/AccountPage/index.html";
        return new ResourceResult(path);
    }
}