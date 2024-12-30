using MyORM;

namespace WebAPI.Models;

public class UserProfileModel
{
    public string? ProfileName { get; init; }
    public string? Avatar { get; set; }
    public string? Country { get; init; }

    public string? Role { get; init; }
}