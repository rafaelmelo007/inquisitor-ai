using System.Security.Claims;
using InquisitorAI.Features.Questionnaires.Dtos;
using InquisitorAI.Features.Questionnaires.Queries;
using InquisitorAI.Features.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace InquisitorAI.Features.Questionnaires.Endpoints;

public static class GetQuestionnaireByIdEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/questionnaires/{id}", Handle);
    }

    public static async Task<IResult> Handle(
        long id,
        ClaimsPrincipal user,
        IQueryHandler<GetQuestionnaireByIdQuery, QuestionnaireDto?> handler,
        CancellationToken ct)
    {
        long? userId = null;
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (long.TryParse(userIdClaim, out var parsed))
            userId = parsed;

        var query = new GetQuestionnaireByIdQuery(id, userId);
        var result = await handler.HandleAsync(query, ct);

        return result is null
            ? Results.NotFound()
            : Results.Ok(result);
    }
}
