using Microsoft.EntityFrameworkCore;
using InquisitorAI.Features.Shared;

namespace InquisitorAI.Features.InterviewSessions.Commands;

public record DeleteInterviewSessionCommand(long SessionId, long UserId) : ICommand;

public class DeleteInterviewSessionHandler(AppDbContext context, NotificationHandler notifications)
    : ICommandHandler<DeleteInterviewSessionCommand>
{
    public async Task HandleAsync(DeleteInterviewSessionCommand command, CancellationToken ct = default)
    {
        var session = await context.InterviewSessions
            .FirstOrDefaultAsync(s => s.Id == command.SessionId, ct);

        if (session is null)
        {
            notifications.AddError("Session not found.");
            return;
        }

        if (session.UserId != command.UserId)
        {
            notifications.AddError("You do not have permission to delete this session.");
            return;
        }

        context.InterviewSessions.Remove(session);
        await context.SaveChangesAsync(ct);
    }
}
