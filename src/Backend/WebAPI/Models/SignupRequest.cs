namespace WebAPI.Models;

public class SignupRequest
{
    public UserSignupDto? User { get; set; }
    public FileDto? Avatar { get; set; }
}