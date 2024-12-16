namespace ArtHub.Entities;

public class User
{
    public long UserId { get; set; }
    public string? Login { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; }
}