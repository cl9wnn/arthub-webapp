using System.Net;
using BusinessLogic.Models;
using BusinessLogic.Services;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using WebAPI.Models;

namespace WebAPI.Controllers;

public class AccountController(AccountService accountService): MyBaseController
{
    
    [HttpGet("/account/{userId}")]
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
    [HttpGet("/api/account/{userId}")]
    public async Task<IMyActionResult> ShowAccountInfoAsync(int userId, HttpListenerContext context, CancellationToken cancellationToken)
    {
        var isAccountExistResult = await accountService.CheckProfileForExist(userId, cancellationToken);
        
        if (!isAccountExistResult.IsSuccess)
            return new ErrorResult(isAccountExistResult.StatusCode, isAccountExistResult.ErrorMessage!);
        
        var accountRoleResult = await accountService.GetAccountRoleAsync(userId, cancellationToken);

        if (!accountRoleResult.IsSuccess)
            return new ErrorResult(accountRoleResult.StatusCode, accountRoleResult.ErrorMessage!);
        
        switch (accountRoleResult.Data)
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


    /*
    public async Task<IMyActionResult> LoadProfileDecoration(int userId, HttpListenerContext context,
        CancellationToken cancellationToken)
    {
        
    }
    */
}