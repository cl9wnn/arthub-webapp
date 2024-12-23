namespace WebAPI;

public interface IS3Storage<T>
{
     Task<T> UploadFileAsync(byte[] fileBytes, string objectName, string contentType);
     Task<byte[]> GetFileAsync(string objectName);


}