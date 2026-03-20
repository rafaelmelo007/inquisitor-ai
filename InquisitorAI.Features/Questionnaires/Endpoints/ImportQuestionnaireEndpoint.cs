using System.Security.Claims;
using InquisitorAI.Features.Questionnaires.Commands;
using InquisitorAI.Features.Questionnaires.Dtos;
using InquisitorAI.Features.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace InquisitorAI.Features.Questionnaires.Endpoints;

public static class ImportQuestionnaireEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/questionnaires", Handle)
            .RequireAuthorization()
            .DisableAntiforgery();
    }

    public static async Task<IResult> Handle(
        IFormFile file,
        [FromForm] bool isPublic,
        ClaimsPrincipal user,
        ICommandHandler<ImportQuestionnaireCommand, QuestionnaireDto?> handler,
        NotificationHandler notifications,
        CancellationToken ct)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdClaim, out var userId))
            return Results.Unauthorized();

        if (file is null || file.Length == 0)
        {
            notifications.AddError("A .md file is required.");
            return Results.BadRequest(new { errors = notifications.Errors });
        }

        if (!file.FileName.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
        {
            notifications.AddError("Only .md files are accepted.");
            return Results.BadRequest(new { errors = notifications.Errors });
        }

        using var reader = new StreamReader(file.OpenReadStream());
        var content = await reader.ReadToEndAsync(ct);

        var command = new ImportQuestionnaireCommand(userId, content, isPublic);
        var result = await handler.HandleAsync(command, ct);

        return notifications.HasErrors
            ? Results.BadRequest(new { errors = notifications.Errors })
            : Results.Created($"/questionnaires/{result!.Id}", result);
    }
}
