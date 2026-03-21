namespace InquisitorAI.UI.Dtos;

public record SessionAnswerDto(
    long Id,
    long SessionId,
    long QuestionId,
    string QuestionText,
    string IdealAnswer,
    string? Transcript,
    decimal? Score,
    string? AiFeedback,
    string? Strengths,
    string? Weaknesses,
    string? ImprovementSuggestions);
