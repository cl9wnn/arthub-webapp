using Persistence.Entities;

namespace WebAPI.Models;

public class SignUpModel
{
    public User? User { get; set; }
    public FileModel? Avatar { get; set; }
}