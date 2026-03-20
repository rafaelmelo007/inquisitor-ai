using InquisitorAI.Features.Auth.Commands;
using InquisitorAI.Features.Auth.Dtos;
using InquisitorAI.Features.Auth.Endpoints;
using InquisitorAI.Features.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InquisitorAI.Features.Auth;

public static class Setup
{
    public static IServiceCollection AddAuthFeature(this IServiceCollection services, IConfiguration config)
    {
        // Command handlers
        services.AddScoped<ICommandHandler<IssueTokensCommand, TokenResponseDto?>, IssueTokensHandler>();
        services.AddScoped<ICommandHandler<RefreshAccessTokenCommand, TokenResponseDto?>, RefreshAccessTokenHandler>();
        services.AddScoped<ICommandHandler<RevokeRefreshTokenCommand>, RevokeRefreshTokenHandler>();

        return services;
    }

    public static WebApplication MapAuthEndpoints(this WebApplication app)
    {
        app.MapOAuthInitiateEndpoints();
        app.MapOAuthCallbackEndpoints();
        app.MapRefreshTokenEndpoint();
        app.MapLogoutEndpoint();

        return app;
    }
}
