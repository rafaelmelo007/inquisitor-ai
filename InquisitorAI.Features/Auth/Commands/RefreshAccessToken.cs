using System.Security.Cryptography;
using System.Text;
using InquisitorAI.Features.Auth.Domain;
using InquisitorAI.Features.Auth.Dtos;
using InquisitorAI.Features.Shared;
using Microsoft.EntityFrameworkCore;

namespace InquisitorAI.Features.Auth.Commands;

public record RefreshAccessTokenCommand(string RefreshToken) : ICommand<TokenResponseDto?>;

public class RefreshAccessTokenHandler(
    AppDbContext context,
    NotificationHandler notifications,
    IDateTimeService clock,
    IJwtService jwtService) : ICommandHandler<RefreshAccessTokenCommand, TokenResponseDto?>
{
    public async Task<TokenResponseDto?> HandleAsync(RefreshAccessTokenCommand command, CancellationToken ct = default)
    {
        var tokenHash = HashToken(command.RefreshToken);
        var now = clock.UtcNow;

        var storedToken = await context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == tokenHash, ct);

        if (storedToken is null)
        {
            notifications.AddError("Invalid refresh token.");
            return null;
        }

        if (storedToken.RevokedAt is not null)
        {
            notifications.AddError("Refresh token has been revoked.");
            return null;
        }

        if (storedToken.ExpiresAt <= now)
        {
            notifications.AddError("Refresh token has expired.");
            return null;
        }

        // Revoke old token
        storedToken.RevokedAt = now;
        storedToken.UpdatedAt = now;

        // Issue new refresh token
        var rawRefreshToken = jwtService.GenerateRefreshToken();
        var newTokenHash = HashToken(rawRefreshToken);

        var newRefreshToken = new RefreshToken
        {
            UserId = storedToken.UserId,
            Token = newTokenHash,
            ExpiresAt = now.AddDays(30),
            CreatedAt = now,
            UpdatedAt = now
        };

        context.RefreshTokens.Add(newRefreshToken);
        await context.SaveChangesAsync(ct);

        var accessToken = jwtService.GenerateAccessToken(storedToken.User);

        return new TokenResponseDto(accessToken, rawRefreshToken, ExpiresIn: 3600);
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}
