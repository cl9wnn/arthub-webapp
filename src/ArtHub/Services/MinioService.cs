using Minio;
using Minio.DataModel.Args;
namespace ArtHub.Services;

public class MinioService
{
    //TODO: сделать конфиг и брать ключи оттуда
    private const string BucketName = "image-bucket";
    private const string Endpoint = "localhost:9000";
    private const string AccessKey = "minioadmin";
    private const string SecretKey = "minioadmin";

    private readonly IMinioClient _minioClient = new MinioClient()
        .WithEndpoint(Endpoint)
        .WithCredentials(AccessKey, SecretKey)
        .Build();

    private async Task EnsureBucketExistsAsync()
    {
        var found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(BucketName));
        if (!found)
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(BucketName));
        }
    }
    
    public async Task<string> UploadFileAsync(string filePath, string objectName)
    {
        try
        {
            await EnsureBucketExistsAsync();

            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(BucketName)
                .WithObject(objectName)
                .WithFileName(filePath)
                .WithContentType("image/jpeg"));

            string objectUrl = $"{Endpoint}/{BucketName}/{objectName}";
            return objectUrl;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке файла: {ex.Message}");
            throw;
        }
    }
}
