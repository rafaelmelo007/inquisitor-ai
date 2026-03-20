using Meziantou.Framework.Win32;

namespace InquisitorAI.UI.Services.Api;

public class WindowsCredentialTokenStore : ITokenStore
{
    private const string AccessTokenTarget = "InquisitorAI_AccessToken";
    private const string RefreshTokenTarget = "InquisitorAI_RefreshToken";

    public string? GetAccessToken()
    {
        var credential = CredentialManager.ReadCredential(AccessTokenTarget);
        return credential?.Password;
    }

    public string? GetRefreshToken()
    {
        var credential = CredentialManager.ReadCredential(RefreshTokenTarget);
        return credential?.Password;
    }

    public void SaveTokens(string accessToken, string refreshToken)
    {
        CredentialManager.WriteCredential(
            AccessTokenTarget,
            "InquisitorAI",
            accessToken,
            CredentialPersistence.LocalMachine);

        CredentialManager.WriteCredential(
            RefreshTokenTarget,
            "InquisitorAI",
            refreshToken,
            CredentialPersistence.LocalMachine);
    }

    public void Clear()
    {
        try { CredentialManager.DeleteCredential(AccessTokenTarget); } catch { }
        try { CredentialManager.DeleteCredential(RefreshTokenTarget); } catch { }
    }
}
