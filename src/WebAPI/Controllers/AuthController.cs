using BusinessLogic.Models;
using BusinessLogic.Services;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using WebAPI.Models;

namespace WebAPI.Controllers;

public class AuthController(UserService userService) : MyBaseController
{
    
    [HttpPost("/auth/signup")]
    public async Task<IMyActionResult> Register([FromBody] SignupRequest? signUpDto, CancellationToken cancellationToken)
    {
        if (signUpDto == null || signUpDto.Avatar == null || signUpDto.User == null)
            return new ErrorResult(400, "Invalid request");
        
        var userDto = signUpDto.User;
        var avatarDto = signUpDto.Avatar;

        var userModel = new UserModel
        {
            Login = userDto.Login,
            Password = userDto.Password,
            ProfileName = userDto.ProfileName,
            Country = userDto.Country
        };
        
        var userResult = await userService.RegisterUserAsync(userModel, avatarDto.FileData!, avatarDto.ContentType!, cancellationToken);
        
        return userResult.IsSuccess
            ? new JsonResult<JwtTokenModel>(userResult!.Data)
            : new ErrorResult(userResult.StatusCode, userResult.ErrorMessage!);
    }
    

    [HttpPost("/auth/signin")]
    public async Task<IMyActionResult> Login([FromBody] UserSigninDto? signinDto, CancellationToken cancellationToken)
    {
        if (signinDto == null)
            return new ErrorResult(400, "Invalid request");
        
        var result = await userService.LoginUserAsync(signinDto.Login!, signinDto.Password!, cancellationToken);

        return result.IsSuccess
            ? new JsonResult<JwtTokenModel>(result!.Data)
            : new ErrorResult(result.StatusCode, result.ErrorMessage!);
    }
}