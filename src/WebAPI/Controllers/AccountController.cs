using System.Net;
using MyFramework;
using MyFramework.Attributes;
using MyFramework.Contracts;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers;

public class AccountController(IS3Storage<string> minioService): MyBaseController
{
    [HttpGet("/account-settings")]
    public IMyActionResult ShowAccountAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        const string path = "public/AccountSettingsPage/index.html";
        return new ResourceResult(path);
    }

    
    [HttpPost("/api/save-avatar")]
    public async Task<IMyActionResult> SaveAvatarAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        var file = await WebHelper.ReadBodyAsync<FileModel>(context, cancellationToken);

        if (file == null)
            return new ErrorResult(404, "File Not Found");
        
        var fileBytes = Convert.FromBase64String(file!.FileData!);
        var fileType = file.ContentType!.Split('/')[1];
        var fileName = $"avatars/{file.FileName}.{fileType}";
        
        var fileUrl = await minioService.UploadFileAsync(fileBytes, fileName, file.ContentType!);
        return new JsonResult<string>(fileUrl);
    }
}