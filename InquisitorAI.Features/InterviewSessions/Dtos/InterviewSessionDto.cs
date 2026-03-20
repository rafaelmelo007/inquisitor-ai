namespace InquisitorAI.Features.InterviewSessions.Dtos;

public record InterviewSessionDto(
    long Id,
    long UserId,
    long QuestionnaireId,
    string QuestionnaireName,
    DateTimeOffset StartedAt,
    DateTimeOffset? EndedAt,
    int? DurationSeconds,
    decimal? FinalScore,
    string? Classification,
    string? ReportContent,
    IReadOnlyList<SessionAnswerDto> Answers);
