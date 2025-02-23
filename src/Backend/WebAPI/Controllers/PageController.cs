using MyFramework.Attributes;
using MyFramework.Contracts;
using MyFramework.Views;

namespace WebAPI.Controllers;

public class PageController: MyBaseController
{
    private const string RootPath = "../../../../../Frontend";
    
    [HttpGet("/")]
    public IMyActionResult ShowIndex()
    {
        const string path = $"{RootPath}/MainPage/index.html";
        return new ResourceResult(path);
    }
    
    [HttpGet("/account/{userId}")]
    public IMyActionResult ShowAccountPage()
    {
        const string path = $"{RootPath}/AccountPage/index.html";
        return new ResourceResult(path);
    }
    
    [HttpGet("/register-account")]
    public IMyActionResult ShowRegisterPage()
    {
        const string path = $"{RootPath}/RegistrationPage/index.html";
        return new ResourceResult(path);
    }
    
    [HttpGet("/register-artist")]
    public IMyActionResult ShowRegistrationArtistPage()
    {
        const string path = $"{RootPath}/RegistrationArtistPage/index.html";
        return new ResourceResult(path);
    }
    
    [HttpGet("/new/artwork")]
    public IMyActionResult ShowAddArtworkPage()
    {
        const string path = $"{RootPath}/AddArtworkPage/index.html";
        return new ResourceResult(path);
    }

    [HttpGet("/artwork/{id}")]
    public IMyActionResult ShowArtworkPage()
    {
        const string path = $"{RootPath}/ArtworkPage/index.html";
        return new ResourceResult(path);
    }
    
    [HttpGet("/market")]
    public IMyActionResult ShowMarketPage()
    {
        const string path = $"{RootPath}/MarketPage/index.html";
        return new ResourceResult(path);
    }
    
    [HttpGet("/savings")]
    public IMyActionResult ShowSavingsPage()
    {
        const string path = $"{RootPath}/SavingsPage/index.html";
        return new ResourceResult(path);
    }

}