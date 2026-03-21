using Microsoft.EntityFrameworkCore;
using InquisitorAI.Features.InterviewSessions.Domain;
using InquisitorAI.Features.InterviewSessions.Dtos;
using InquisitorAI.Features.InterviewSessions.Extensions;
using InquisitorAI.Features.Shared;

namespace InquisitorAI.Features.InterviewSessions.Commands;

public record SubmitAnswerCommand(long SessionId, long UserId, long QuestionId, string Transcript) : ICommand<SessionAnswerDto?>;

public class SubmitAnswerHandler(
    AppDbContext context,
    NotificationHandler notifications,
    IDateTimeService clock,
    IAiEvaluationService aiEvaluation)
    : ICommandHandler<SubmitAnswerCommand, SessionAnswerDto?>
{
    public async Task<SessionAnswerDto?> HandleAsync(SubmitAnswerCommand command, CancellationToken ct = default)
    {
        var session = await context.InterviewSessions
            .FirstOrDefaultAsync(s => s.Id == command.SessionId, ct);

        if (session is null)
        {
            notifications.AddError("Session not found.");
            return null;
        }

        if (session.UserId != command.UserId)
        {
            notifications.AddError("You do not have permission to submit answers for this session.");
            return null;
        }

        if (session.EndedAt is not null)
        {
            notifications.AddError("This session has already been completed.");
            return null;
        }

        var question = await context.Questions
            .FirstOrDefaultAsync(q => q.Id == command.QuestionId, ct);

        if (question is null)
        {
            notifications.AddError("Question not found.");
            return null;
        }

        var evaluationRequest = new EvaluateAnswerRequest(
            question.QuestionText,
            question.IdealAnswer,
            command.Transcript,
            session.Language);

        var evaluation = await aiEvaluation.EvaluateAsync(evaluationRequest, ct);

        var now = clock.UtcNow;

        var answer = new SessionAnswer
        {
            SessionId = command.SessionId,
            QuestionId = command.QuestionId,
            Transcript = command.Transcript,
            Score = evaluation.Score,
            AiFeedback = evaluation.Summary,
            Strengths = evaluation.Strengths,
            Weaknesses = evaluation.Weaknesses,
            ImprovementSuggestions = evaluation.ImprovementSuggestions,
            CreatedAt = now,
            UpdatedAt = now
        };

        context.SessionAnswers.Add(answer);
        await context.SaveChangesAsync(ct);

        return answer.ToDto(question.QuestionText, question.IdealAnswer);
    }
}
