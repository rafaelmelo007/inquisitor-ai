using System.Security.Claims;
using InquisitorAI.Features.Auth.Commands;
using InquisitorAI.Features.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace InquisitorAI.Features.Auth.Endpoints;

public static class LogoutEndpoint
{
    public static void MapLogoutEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/logout", Handle).RequireAuthorization();
    }

    public static async Task<IResult> Handle(
        ClaimsPrincipal user,
        ICommandHandler<RevokeRefreshTokenCommand> handler,
        CancellationToken ct)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdClaim, out var userId))
        {
            return Results.Unauthorized();
        }

        var command = new RevokeRefreshTokenCommand(userId);
        await handler.HandleAsync(command, ct);

        return Results.Ok();
    }
}
