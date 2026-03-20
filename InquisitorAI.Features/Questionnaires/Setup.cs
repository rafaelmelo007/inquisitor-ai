using InquisitorAI.Features.Questionnaires.Commands;
using InquisitorAI.Features.Questionnaires.Dtos;
using InquisitorAI.Features.Questionnaires.Endpoints;
using InquisitorAI.Features.Questionnaires.Queries;
using InquisitorAI.Features.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace InquisitorAI.Features.Questionnaires;

public static class Setup
{
    public static IServiceCollection AddQuestionnairesFeature(this IServiceCollection services)
    {
        // Command handlers
        services.AddScoped<ICommandHandler<ImportQuestionnaireCommand, QuestionnaireDto?>, ImportQuestionnaireHandler>();
        services.AddScoped<ICommandHandler<DeleteQuestionnaireCommand>, DeleteQuestionnaireHandler>();

        // Query handlers
        services.AddScoped<IQueryHandler<GetQuestionnairesQuery, IEnumerable<QuestionnaireDto>>, GetQuestionnairesHandler>();
        services.AddScoped<IQueryHandler<GetQuestionnaireByIdQuery, QuestionnaireDto?>, GetQuestionnaireByIdHandler>();

        return services;
    }

    public static WebApplication MapQuestionnairesEndpoints(this WebApplication app)
    {
        ImportQuestionnaireEndpoint.Map(app);
        GetQuestionnairesEndpoint.Map(app);
        GetQuestionnaireByIdEndpoint.Map(app);
        DeleteQuestionnaireEndpoint.Map(app);
        return app;
    }
}
