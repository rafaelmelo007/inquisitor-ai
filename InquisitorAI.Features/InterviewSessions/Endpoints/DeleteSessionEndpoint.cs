using System.Security.Claims;
using InquisitorAI.Features.InterviewSessions.Commands;
using InquisitorAI.Features.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace InquisitorAI.Features.InterviewSessions.Endpoints;

public static class DeleteSessionEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/sessions/{id}", Handle)
            .RequireAuthorization();
    }

    public static async Task<IResult> Handle(
        long id,
        ClaimsPrincipal user,
        ICommandHandler<DeleteInterviewSessionCommand> handler,
        NotificationHandler notifications,
        CancellationToken ct)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdClaim, out var userId))
            return Results.Unauthorized();

        var command = new DeleteInterviewSessionCommand(id, userId);
        await handler.HandleAsync(command, ct);

        return notifications.HasErrors
            ? Results.BadRequest(new { errors = notifications.Errors })
            : Results.NoContent();
    }
}
