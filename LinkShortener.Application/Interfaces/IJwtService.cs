namespace LinkShortener.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(string userId, string email, string role);
}