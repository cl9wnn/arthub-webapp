using Minio;
using Minio.DataModel.Args;
namespace WebAPI.Services;

public class MinioService: IS3Storage<string>
{
    //TODO: сделать конфиг и брать ключи оттуда
    private const string BucketName = "image-bucket";
    private const string Endpoint = "localhost:9000";
    private const string AccessKey = "cl9wn";
    private const string SecretKey = "1029384756u";

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

    public async Task<string> UploadFileAsync(byte[] fileBytes, string objectName, string contentType)
    {
        try
        {
            await EnsureBucketExistsAsync();

            using var stream = new MemoryStream(fileBytes);
            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(BucketName)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(fileBytes.Length)
                .WithContentType(contentType));

            var objectUrl = $"{Endpoint}/{BucketName}/{objectName}";
            return objectUrl;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    
    public async Task<byte[]> GetFileAsync(string objectName)
    {
        try
        {
            using var memoryStream = new MemoryStream();
        
            await _minioClient.GetObjectAsync(new GetObjectArgs()
                .WithBucket(BucketName)
                .WithObject(objectName)
                .WithCallbackStream(async (stream) =>
                {
                    await stream.CopyToAsync(memoryStream);
                }));
        
            return memoryStream.ToArray();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении файла: {ex.Message}");
            throw;
        }
    }
}
