namespace InquisitorAI.UI.Services.Api;

public interface ITokenStore
{
    string? GetAccessToken();
    string? GetRefreshToken();
    void SaveTokens(string accessToken, string refreshToken);
    void Clear();
}
