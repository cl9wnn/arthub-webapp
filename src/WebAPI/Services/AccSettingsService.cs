using WebAPI.Models;

namespace WebAPI.Services;

public class AccSettingsService(IS3Storage<string> storage)
{

    public async Task<Result<string>> SaveAvatarAsync(FileModel? file, CancellationToken cancellationToken)
    {
        if (file == null)
            return Result<string>.Failure(404, "File not uploaded!")!;
        
        if (!file.ContentType!.StartsWith("image/"))
            return Result<string>.Failure(404, "File is not an image!")!;
        
        var fileBytes = Convert.FromBase64String(file.FileData!);
        var fileType = file.ContentType!.Split('/')[1];
        var fileName = $"avatars/{file.FileName}.{fileType}";
        
        var fileUrl = await storage.UploadFileAsync(fileBytes, fileName, file.ContentType!);

        return Result<string>.Success(fileUrl);
    }
}