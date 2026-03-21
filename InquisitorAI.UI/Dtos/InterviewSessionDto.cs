namespace InquisitorAI.UI.Dtos;

public record InterviewSessionDto(
    long Id,
    long QuestionnaireId,
    string QuestionnaireName,
    long UserId,
    string Status,
    DateTimeOffset StartedAt,
    DateTimeOffset? FinishedAt,
    TimeSpan? Duration,
    decimal? FinalScore,
    string? Classification,
    string? ReportContent,
    IReadOnlyList<SessionAnswerDto>? Answers = null);
