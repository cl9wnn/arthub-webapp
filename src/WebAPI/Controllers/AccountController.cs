using System.Net;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers;

public class AccountController(AccSettingsService accSettingsService): MyBaseController
{
    [HttpGet("/account")]
    public IMyActionResult ShowAccountPageAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        const string path = "public/AccountPage/index.html";
        return new ResourceResult(path);
    }

    
    [HttpGet("/account-settings")]
    public IMyActionResult ShowSettingsPageAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        const string path = "public/AccountSettingsPage/index.html";
        return new ResourceResult(path);
    }

    
    [Authorize("user")]
    [HttpPost("/api/save-avatar")]
    public async Task<IMyActionResult> SaveAvatarAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        var file = await WebHelper.ReadBodyAsync<FileModel>(context, cancellationToken);
        
        var result = await accSettingsService.SaveAvatarAsync(file!, cancellationToken);

        return result.IsSuccess
            ? new JsonResult<string>("success!")
            : new ErrorResult(result.StatusCode, result.ErrorMessage!);
    }
    
    
}