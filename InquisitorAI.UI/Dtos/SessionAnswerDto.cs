namespace InquisitorAI.UI.Dtos;

public record SessionAnswerDto(
    long Id,
    long SessionId,
    long QuestionId,
    string Transcript,
    decimal Score,
    string Feedback,
    string? Strengths,
    string? Weaknesses,
    string? ImprovementSuggestions);
