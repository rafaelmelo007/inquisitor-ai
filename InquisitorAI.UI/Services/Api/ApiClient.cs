using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using InquisitorAI.UI.Dtos;

namespace InquisitorAI.UI.Services.Api;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ITokenStore _tokenStore;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiClient(HttpClient httpClient, ITokenStore tokenStore)
    {
        _httpClient = httpClient;
        _tokenStore = tokenStore;
    }

    private void AttachToken(HttpRequestMessage request)
    {
        var token = _tokenStore.GetAccessToken();
        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<T> SendAsync<T>(HttpRequestMessage request)
    {
        AttachToken(request);
        var response = await _httpClient.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            response = await TryRefreshAndRetryAsync(request) ?? response;
        }

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
        return result ?? throw new InvalidOperationException("Received null response from API.");
    }

    private async Task SendAsync(HttpRequestMessage request)
    {
        AttachToken(request);
        var response = await _httpClient.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            response = await TryRefreshAndRetryAsync(request) ?? response;
        }

        response.EnsureSuccessStatusCode();
    }

    private async Task<HttpResponseMessage?> TryRefreshAndRetryAsync(HttpRequestMessage request)
    {
        var refreshToken = _tokenStore.GetRefreshToken();
        if (string.IsNullOrEmpty(refreshToken))
            return null;

        try
        {
            var tokenResponse = await RefreshTokenAsync(refreshToken);
            _tokenStore.SaveTokens(tokenResponse.AccessToken, tokenResponse.RefreshToken);

            request = await CloneRequestAsync(request);
            AttachToken(request);
            return await _httpClient.SendAsync(request);
        }
        catch
        {
            // Refresh failed (expired, revoked, etc.) — clear tokens so user is prompted to re-login
            _tokenStore.Clear();
            return null;
        }
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage original)
    {
        var clone = new HttpRequestMessage(original.Method, original.RequestUri);
        if (original.Content != null)
        {
            var content = await original.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(content);
            if (original.Content.Headers.ContentType != null)
                clone.Content.Headers.ContentType = original.Content.Headers.ContentType;
        }

        foreach (var header in original.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        return clone;
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "auth/refresh")
        {
            Content = JsonContent.Create(new { refreshToken })
        };
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TokenResponseDto>(JsonOptions)
            ?? throw new InvalidOperationException("Failed to refresh token.");
    }

    public async Task LogoutAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "auth/logout");
        await SendAsync(request);
    }

    public async Task<UserDto> GetCurrentUserAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "users/me");
        return await SendAsync<UserDto>(request);
    }

    public async Task<UserDto> UpdateProfileAsync(string displayName, string? avatarUrl)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, "users/me")
        {
            Content = JsonContent.Create(new { displayName, avatarUrl })
        };
        return await SendAsync<UserDto>(request);
    }

    public async Task<IEnumerable<QuestionnaireDto>> GetQuestionnairesAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "questionnaires");
        return await SendAsync<IEnumerable<QuestionnaireDto>>(request);
    }

    public async Task<QuestionnaireDetailDto> GetQuestionnaireByIdAsync(long id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"questionnaires/{id}");
        return await SendAsync<QuestionnaireDetailDto>(request);
    }

    public async Task<QuestionnaireDto> ImportQuestionnaireAsync(string filePath, bool isPublic)
    {
        using var content = new MultipartFormDataContent();
        var fileBytes = await File.ReadAllBytesAsync(filePath);
        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/markdown");
        content.Add(fileContent, "file", Path.GetFileName(filePath));
        content.Add(new StringContent(isPublic.ToString().ToLower()), "isPublic");

        var request = new HttpRequestMessage(HttpMethod.Post, "questionnaires")
        {
            Content = content
        };
        return await SendAsync<QuestionnaireDto>(request);
    }

    public async Task DeleteQuestionnaireAsync(long id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"questionnaires/{id}");
        await SendAsync(request);
    }

    public async Task<InterviewSessionDto> StartSessionAsync(long questionnaireId, string? language = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "sessions")
        {
            Content = JsonContent.Create(new { questionnaireId, language })
        };
        return await SendAsync<InterviewSessionDto>(request);
    }

    public async Task<SessionAnswerDto> SubmitAnswerAsync(long sessionId, long questionId, string transcript)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"sessions/{sessionId}/answers")
        {
            Content = JsonContent.Create(new { questionId, transcript })
        };
        return await SendAsync<SessionAnswerDto>(request);
    }

    public async Task<FinalResultDto> FinishSessionAsync(long sessionId)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"sessions/{sessionId}/finish");
        return await SendAsync<FinalResultDto>(request);
    }

    public async Task<IEnumerable<InterviewSessionDto>> GetSessionsAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "sessions");
        return await SendAsync<IEnumerable<InterviewSessionDto>>(request);
    }

    public async Task<InterviewSessionDto> GetSessionDetailsAsync(long sessionId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"sessions/{sessionId}");
        return await SendAsync<InterviewSessionDto>(request);
    }

    public async Task DeleteSessionAsync(long sessionId)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"sessions/{sessionId}");
        await SendAsync(request);
    }
}
