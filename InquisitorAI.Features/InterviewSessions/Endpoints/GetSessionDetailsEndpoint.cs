using System.Security.Claims;
using InquisitorAI.Features.InterviewSessions.Dtos;
using InquisitorAI.Features.InterviewSessions.Queries;
using InquisitorAI.Features.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace InquisitorAI.Features.InterviewSessions.Endpoints;

public static class GetSessionDetailsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/sessions/{id}", Handle)
            .RequireAuthorization();
    }

    public static async Task<IResult> Handle(
        long id,
        ClaimsPrincipal user,
        IQueryHandler<GetSessionDetailsQuery, InterviewSessionDto?> handler,
        CancellationToken ct)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdClaim, out var userId))
            return Results.Unauthorized();

        var query = new GetSessionDetailsQuery(id, userId);
        var result = await handler.HandleAsync(query, ct);

        return result is null
            ? Results.NotFound()
            : Results.Ok(result);
    }
}
