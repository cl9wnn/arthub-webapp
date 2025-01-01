using System.Net;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers;

public class AccountController(AccountService accountService): MyBaseController
{
    
    [HttpGet("/account")]
    public IMyActionResult ShowAccountPageAsync()
    {
        const string path = "public/AccountPage/index.html";
        return new ResourceResult(path);
    }
    
    [HttpGet("/register-account")]
    public IMyActionResult ShowRegisterPageAsync()
    {
        const string path = "public/RegistrationPage/index.html";
        return new ResourceResult(path);
    }
    
    [HttpGet("/register-artist")]
    public IMyActionResult ShowRegistrationArtistPageAsync()
    {
        const string path = "public/RegistrationArtistPage/index.html";
        return new ResourceResult(path);
    }
    
    [Authorize("user", "artist")]
    [HttpGet("/api/get-account")]
    public async Task<IMyActionResult> ShowAccountInfoAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        if (!context.TryGetItem<int>("userId", out var userId))
            return new ErrorResult(400, "Not authorized");

        if (!context.TryGetItem<string>("userRole", out var userRole))
            return new ErrorResult(400, "User role not provided");

        switch (userRole)
        {
            case "user":
            {
                var userResult = await accountService.GetUserDataAsync(userId, cancellationToken);

                return userResult.IsSuccess
                    ? new JsonResult<UserProfileModel>(userResult.Data)
                    : new ErrorResult(userResult.StatusCode, userResult.ErrorMessage!);
            }
            case "artist":
            {
                var artistResult = await accountService.GetUpgradeUserDataAsync(userId, cancellationToken);

                return artistResult.IsSuccess
                    ? new JsonResult<ArtistProfileModel>(artistResult.Data)
                    : new ErrorResult(artistResult.StatusCode, artistResult.ErrorMessage!);
            }
            default:
                return new ErrorResult(403, "Invalid user role");
        }
    }
}