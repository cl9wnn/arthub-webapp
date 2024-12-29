using MyFramework.Contracts;
using Persistence.Entities;
using Persistence.Repositories;
using WebAPI.Models;

namespace WebAPI.Services;

public class AccountService(IS3Storage<string> storage, UserRepository userRepository)
{
    public async Task<Result<string>> SaveAvatarAsync(FileModel? file, CancellationToken cancellationToken)
    {
        if (file == null)
            return Result<string>.Failure(404, "File not uploaded!")!;
        
        if (!file.ContentType!.StartsWith("image/"))
            return Result<string>.Failure(404, "File is not an image!")!;
        
        var fileBytes = Convert.FromBase64String(file.FileData!);
        var fileType = file.ContentType!.Split('/')[1];
        var fileName = $"{file.FileName}.{fileType}";
        
        var fileUrl = await storage.UploadFileAsync(fileBytes, $"avatars/{fileName}", file.ContentType!, cancellationToken);

        return fileUrl == null
            ? Result<string>.Failure(404,"Ошибка сохранения файла!")!
            : Result<string>.Success(fileName!);
    }
    
    public async Task<Result<bool>> DeleteAvatarAsync(string fileName, CancellationToken cancellationToken)
    {
            var objectName = $"avatars/{fileName}";
            
            var isDeleted = await storage.DeleteFileAsync(objectName, cancellationToken);

            return isDeleted
                ? Result<bool>.Success(true)
                : Result<bool>.Failure(400, "Ошибка удаления файла!");
    }    
    
    public async Task<Result<UserProfileModel>> GetAccountDataAsync(int id, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserAsync(id, cancellationToken);

        if (user == null)
            return Result<UserProfileModel>.Failure(404, "User dont found")!;
        
        var profileData = new UserProfileModel
        {
            ProfileName = user.ProfileName,
            RealName = user.RealName,
            Avatar = user.Avatar,
            ContactInfo = user.ContactInfo,
            Country = user.Country
        };
        
        return Result<UserProfileModel>.Success(profileData);
    }
}