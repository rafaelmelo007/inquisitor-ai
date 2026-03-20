namespace InquisitorAI.UI.Dtos;

public record QuestionDto(
    long Id,
    int OrderIndex,
    string? Category,
    string? Difficulty,
    string QuestionText,
    string IdealAnswer);
