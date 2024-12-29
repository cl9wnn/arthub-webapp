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
    [ColumnName("contact_info")]
    public string? ContactInfo { get; set; }
    public string? Country { get; set; }
    public string? Avatar { get; set; }
    public string? Role { get; init; }
}