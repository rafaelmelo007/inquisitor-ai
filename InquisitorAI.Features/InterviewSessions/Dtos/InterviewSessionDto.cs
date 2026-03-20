namespace InquisitorAI.Features.InterviewSessions.Dtos;

public record InterviewSessionDto(
    long Id,
    long UserId,
    long QuestionnaireId,
    string QuestionnaireName,
    DateTime StartedAt,
    DateTime? EndedAt,
    int? DurationSeconds,
    decimal? FinalScore,
    string? Classification,
    string? ReportContent,
    IReadOnlyList<SessionAnswerDto> Answers);
