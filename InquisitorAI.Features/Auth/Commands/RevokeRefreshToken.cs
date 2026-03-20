using InquisitorAI.Features.Shared;
using Microsoft.EntityFrameworkCore;

namespace InquisitorAI.Features.Auth.Commands;

public record RevokeRefreshTokenCommand(long UserId) : ICommand;

public class RevokeRefreshTokenHandler(
    AppDbContext context,
    IDateTimeService clock) : ICommandHandler<RevokeRefreshTokenCommand>
{
    public async Task HandleAsync(RevokeRefreshTokenCommand command, CancellationToken ct = default)
    {
        var now = clock.UtcNow;

        var activeTokens = await context.RefreshTokens
            .Where(rt => rt.UserId == command.UserId && rt.RevokedAt == null)
            .ToListAsync(ct);

        foreach (var token in activeTokens)
        {
            token.RevokedAt = now;
            token.UpdatedAt = now;
        }

        await context.SaveChangesAsync(ct);
    }
}
