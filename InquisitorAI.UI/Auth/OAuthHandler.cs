using System.Diagnostics;
using System.Net;
using System.Web;
using InquisitorAI.UI.Dtos;
using InquisitorAI.UI.Services.Api;
using Microsoft.Extensions.Configuration;

namespace InquisitorAI.UI.Auth;

public enum OAuthProvider
{
    Google,
    GitHub,
    LinkedIn
}

public class OAuthHandler
{
    private readonly ITokenStore _tokenStore;
    private readonly string _apiBaseUrl;

    public OAuthHandler(ITokenStore tokenStore, IConfiguration configuration)
    {
        _tokenStore = tokenStore;
        _apiBaseUrl = configuration["ApiBaseUrl"]
            ?? throw new InvalidOperationException("ApiBaseUrl is not configured.");
    }

    public async Task<TokenResponseDto?> AuthenticateAsync(OAuthProvider provider)
    {
        var port = GetAvailablePort();
        var redirectUri = $"http://localhost:{port}/callback";
        var providerName = provider.ToString().ToLower();
        var authUrl = $"{_apiBaseUrl}/auth/{providerName}?redirect_uri={Uri.EscapeDataString(redirectUri)}";

        using var listener = new HttpListener();
        listener.Prefixes.Add($"http://localhost:{port}/");
        listener.Start();

        Process.Start(new ProcessStartInfo
        {
            FileName = authUrl,
            UseShellExecute = true
        });

        try
        {
            var context = await listener.GetContextAsync();
            var query = context.Request.Url?.Query;

            if (string.IsNullOrEmpty(query))
            {
                await SendResponseAsync(context, "Authentication failed. No parameters received.");
                return null;
            }

            var queryParams = HttpUtility.ParseQueryString(query);
            var accessToken = queryParams["access_token"];
            var refreshToken = queryParams["refresh_token"];
            var expiresIn = int.TryParse(queryParams["expires_in"], out var exp) ? exp : 3600;

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                await SendResponseAsync(context, "Authentication failed. Missing tokens.");
                return null;
            }

            _tokenStore.SaveTokens(accessToken, refreshToken);
            await SendResponseAsync(context, "Authentication successful! You can close this window.");

            return new TokenResponseDto(accessToken, refreshToken, expiresIn);
        }
        finally
        {
            listener.Stop();
        }
    }

    private static async Task SendResponseAsync(HttpListenerContext context, string message)
    {
        var html = $"""
            <html><body style="font-family:sans-serif;text-align:center;margin-top:50px;">
            <h2>{message}</h2></body></html>
            """;
        var buffer = System.Text.Encoding.UTF8.GetBytes(html);
        context.Response.ContentType = "text/html";
        context.Response.ContentLength64 = buffer.Length;
        await context.Response.OutputStream.WriteAsync(buffer);
        context.Response.Close();
    }

    private static int GetAvailablePort()
    {
        using var listener = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
