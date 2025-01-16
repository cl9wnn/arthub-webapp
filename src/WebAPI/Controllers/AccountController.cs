using System.Net;
using BusinessLogic.Models;
using BusinessLogic.Services;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using MyFramework.Views;
using WebAPI.Models;

namespace WebAPI.Controllers;

public class AccountController(AccountService accountService): MyBaseController
{
    [Authorize("user", "artist")]
    [HttpGet("/api/account/{userId}")]
    public async Task<IMyActionResult> ShowAccountInfoAsync(int userId, CancellationToken cancellationToken)
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
    
    [Authorize( "user","artist")]
    [HttpGet("/api/get-decoration/{userId}")]
    public async Task<IMyActionResult> LoadProfileDecoration(int userId,
        CancellationToken cancellationToken)
    {
        var isAccountExistResult = await accountService.CheckProfileForExist(userId, cancellationToken);
        
        if (!isAccountExistResult.IsSuccess)
            return new ErrorResult(isAccountExistResult.StatusCode, isAccountExistResult.ErrorMessage!);

        var profileDecorationResult = await accountService.GetProfileDecorationAsync(userId, cancellationToken);

        return new JsonResult<int>(profileDecorationResult.Data);
    }
}