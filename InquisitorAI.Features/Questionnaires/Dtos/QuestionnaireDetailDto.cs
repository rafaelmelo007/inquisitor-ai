namespace InquisitorAI.Features.Questionnaires.Dtos;

public record QuestionnaireDetailDto(
    long Id,
    string Name,
    long CreatedByUserId,
    string CreatedByDisplayName,
    bool IsPublic,
    long QuestionCount,
    DateTime CreatedAt,
    List<QuestionDto> Questions);
