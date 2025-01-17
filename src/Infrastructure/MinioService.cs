using Application.Interfaces;
using Minio;
using Minio.DataModel.Args;

namespace Infrastructure;

public class MinioService(string bucketName, string accessKey, string secretKey): IS3Storage<string>
{
    private const string Endpoint = "localhost:9000";

    private readonly IMinioClient _minioClient = new MinioClient()
        .WithEndpoint(Endpoint)
        .WithCredentials(accessKey, secretKey)
        .Build();

    private async Task EnsureBucketExistsAsync()
    {
        var found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
        if (!found)
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
        }
    }

    public async Task<string?> UploadFileAsync(byte[] fileBytes, string objectName, string contentType, CancellationToken cancellationToken)
    {
        try
        {
            await EnsureBucketExistsAsync();

            using var stream = new MemoryStream(fileBytes);
            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(fileBytes.Length)
                .WithContentType(contentType), cancellationToken);

            var objectUrl = $"{Endpoint}/{bucketName}/{objectName}";
            return objectUrl;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    
    public async Task<bool> DeleteFileAsync(string objectName, CancellationToken cancellationToken)
    {
        try
        {
            await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName), cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении файла {objectName}: {ex.Message}");
            return false;
        }
       
    }
    public async Task<byte[]> GetFileAsync(string objectName, CancellationToken cancellationToken)
    {
        try
        {
            using var memoryStream = new MemoryStream();
        
            await _minioClient.GetObjectAsync(new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithCallbackStream(async (stream) =>
                {
                    await stream.CopyToAsync(memoryStream, cancellationToken);
                }), cancellationToken);
        
            return memoryStream.ToArray();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении файла: {ex.Message}");
            return null;
        }
    }
}
