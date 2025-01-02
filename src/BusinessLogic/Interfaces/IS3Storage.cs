namespace BusinessLogic.Interfaces;

public interface IS3Storage<T>
{
     Task<T?> UploadFileAsync(byte[] fileBytes, string objectName, string contentType, CancellationToken cancellationToken);
     Task<byte[]> GetFileAsync(string objectName, CancellationToken cancellationToken);
     Task<bool> DeleteFileAsync(string objectName, CancellationToken cancellationToken);


}