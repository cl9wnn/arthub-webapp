using MyFramework.Contracts;
using MyORM;
namespace Persistence.Entities;

public class User: IAuthEntity
{
    [ColumnName("user_id")]
    public int UserId { get; init; }
    public string? Login { get; init; }
    public string? Password { get; set; }
    [ColumnName("profile_name")]
    public string? ProfileName { get; init; }
    [ColumnName("real_name")]
    public string? RealName { get; init; }
    public string? Role { get; init; }
    public string? Avatar { get; set; }
}