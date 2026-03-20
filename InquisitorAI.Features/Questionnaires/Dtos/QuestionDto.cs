namespace InquisitorAI.Features.Questionnaires.Dtos;

public record QuestionDto(
    long Id,
    long QuestionnaireId,
    int OrderIndex,
    string? Category,
    string? Difficulty,
    string QuestionText,
    string IdealAnswer);
