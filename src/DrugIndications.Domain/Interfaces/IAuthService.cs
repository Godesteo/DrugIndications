using DrugIndications.Domain.Entities;

namespace DrugIndications.Domain.Interfaces;
public interface IAuthService
{
    string GenerateToken(User user);
    User? ValidateUser(string username, string password);
}

