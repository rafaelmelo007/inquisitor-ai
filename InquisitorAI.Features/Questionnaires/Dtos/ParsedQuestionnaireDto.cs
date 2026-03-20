namespace InquisitorAI.Features.Questionnaires.Dtos;

public record ParsedQuestionnaireDto(
    string Name,
    IReadOnlyList<ParsedQuestionDto> Questions);
