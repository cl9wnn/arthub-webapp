using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Persistence.Entities;
namespace BusinessLogic.Services;

public static class JwtService
{
      private const string? SecretKey = "15tkwagt2h6k8m3vs9z9qf21qkgbheho155mbs";

    public static string GenerateJwtToken(User? user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("Sub", user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("Role", user.Role!),
        };

        var token = new JwtSecurityToken(
            issuer: "http://localhost:5050",
            audience: "http://localhost:5050",
            expires: DateTime.Now.AddMinutes(1),
            claims: claims,
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static (bool isSuccess, User user) ValidateJwtToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = "http://localhost:5050",
            ValidAudience = "http://localhost:5050",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey!))
        };
        try
        {
            var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            var userId = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "Sub")?.Value;
            var role = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "Role")?.Value;

            return (true, new User
            {
                UserId = int.Parse(userId!),
                Role = role
            });
        }
        catch
        {
            return (false, default)!;
        }
    }
}