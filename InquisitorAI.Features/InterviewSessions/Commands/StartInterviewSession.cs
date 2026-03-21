using Microsoft.EntityFrameworkCore;
using InquisitorAI.Features.InterviewSessions.Domain;
using InquisitorAI.Features.InterviewSessions.Dtos;
using InquisitorAI.Features.InterviewSessions.Extensions;
using InquisitorAI.Features.Shared;

namespace InquisitorAI.Features.InterviewSessions.Commands;

public record StartInterviewSessionCommand(long UserId, long QuestionnaireId, string? Language = null) : ICommand<InterviewSessionDto?>;

public class StartInterviewSessionHandler(
    AppDbContext context,
    NotificationHandler notifications,
    IDateTimeService clock)
    : ICommandHandler<StartInterviewSessionCommand, InterviewSessionDto?>
{
    public async Task<InterviewSessionDto?> HandleAsync(StartInterviewSessionCommand command, CancellationToken ct = default)
    {
        var questionnaire = await context.Questionnaires
            .FirstOrDefaultAsync(q => q.Id == command.QuestionnaireId
                && (q.IsPublic || q.CreatedByUserId == command.UserId), ct);

        if (questionnaire is null)
        {
            notifications.AddError("Questionnaire not found or not accessible.");
            return null;
        }

        var now = clock.UtcNow;

        var session = new InterviewSession
        {
            UserId = command.UserId,
            QuestionnaireId = command.QuestionnaireId,
            Language = command.Language,
            StartedAt = now,
            CreatedAt = now,
            UpdatedAt = now
        };

        context.InterviewSessions.Add(session);
        await context.SaveChangesAsync(ct);

        return session.ToDto(questionnaire.Name);
    }
}
