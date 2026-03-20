using InquisitorAI.Features.Scores.Dtos;
using InquisitorAI.Features.Scores.Queries;
using InquisitorAI.Features.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace InquisitorAI.Features.Scores.Endpoints;

public static class GetLeaderboardEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/scores/leaderboard", Handle);
    }

    public static async Task<IResult> Handle(
        int? top,
        IQueryHandler<GetLeaderboardQuery, IEnumerable<LeaderboardEntryDto>> handler,
        CancellationToken ct)
    {
        var query = new GetLeaderboardQuery(top ?? 50);
        var result = await handler.HandleAsync(query, ct);
        return Results.Ok(result);
    }
}
