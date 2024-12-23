namespace WebAPI.Models;
public class FileModel
{
    public string? FileName { get; } = Guid.NewGuid().ToString();
    public string? ContentType { get; init; }
    public string? FileData { get; init; }
}