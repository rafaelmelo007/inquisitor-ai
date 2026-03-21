using System.Security.Claims;
using InquisitorAI.Features.InterviewSessions.Commands;
using InquisitorAI.Features.InterviewSessions.Dtos;
using InquisitorAI.Features.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace InquisitorAI.Features.InterviewSessions.Endpoints;

public static class StartSessionEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/sessions", Handle)
            .RequireAuthorization();
    }

    public static async Task<IResult> Handle(
        StartSessionRequest request,
        ClaimsPrincipal user,
        ICommandHandler<StartInterviewSessionCommand, InterviewSessionDto?> handler,
        NotificationHandler notifications,
        CancellationToken ct)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdClaim, out var userId))
            return Results.Unauthorized();

        var command = new StartInterviewSessionCommand(userId, request.QuestionnaireId, request.Language);
        var result = await handler.HandleAsync(command, ct);

        return notifications.HasErrors
            ? Results.BadRequest(new { errors = notifications.Errors })
            : Results.Created($"/sessions/{result!.Id}", result);
    }
}
