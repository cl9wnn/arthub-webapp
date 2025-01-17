using MyFramework.Attributes;
using MyFramework.Contracts;
using MyFramework.Views;

namespace WebAPI.Controllers;

public class PageController: MyBaseController
{
    [HttpGet("/")]
    public IMyActionResult ShowIndex()
    {
        const string path = "public/MainPage/index.html";
        return new ResourceResult(path);
    }
    
    [HttpGet("/account/{userId}")]
    public IMyActionResult ShowAccountPage()
    {
        const string path = "public/AccountPage/index.html";
        return new ResourceResult(path);
    }
    
    [HttpGet("/register-account")]
    public IMyActionResult ShowRegisterPage()
    {
        const string path = "public/RegistrationPage/index.html";
        return new ResourceResult(path);
    }
    
    [HttpGet("/register-artist")]
    public IMyActionResult ShowRegistrationArtistPage()
    {
        const string path = "public/RegistrationArtistPage/index.html";
        return new ResourceResult(path);
    }
    
    [HttpGet("/new/artwork")]
    public IMyActionResult ShowAddArtworkPage()
    {
        const string path = "public/AddArtworkPage/index.html";
        return new ResourceResult(path);
    }

    [HttpGet("/artwork/{id}")]
    public IMyActionResult ShowArtworkPage()
    {
        const string path = "public/ArtworkPage/index.html";
        return new ResourceResult(path);
    }
    
    [HttpGet("/market")]
    public IMyActionResult ShowMarketPage()
    {
        const string path = "public/MarketPage/index.html";
        return new ResourceResult(path);
    }
    
    [HttpGet("/savings")]
    public IMyActionResult ShowSavingsPage()
    {
        const string path = "public/SavingsPage/index.html";
        return new ResourceResult(path);
    }

}