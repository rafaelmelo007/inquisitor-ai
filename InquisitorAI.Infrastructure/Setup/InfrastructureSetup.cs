using System.Data;
using InquisitorAI.Features.InterviewSessions;
using InquisitorAI.Features.Auth;
using InquisitorAI.Features.Questionnaires;
using InquisitorAI.Features.Shared;
using InquisitorAI.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace InquisitorAI.Infrastructure.Setup;

public static class InfrastructureSetup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionStrings:Default is not configured.");

        // EF Core with PostgreSQL
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString, b =>
                b.MigrationsAssembly(typeof(InfrastructureSetup).Assembly.FullName)));

        // Raw IDbConnection for Dapper queries
        services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(connectionString));

        // Services
        services.AddSingleton<IDateTimeService, DateTimeService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IMarkdownParserService, MarkdownParserService>();
        services.AddScoped<IReportGeneratorService, MarkdownReportGeneratorService>();

        // HttpClient for Claude AI evaluation service
        services.AddHttpClient<IAiEvaluationService, ClaudeAiEvaluationService>();

        return services;
    }
}
