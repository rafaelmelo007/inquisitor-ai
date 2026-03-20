using System.Security.Claims;
using InquisitorAI.Features.Shared;
using InquisitorAI.Features.Users.Dtos;
using InquisitorAI.Features.Users.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace InquisitorAI.Features.Users.Endpoints;

public static class GetCurrentUserEndpoint
{
    public static void MapGetCurrentUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/users/me", Handle).RequireAuthorization();
    }

    public static async Task<IResult> Handle(
        ClaimsPrincipal user,
        IQueryHandler<GetCurrentUserQuery, UserDto?> handler,
        CancellationToken ct)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdClaim, out var userId))
        {
            return Results.Unauthorized();
        }

        var result = await handler.HandleAsync(new GetCurrentUserQuery(userId), ct);

        return result is not null
            ? Results.Ok(result)
            : Results.NotFound();
    }
}
