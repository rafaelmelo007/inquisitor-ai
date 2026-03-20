namespace InquisitorAI.UI.Dtos;

public record QuestionnaireDetailDto(
    long Id,
    string Name,
    long CreatedByUserId,
    string CreatedByDisplayName,
    bool IsPublic,
    int QuestionCount,
    List<QuestionDto> Questions);
