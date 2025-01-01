using WebAPI.Models;

namespace WebAPI.Services;

public class FileService(IS3Storage<string> storage)
{
    public async Task<Result<string>> SaveFileAsync(FileModel? file, string folder, CancellationToken cancellationToken)
    {
        if (file == null)
            return Result<string>.Failure(404, "File not uploaded!")!;
        
        if (!file.ContentType!.StartsWith("image/"))
            return Result<string>.Failure(404, "File is not an image!")!;
        
        var fileBytes = Convert.FromBase64String(file.FileData!);
        var fileType = file.ContentType!.Split('/')[1];
        var fileName = $"{file.FileName}.{fileType}";
        var filePath = $"{folder}/{fileName}";
        
        var fileUrl = await storage.UploadFileAsync(fileBytes, filePath, file.ContentType!, cancellationToken);

        return fileUrl == null
            ? Result<string>.Failure(404,"Error uploading file!")!
            : Result<string>.Success(fileName!);
    }
    
    public async Task<Result<bool>> DeleteFileAsync(string fileName, string folder, CancellationToken cancellationToken)
    {
        var objectName = $"{folder}/{fileName}";
            
        var isDeleted = await storage.DeleteFileAsync(objectName, cancellationToken);

        return isDeleted
            ? Result<bool>.Success(true)
            : Result<bool>.Failure(400, "Error deleting file!");
    }    
}