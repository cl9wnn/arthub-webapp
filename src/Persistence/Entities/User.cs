using MyFramework.Contracts;

namespace Persistence.Entities;

public class User: IAuthEntity
{
    public int UserId { get; init; }
    public string? Login { get; init; }
    public string? Password { get; set; }
    public string? Role { get; init; }
}