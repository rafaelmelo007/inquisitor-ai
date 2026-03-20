using InquisitorAI.Features.Auth;
using InquisitorAI.Features.InterviewSessions;
using InquisitorAI.Features.Questionnaires;
using InquisitorAI.Features.Scores;
using InquisitorAI.Features.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InquisitorAI.Features;

public static class Setup
{
    public static IServiceCollection RegisterAllFeatures(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthFeature(config);
        services.AddUsersFeature();
        services.AddQuestionnairesFeature();
        services.AddInterviewSessionsFeature(config);
        services.AddScoresFeature();
        return services;
    }

    public static WebApplication MapAllEndpoints(this WebApplication app)
    {
        app.MapAuthEndpoints();
        app.MapUsersEndpoints();
        app.MapQuestionnairesEndpoints();
        app.MapInterviewSessionsEndpoints();
        app.MapScoresEndpoints();
        return app;
    }
}
