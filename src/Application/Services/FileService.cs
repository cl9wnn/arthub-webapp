using Application;
using Application.Interfaces;

namespace Application.Services;


public class FileService(IS3Storage<string> storage)
{
    public async Task<Result<string>> SaveFileAsync(string? data, string? contentType, string folder, CancellationToken cancellationToken)
    {
        if (data == null)
            return Result<string>.Failure(404, "File not uploaded!")!;
        
        if (!contentType!.StartsWith("image/"))
            return Result<string>.Failure(404, "File is not an image!")!;
        
        var fileBytes = Convert.FromBase64String(data!);
        var fileType = contentType!.Split('/')[1];
        var fileName = $"{Guid.NewGuid()}.{fileType}";
        var filePath = $"{folder}/{fileName}";
        
        var fileUrl = await storage.UploadFileAsync(fileBytes, filePath, contentType!, cancellationToken);

        return fileUrl == null
            ? Result<string>.Failure(404,"Error uploading file!")!
            : Result<string>.Success(fileName!);
    }
    
    public async Task DeleteFileAsync(string fileName, string folder, CancellationToken cancellationToken)
    {
        var objectName = $"{folder}/{fileName}";
        await storage.DeleteFileAsync(objectName, cancellationToken);
    }    
}