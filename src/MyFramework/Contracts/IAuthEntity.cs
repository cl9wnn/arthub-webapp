namespace MyFramework.Contracts;

public interface IAuthEntity
{ 
    public int UserId { get; init; }
    public string? Role { get; init; }
}