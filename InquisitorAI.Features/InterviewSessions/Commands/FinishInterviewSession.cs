using Microsoft.EntityFrameworkCore;
using InquisitorAI.Features.InterviewSessions.Dtos;
using InquisitorAI.Features.InterviewSessions.Extensions;
using InquisitorAI.Features.Shared;

namespace InquisitorAI.Features.InterviewSessions.Commands;

public record FinishInterviewSessionCommand(long SessionId, long UserId) : ICommand<FinalResultDto?>;

public class FinishInterviewSessionHandler(
    AppDbContext context,
    NotificationHandler notifications,
    IDateTimeService clock,
    IReportGeneratorService reportGenerator)
    : ICommandHandler<FinishInterviewSessionCommand, FinalResultDto?>
{
    public async Task<FinalResultDto?> HandleAsync(FinishInterviewSessionCommand command, CancellationToken ct = default)
    {
        var session = await context.InterviewSessions
            .Include(s => s.SessionAnswers)
            .Include(s => s.Questionnaire)
            .FirstOrDefaultAsync(s => s.Id == command.SessionId, ct);

        if (session is null)
        {
            notifications.AddError("Session not found.");
            return null;
        }

        if (session.UserId != command.UserId)
        {
            notifications.AddError("You do not have permission to finish this session.");
            return null;
        }

        if (session.EndedAt is not null)
        {
            notifications.AddError("This session has already been completed.");
            return null;
        }

        var now = clock.UtcNow;

        var scores = session.SessionAnswers
            .Where(a => a.Score.HasValue)
            .Select(a => a.Score!.Value)
            .ToList();

        var finalScore = scores.Count > 0
            ? Math.Round(scores.Average(), 2)
            : 0m;

        var classification = finalScore >= 8.0m
            ? Domain.Classification.Approved
            : finalScore >= 6.5m
                ? Domain.Classification.ApprovedWithReservations
                : Domain.Classification.Failed;

        session.FinalScore = finalScore;
        session.Classification = classification;
        session.EndedAt = now;
        session.DurationSeconds = (int)(now - session.StartedAt).TotalSeconds;
        session.UpdatedAt = now;

        // Build a temporary DTO to generate the report
        var sessionDto = session.ToDto(session.Questionnaire.Name);
        var reportContent = reportGenerator.Generate(sessionDto);
        session.ReportContent = reportContent;

        await context.SaveChangesAsync(ct);

        var strengths = string.Join("; ", session.SessionAnswers
            .Where(a => !string.IsNullOrWhiteSpace(a.Strengths))
            .Select(a => a.Strengths));

        var improvementAreas = string.Join("; ", session.SessionAnswers
            .Where(a => !string.IsNullOrWhiteSpace(a.ImprovementSuggestions))
            .Select(a => a.ImprovementSuggestions));

        return new FinalResultDto(
            finalScore,
            classification.ToString(),
            strengths,
            improvementAreas,
            reportContent);
    }
}
