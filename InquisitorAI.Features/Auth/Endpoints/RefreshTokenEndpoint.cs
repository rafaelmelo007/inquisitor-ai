using InquisitorAI.Features.Auth.Commands;
using InquisitorAI.Features.Auth.Dtos;
using InquisitorAI.Features.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace InquisitorAI.Features.Auth.Endpoints;

public static class RefreshTokenEndpoint
{
    public static void MapRefreshTokenEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/refresh", Handle).AllowAnonymous();
    }

    public static async Task<IResult> Handle(
        RefreshTokenRequest request,
        ICommandHandler<RefreshAccessTokenCommand, TokenResponseDto?> handler,
        NotificationHandler notifications,
        CancellationToken ct)
    {
        var command = new RefreshAccessTokenCommand(request.RefreshToken);
        var result = await handler.HandleAsync(command, ct);

        return notifications.HasErrors
            ? Results.BadRequest(new { errors = notifications.Errors })
            : Results.Ok(result);
    }
}
