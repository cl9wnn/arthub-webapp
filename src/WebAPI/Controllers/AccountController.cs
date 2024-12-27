using System.Net;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using MyORM;
using Persistence.Entities;
using Persistence.Repositories;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers;

public class AccountController(AvatarService avatarService): MyBaseController
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
        const string path = "public/RegistrationPage/index.html";
        return new ResourceResult(path);
    }
    
    
    
    [Authorize("user")]
    [HttpGet("/api/get-account")]
    public async Task<IMyActionResult> GetAccountInfoAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        if (!context.TryGetItem<int>("userId", out var userId))
            return new ErrorResult(400, "Not authorized");

        var result = await avatarService.GetAccountDataAsync(userId, cancellationToken);
        
        return result.IsSuccess
            ? new JsonResult<UserProfileModel>(result.Data)
            : new ErrorResult(result.StatusCode, result.ErrorMessage!);
    }
}