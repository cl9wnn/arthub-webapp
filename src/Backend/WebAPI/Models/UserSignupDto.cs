using Persistence.Entities;
namespace WebAPI.Models;

public class UserSignupDto
{
    public string? Login { get; set; }
    public string? Password { get; set; }
    public string? ProfileName { get; set; }
    public string? Country { get; set; }
}