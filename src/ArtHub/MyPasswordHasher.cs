using System.Security.Cryptography;
namespace ArtHub;

public static class MyPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 10000;
    private const char SaltDelimiter = ';';
    private static readonly HashAlgorithmName HashAlgorithmName = HashAlgorithmName.SHA256;
    
    public static string HashPassword(string? password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName,KeySize);
        return string.Join(SaltDelimiter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
    }

    public static bool ValidatePassword(string passwordHash, string password)
    {
        var passwordElements = passwordHash.Split(SaltDelimiter);
        var salt = Convert.FromBase64String(passwordElements[0]);
        var hash = Convert.FromBase64String(passwordElements[1]);
        
        var inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName,KeySize);
        return CryptographicOperations.FixedTimeEquals(inputHash, hash);
    }
}