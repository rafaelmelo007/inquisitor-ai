using System.Security.Claims;
using InquisitorAI.Features.InterviewSessions.Commands;
using InquisitorAI.Features.InterviewSessions.Dtos;
using InquisitorAI.Features.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace InquisitorAI.Features.InterviewSessions.Endpoints;

public static class SubmitAnswerEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/sessions/{id}/answers", Handle)
            .RequireAuthorization();
    }

    public static async Task<IResult> Handle(
        long id,
        SubmitAnswerRequest request,
        ClaimsPrincipal user,
        ICommandHandler<SubmitAnswerCommand, SessionAnswerDto?> handler,
        NotificationHandler notifications,
        CancellationToken ct)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdClaim, out var userId))
            return Results.Unauthorized();

        var command = new SubmitAnswerCommand(id, userId, request.QuestionId, request.Transcript);
        var result = await handler.HandleAsync(command, ct);

        return notifications.HasErrors
            ? Results.BadRequest(new { errors = notifications.Errors })
            : Results.Ok(result);
    }
}
