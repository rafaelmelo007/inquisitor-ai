using InquisitorAI.Features.Questionnaires.Domain;
using InquisitorAI.Features.Questionnaires.Dtos;
using InquisitorAI.Features.Questionnaires.Extensions;
using InquisitorAI.Features.Shared;

namespace InquisitorAI.Features.Questionnaires.Commands;

public record ImportQuestionnaireCommand(long UserId, string MarkdownContent, bool IsPublic) : ICommand<QuestionnaireDto?>;

public class ImportQuestionnaireHandler(
    AppDbContext context,
    NotificationHandler notifications,
    IDateTimeService clock,
    IMarkdownParserService markdownParser)
    : ICommandHandler<ImportQuestionnaireCommand, QuestionnaireDto?>
{
    public async Task<QuestionnaireDto?> HandleAsync(ImportQuestionnaireCommand command, CancellationToken ct = default)
    {
        ParsedQuestionnaireDto parsed;
        try
        {
            parsed = markdownParser.Parse(command.MarkdownContent);
        }
        catch (FormatException ex)
        {
            notifications.AddError(ex.Message);
            return null;
        }

        if (parsed.Questions.Count == 0)
        {
            notifications.AddError("Questionnaire must contain at least one question.");
            return null;
        }

        var now = clock.UtcNow;

        var questionnaire = new Questionnaire
        {
            Name = parsed.Name,
            CreatedByUserId = command.UserId,
            IsPublic = command.IsPublic,
            CreatedAt = now,
            UpdatedAt = now
        };

        foreach (var pq in parsed.Questions)
        {
            Difficulty? difficulty = null;
            if (!string.IsNullOrWhiteSpace(pq.Difficulty) &&
                Enum.TryParse<Difficulty>(pq.Difficulty, ignoreCase: true, out var d))
            {
                difficulty = d;
            }

            questionnaire.Questions.Add(new Question
            {
                OrderIndex = pq.OrderIndex,
                Category = pq.Category,
                Difficulty = difficulty,
                QuestionText = pq.QuestionText,
                IdealAnswer = pq.IdealAnswer,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        context.Questionnaires.Add(questionnaire);
        await context.SaveChangesAsync(ct);

        return questionnaire.ToDto();
    }
}
