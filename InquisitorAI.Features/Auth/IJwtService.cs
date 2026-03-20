using System.Security.Claims;
using InquisitorAI.Features.Users.Domain;

namespace InquisitorAI.Features.Auth;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateAccessToken(string token);
}
