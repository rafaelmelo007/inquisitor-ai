using FluentValidation;
using InquisitorAI.Features.Shared;
using InquisitorAI.Features.Users.Dtos;
using InquisitorAI.Features.Users.Extensions;
using Microsoft.EntityFrameworkCore;

namespace InquisitorAI.Features.Users.Commands;

public record UpdateUserProfileCommand(long UserId, string DisplayName, string? AvatarUrl) : ICommand<UserDto?>;

public class UpdateUserProfileValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AvatarUrl).MaximumLength(1024).When(x => x.AvatarUrl is not null);
    }
}

public class UpdateUserProfileHandler(
    AppDbContext context,
    NotificationHandler notifications,
    IDateTimeService clock) : ICommandHandler<UpdateUserProfileCommand, UserDto?>
{
    public async Task<UserDto?> HandleAsync(UpdateUserProfileCommand command, CancellationToken ct = default)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == command.UserId, ct);

        if (user is null)
        {
            notifications.AddError("User not found.");
            return null;
        }

        user.DisplayName = command.DisplayName;
        user.AvatarUrl = command.AvatarUrl;
        user.UpdatedAt = clock.UtcNow;

        await context.SaveChangesAsync(ct);

        return user.ToDto();
    }
}
