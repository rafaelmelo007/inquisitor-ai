using InquisitorAI.Features.Scores.Dtos;
using InquisitorAI.Features.Scores.Endpoints;
using InquisitorAI.Features.Scores.Queries;
using InquisitorAI.Features.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace InquisitorAI.Features.Scores;

public static class Setup
{
    public static IServiceCollection AddScoresFeature(this IServiceCollection services)
    {
        // Query handlers
        services.AddScoped<IQueryHandler<GetLeaderboardQuery, IEnumerable<LeaderboardEntryDto>>, GetLeaderboardHandler>();
        services.AddScoped<IQueryHandler<GetUserScoresQuery, UserScoreSummaryDto?>, GetUserScoresHandler>();

        return services;
    }

    public static WebApplication MapScoresEndpoints(this WebApplication app)
    {
        GetLeaderboardEndpoint.Map(app);
        GetUserScoresEndpoint.Map(app);
        return app;
    }
}
