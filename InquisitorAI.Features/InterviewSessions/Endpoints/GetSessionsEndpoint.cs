using System.Security.Claims;
using InquisitorAI.Features.InterviewSessions.Dtos;
using InquisitorAI.Features.InterviewSessions.Queries;
using InquisitorAI.Features.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace InquisitorAI.Features.InterviewSessions.Endpoints;

public static class GetSessionsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/sessions", Handle)
            .RequireAuthorization();
    }

    public static async Task<IResult> Handle(
        ClaimsPrincipal user,
        IQueryHandler<GetInterviewSessionsQuery, IEnumerable<InterviewSessionDto>> handler,
        CancellationToken ct)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdClaim, out var userId))
            return Results.Unauthorized();

        var query = new GetInterviewSessionsQuery(userId);
        var result = await handler.HandleAsync(query, ct);
        return Results.Ok(result);
    }
}
