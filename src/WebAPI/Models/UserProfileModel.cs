using MyORM;

namespace WebAPI.Models;

public class UserProfileModel
{
    public string? ProfileName { get; init; }
    public string? RealName { get; init; }
    public string? Avatar { get; set; }
}