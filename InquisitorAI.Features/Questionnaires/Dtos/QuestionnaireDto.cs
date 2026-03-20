namespace InquisitorAI.Features.Questionnaires.Dtos;

public record QuestionnaireDto(
    long Id,
    string Name,
    long CreatedByUserId,
    string CreatedByDisplayName,
    bool IsPublic,
    long QuestionCount,
    DateTime CreatedAt);
