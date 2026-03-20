using InquisitorAI.Features.InterviewSessions.Domain;
using InquisitorAI.Features.InterviewSessions.Dtos;

namespace InquisitorAI.Features.InterviewSessions.Extensions;

public static class SessionMappingExtensions
{
    public static InterviewSessionDto ToDto(this InterviewSession entity, string questionnaireName) =>
        new(
            entity.Id,
            entity.UserId,
            entity.QuestionnaireId,
            questionnaireName,
            entity.StartedAt.UtcDateTime,
            entity.EndedAt?.UtcDateTime,
            entity.DurationSeconds,
            entity.FinalScore,
            entity.Classification?.ToString(),
            entity.ReportContent,
            entity.SessionAnswers?.Select(a => a.ToDto(
                a.Question?.QuestionText ?? string.Empty,
                a.Question?.IdealAnswer ?? string.Empty)).ToList()
            ?? []);

    public static SessionAnswerDto ToDto(this SessionAnswer entity, string questionText, string idealAnswer) =>
        new(
            entity.Id,
            entity.SessionId,
            entity.QuestionId,
            questionText,
            idealAnswer,
            entity.Transcript,
            entity.Score,
            entity.AiFeedback,
            entity.Strengths,
            entity.Weaknesses,
            entity.ImprovementSuggestions);
}
