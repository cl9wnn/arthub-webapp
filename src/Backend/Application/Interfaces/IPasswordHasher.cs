namespace Application.Interfaces;

public interface IPasswordHasher
{
      string HashPassword(string? password);
      bool ValidatePassword(string passwordHash, string password);

}