using FluentValidation;
using InquisitorAI.Features.Shared;
using InquisitorAI.Features.Users.Commands;
using InquisitorAI.Features.Users.Dtos;
using InquisitorAI.Features.Users.Endpoints;
using InquisitorAI.Features.Users.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace InquisitorAI.Features.Users;

public static class Setup
{
    public static IServiceCollection AddUsersFeature(this IServiceCollection services)
    {
        // Command handlers
        services.AddScoped<ICommandHandler<UpdateUserProfileCommand, UserDto?>, UpdateUserProfileHandler>();

        // Query handlers
        services.AddScoped<IQueryHandler<GetCurrentUserQuery, UserDto?>, GetCurrentUserHandler>();

        // Validators
        services.AddScoped<IValidator<UpdateUserProfileCommand>, UpdateUserProfileValidator>();

        return services;
    }

    public static WebApplication MapUsersEndpoints(this WebApplication app)
    {
        app.MapGetCurrentUserEndpoint();
        app.MapUpdateUserProfileEndpoint();

        return app;
    }
}
