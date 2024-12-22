namespace MyFramework.Contracts;

public interface IAuthEntity
{ 
    public long UserId { get; init; }
    public string? Role { get; init; }
}