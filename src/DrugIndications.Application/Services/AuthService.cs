using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using DrugIndications.Domain.Entities;
using DrugIndications.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DrugIndications.Application.Services;
public class AuthService : IAuthService
{
    private readonly IConfiguration _config;

    private readonly List<User> _users = new()
    {
        new User { Id = 1, Username = "admin", PasswordHash = "admin123", Role = "Admin" },
        new User { Id = 2, Username = "user", PasswordHash = "user123", Role = "User" }
    };

    public AuthService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public User? ValidateUser(string username, string password)
    {
        return _users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
            && u.PasswordHash == password);
    }
}
