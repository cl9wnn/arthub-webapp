namespace ArtHub.Entities;

public class User
{
    public long UserId { get; init; }
    public string? Login { get; init; }
    public string? Password { get; set; }
    public string? Role { get; init; }
}