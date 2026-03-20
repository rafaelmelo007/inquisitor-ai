using System.Security.Claims;
using FluentValidation;
using InquisitorAI.Features.Shared;
using InquisitorAI.Features.Users.Commands;
using InquisitorAI.Features.Users.Dtos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace InquisitorAI.Features.Users.Endpoints;

public static class UpdateUserProfileEndpoint
{
    public static void MapUpdateUserProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/users/me", Handle).RequireAuthorization();
    }

    public static async Task<IResult> Handle(
        UpdateProfileRequest request,
        ClaimsPrincipal user,
        IValidator<UpdateUserProfileCommand> validator,
        ICommandHandler<UpdateUserProfileCommand, UserDto?> handler,
        NotificationHandler notifications,
        CancellationToken ct)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdClaim, out var userId))
        {
            return Results.Unauthorized();
        }

        var command = new UpdateUserProfileCommand(userId, request.DisplayName, request.AvatarUrl);

        var validation = validator.Validate(command);
        if (!validation.IsValid)
        {
            validation.Errors.ForEach(e => notifications.AddError(e.ErrorMessage));
            return Results.BadRequest(new { errors = notifications.Errors });
        }

        var result = await handler.HandleAsync(command, ct);

        return notifications.HasErrors
            ? Results.BadRequest(new { errors = notifications.Errors })
            : Results.Ok(result);
    }
}
