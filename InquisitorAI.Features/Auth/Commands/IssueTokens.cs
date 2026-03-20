using System.Security.Cryptography;
using System.Text;
using InquisitorAI.Features.Auth.Domain;
using InquisitorAI.Features.Auth.Dtos;
using InquisitorAI.Features.Shared;
using InquisitorAI.Features.Users.Domain;
using Microsoft.EntityFrameworkCore;

namespace InquisitorAI.Features.Auth.Commands;

public record IssueTokensCommand(
    string Provider,
    string ExternalId,
    string Email,
    string DisplayName,
    string? AvatarUrl) : ICommand<TokenResponseDto?>;

public class IssueTokensHandler(
    AppDbContext context,
    NotificationHandler notifications,
    IDateTimeService clock,
    IJwtService jwtService) : ICommandHandler<IssueTokensCommand, TokenResponseDto?>
{
    public async Task<TokenResponseDto?> HandleAsync(IssueTokensCommand command, CancellationToken ct = default)
    {
        if (!Enum.TryParse<OAuthProvider>(command.Provider, ignoreCase: true, out var provider))
        {
            notifications.AddError($"Unsupported OAuth provider: {command.Provider}");
            return null;
        }

        var now = clock.UtcNow;

        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Provider == provider && u.ExternalId == command.ExternalId, ct);

        if (user is null)
        {
            user = new User
            {
                Provider = provider,
                ExternalId = command.ExternalId,
                Email = command.Email,
                DisplayName = command.DisplayName,
                AvatarUrl = command.AvatarUrl,
                CreatedAt = now,
                UpdatedAt = now
            };
            context.Users.Add(user);
        }
        else
        {
            user.Email = command.Email;
            user.DisplayName = command.DisplayName;
            user.AvatarUrl = command.AvatarUrl;
            user.UpdatedAt = now;
        }

        var rawRefreshToken = jwtService.GenerateRefreshToken();
        var tokenHash = HashToken(rawRefreshToken);

        var refreshToken = new RefreshToken
        {
            User = user,
            Token = tokenHash,
            ExpiresAt = now.AddDays(30),
            CreatedAt = now,
            UpdatedAt = now
        };

        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync(ct);

        var accessToken = jwtService.GenerateAccessToken(user);

        return new TokenResponseDto(accessToken, rawRefreshToken, ExpiresIn: 3600);
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}
