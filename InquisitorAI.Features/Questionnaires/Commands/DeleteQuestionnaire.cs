using Microsoft.EntityFrameworkCore;
using InquisitorAI.Features.Shared;

namespace InquisitorAI.Features.Questionnaires.Commands;

public record DeleteQuestionnaireCommand(long QuestionnaireId, long RequestingUserId) : ICommand;

public class DeleteQuestionnaireHandler(AppDbContext context, NotificationHandler notifications)
    : ICommandHandler<DeleteQuestionnaireCommand>
{
    public async Task HandleAsync(DeleteQuestionnaireCommand command, CancellationToken ct = default)
    {
        var questionnaire = await context.Questionnaires
            .FirstOrDefaultAsync(q => q.Id == command.QuestionnaireId, ct);

        if (questionnaire is null)
        {
            notifications.AddError("Questionnaire not found.");
            return;
        }

        if (questionnaire.CreatedByUserId != command.RequestingUserId)
        {
            notifications.AddError("You do not have permission to delete this questionnaire.");
            return;
        }

        context.Questionnaires.Remove(questionnaire);
        await context.SaveChangesAsync(ct);
    }
}
