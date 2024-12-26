using MyFramework.Contracts;
using MyORM;
namespace Persistence.Entities;

public class User: IAuthEntity
{
    [ColumnName("user_id")]
    public int UserId { get; init; }
    public string? Login { get; init; }
    public string? Password { get; set; }
    public string? Role { get; init; }
}