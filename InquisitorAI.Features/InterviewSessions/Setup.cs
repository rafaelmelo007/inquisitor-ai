using InquisitorAI.Features.InterviewSessions.Commands;
using InquisitorAI.Features.InterviewSessions.Dtos;
using InquisitorAI.Features.InterviewSessions.Endpoints;
using InquisitorAI.Features.InterviewSessions.Queries;
using InquisitorAI.Features.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InquisitorAI.Features.InterviewSessions;

public static class Setup
{
    public static IServiceCollection AddInterviewSessionsFeature(this IServiceCollection services, IConfiguration config)
    {
        // Command handlers
        services.AddScoped<ICommandHandler<StartInterviewSessionCommand, InterviewSessionDto?>, StartInterviewSessionHandler>();
        services.AddScoped<ICommandHandler<SubmitAnswerCommand, SessionAnswerDto?>, SubmitAnswerHandler>();
        services.AddScoped<ICommandHandler<FinishInterviewSessionCommand, FinalResultDto?>, FinishInterviewSessionHandler>();
        services.AddScoped<ICommandHandler<DeleteInterviewSessionCommand>, DeleteInterviewSessionHandler>();

        // Query handlers
        services.AddScoped<IQueryHandler<GetInterviewSessionsQuery, IEnumerable<InterviewSessionDto>>, GetInterviewSessionsHandler>();
        services.AddScoped<IQueryHandler<GetSessionDetailsQuery, InterviewSessionDto?>, GetSessionDetailsHandler>();

        return services;
    }

    public static WebApplication MapInterviewSessionsEndpoints(this WebApplication app)
    {
        StartSessionEndpoint.Map(app);
        SubmitAnswerEndpoint.Map(app);
        FinishSessionEndpoint.Map(app);
        GetSessionsEndpoint.Map(app);
        GetSessionDetailsEndpoint.Map(app);
        DeleteSessionEndpoint.Map(app);
        return app;
    }
}
