using System.Security.Claims;
using InquisitorAI.Features.Auth.Commands;
using InquisitorAI.Features.Auth.Dtos;
using InquisitorAI.Features.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;

namespace InquisitorAI.Features.Auth.Endpoints;

public static class OAuthCallbackEndpoint
{
    public static void MapOAuthCallbackEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/google/callback", HandleGoogleCallback).AllowAnonymous();
        app.MapGet("/auth/github/callback", HandleGitHubCallback).AllowAnonymous();
        app.MapGet("/auth/linkedin/callback", HandleLinkedInCallback).AllowAnonymous();
    }

    public static async Task<IResult> HandleGoogleCallback(
        HttpContext httpContext,
        ICommandHandler<IssueTokensCommand, TokenResponseDto?> handler,
        NotificationHandler notifications,
        IConfiguration config,
        CancellationToken ct)
    {
        return await HandleCallback(httpContext, handler, notifications, config, "Google", ct);
    }

    public static async Task<IResult> HandleGitHubCallback(
        HttpContext httpContext,
        ICommandHandler<IssueTokensCommand, TokenResponseDto?> handler,
        NotificationHandler notifications,
        IConfiguration config,
        CancellationToken ct)
    {
        return await HandleCallback(httpContext, handler, notifications, config, "GitHub", ct);
    }

    public static async Task<IResult> HandleLinkedInCallback(
        HttpContext httpContext,
        ICommandHandler<IssueTokensCommand, TokenResponseDto?> handler,
        NotificationHandler notifications,
        IConfiguration config,
        CancellationToken ct)
    {
        return await HandleCallback(httpContext, handler, notifications, config, "LinkedIn", ct);
    }

    private static async Task<IResult> HandleCallback(
        HttpContext httpContext,
        ICommandHandler<IssueTokensCommand, TokenResponseDto?> handler,
        NotificationHandler notifications,
        IConfiguration config,
        string provider,
        CancellationToken ct)
    {
        var authResult = await httpContext.AuthenticateAsync("External");
        if (!authResult.Succeeded)
            return Results.BadRequest(new { errors = new[] { "External authentication failed." } });

        var principal = authResult.Principal!;

        var externalId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = principal.FindFirstValue(ClaimTypes.Email);
        var displayName = principal.FindFirstValue(ClaimTypes.Name);
        var avatarUrl = principal.FindFirstValue("urn:oauth:picture");

        if (string.IsNullOrEmpty(externalId) || string.IsNullOrEmpty(email))
        {
            return Results.BadRequest(new { errors = new[] { "Missing required claims from OAuth provider." } });
        }

        var command = new IssueTokensCommand(provider, externalId, email, displayName ?? email, avatarUrl);
        var result = await handler.HandleAsync(command, ct);

        await httpContext.SignOutAsync("External");

        if (notifications.HasErrors)
        {
            return Results.BadRequest(new { errors = notifications.Errors });
        }

        // Check if this is a native flow (loopback redirect)
        var loopbackUri = authResult.Properties?.Items["loopback_uri"];
        if (!string.IsNullOrEmpty(loopbackUri))
        {
            var separator = loopbackUri.Contains('?') ? "&" : "?";
            var redirectUrl = $"{loopbackUri}{separator}access_token={result!.AccessToken}&refresh_token={result.RefreshToken}";
            return Results.Redirect(redirectUrl);
        }

        // Web flow: redirect to web portal
        var webPortalUrl = config["WebPortalUrl"] ?? "http://localhost:4200";
        var webRedirectUrl = $"{webPortalUrl}/auth/callback?token={result!.AccessToken}&refresh_token={result.RefreshToken}";
        return Results.Redirect(webRedirectUrl);
    }
}
