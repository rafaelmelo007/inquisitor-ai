using System.Security.Claims;
using InquisitorAI.Features.Scores.Dtos;
using InquisitorAI.Features.Scores.Queries;
using InquisitorAI.Features.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace InquisitorAI.Features.Scores.Endpoints;

public static class GetUserScoresEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/scores/me", Handle)
            .RequireAuthorization();
    }

    public static async Task<IResult> Handle(
        ClaimsPrincipal user,
        IQueryHandler<GetUserScoresQuery, UserScoreSummaryDto?> handler,
        CancellationToken ct)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdClaim, out var userId))
            return Results.Unauthorized();

        var query = new GetUserScoresQuery(userId);
        var result = await handler.HandleAsync(query, ct);

        return result is null
            ? Results.NotFound()
            : Results.Ok(result);
    }
}
