using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace InquisitorAI.Features.Auth.Endpoints;

public static class OAuthInitiateEndpoint
{
    private static readonly Dictionary<string, string> _schemeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["google"] = "Google",
        ["github"] = "GitHub",
        ["linkedin"] = "LinkedIn"
    };

    public static void MapOAuthInitiateEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/{provider}", HandleInitiate).AllowAnonymous();
    }

    private static async Task HandleInitiate(string provider, string? redirect_uri, HttpContext ctx)
    {
        if (!_schemeMap.TryGetValue(provider, out var scheme))
        {
            ctx.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        var props = new AuthenticationProperties
        {
            RedirectUri = $"/auth/{provider.ToLower()}/callback"
        };

        if (!string.IsNullOrEmpty(redirect_uri))
            props.Items["loopback_uri"] = redirect_uri;

        await ctx.ChallengeAsync(scheme, props);
    }
}
