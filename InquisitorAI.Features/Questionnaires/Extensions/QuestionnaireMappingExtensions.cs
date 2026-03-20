using InquisitorAI.Features.Questionnaires.Domain;
using InquisitorAI.Features.Questionnaires.Dtos;

namespace InquisitorAI.Features.Questionnaires.Extensions;

public static class QuestionnaireMappingExtensions
{
    public static QuestionnaireDto ToDto(this Questionnaire entity) =>
        new(
            entity.Id,
            entity.Name,
            entity.CreatedByUserId,
            entity.User?.DisplayName ?? string.Empty,
            entity.IsPublic,
            entity.Questions?.Count ?? 0,
            entity.CreatedAt);

    public static QuestionDto ToDto(this Question entity) =>
        new(
            entity.Id,
            entity.QuestionnaireId,
            entity.OrderIndex,
            entity.Category,
            entity.Difficulty?.ToString(),
            entity.QuestionText,
            entity.IdealAnswer);
}
