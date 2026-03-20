namespace InquisitorAI.Features.Questionnaires.Dtos;

public record ParsedQuestionDto(
    int OrderIndex,
    string? Category,
    string? Difficulty,
    string QuestionText,
    string IdealAnswer);
