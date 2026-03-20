using System.Security.Claims;
using InquisitorAI.Features.Questionnaires.Commands;
using InquisitorAI.Features.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace InquisitorAI.Features.Questionnaires.Endpoints;

public static class DeleteQuestionnaireEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/questionnaires/{id}", Handle)
            .RequireAuthorization();
    }

    public static async Task<IResult> Handle(
        long id,
        ClaimsPrincipal user,
        ICommandHandler<DeleteQuestionnaireCommand> handler,
        NotificationHandler notifications,
        CancellationToken ct)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdClaim, out var userId))
            return Results.Unauthorized();

        var command = new DeleteQuestionnaireCommand(id, userId);
        await handler.HandleAsync(command, ct);

        return notifications.HasErrors
            ? Results.BadRequest(new { errors = notifications.Errors })
            : Results.NoContent();
    }
}
