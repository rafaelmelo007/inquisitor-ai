using System.Diagnostics;
using System.Text;
using System.Threading.RateLimiting;
using InquisitorAI.Features;
using InquisitorAI.Features.Shared;
using InquisitorAI.Infrastructure;
using InquisitorAI.Infrastructure.Setup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .WriteTo.File("/logs/inquisitor-ai/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    // Load .env from solution root — walk up from current directory until found
    var dir = Directory.GetCurrentDirectory();
    while (dir is not null && !File.Exists(Path.Combine(dir, ".env")))
        dir = Directory.GetParent(dir)?.FullName;
    if (dir is not null)
        DotNetEnv.Env.Load(Path.Combine(dir, ".env"));

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();
    builder.Configuration.AddEnvironmentVariables();

    var config = builder.Configuration;

    builder.Services.AddInfrastructure(config);
    builder.Services.RegisterAllFeatures(config);
    builder.Services.AddScoped<NotificationHandler>();

    var authBuilder = builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddCookie("External")
        .AddJwtBearer(options =>
        {
            var secret = config["Jwt:Secret"]
                ?? throw new InvalidOperationException("Jwt:Secret is not configured.");

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
            };
        });

    if (!string.IsNullOrEmpty(config["Authentication:Google:ClientId"]))
    {
        authBuilder.AddGoogle(options =>
        {
            options.SignInScheme = "External";
            options.ClientId = config["Authentication:Google:ClientId"]!;
            options.ClientSecret = config["Authentication:Google:ClientSecret"]!;
        });
    }

    if (!string.IsNullOrEmpty(config["Authentication:GitHub:ClientId"]))
    {
        authBuilder.AddGitHub(options =>
        {
            options.SignInScheme = "External";
            options.ClientId = config["Authentication:GitHub:ClientId"]!;
            options.ClientSecret = config["Authentication:GitHub:ClientSecret"]!;
        });
    }

    if (!string.IsNullOrEmpty(config["Authentication:LinkedIn:ClientId"]))
    {
        authBuilder.AddLinkedIn(options =>
        {
            options.SignInScheme = "External";
            options.ClientId = config["Authentication:LinkedIn:ClientId"]!;
            options.ClientSecret = config["Authentication:LinkedIn:ClientSecret"]!;
        });
    }

    builder.Services.AddAuthorization();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            RateLimitPartition.GetFixedWindowLimiter("fixed", _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
    });

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins(config.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [])
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    builder.Services.AddHealthChecks()
        .AddDbContextCheck<AppDbContext>("database")
        .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());

    var app = builder.Build();

    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var correlationId = Activity.Current?.Id ?? context.TraceIdentifier;
            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

            Log.Error(exception, "Unhandled exception. CorrelationId: {CorrelationId}", correlationId);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                error = "An unexpected error occurred.",
                correlationId
            });
        });
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging();
    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseRateLimiter();

    app.MapAllEndpoints();

    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = _ => true
    });

    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = check => check.Name == "self"
    });

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    await Log.CloseAndFlushAsync();
}
