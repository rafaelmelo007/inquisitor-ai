using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace InquisitorAI.UI.Services.Local;

public class OpenAiWhisperService : ISpeechToTextService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAiWhisperService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenAi:ApiKey"]
            ?? throw new InvalidOperationException("OpenAi:ApiKey is not configured.");
    }

    public async Task<string> TranscribeAsync(string audioFilePath, CancellationToken ct, string? language = null)
    {
        using var content = new MultipartFormDataContent();

        var fileBytes = await File.ReadAllBytesAsync(audioFilePath, ct);
        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");
        content.Add(fileContent, "file", Path.GetFileName(audioFilePath));
        content.Add(new StringContent("whisper-1"), "model");

        var languageCode = GetWhisperLanguageCode(language);
        if (languageCode != null)
            content.Add(new StringContent(languageCode), "language");

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/audio/transcriptions")
        {
            Content = content
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        using var response = await _httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        var result = JsonSerializer.Deserialize<WhisperResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return result?.Text ?? string.Empty;
    }

    private static string? GetWhisperLanguageCode(string? language)
    {
        return language switch
        {
            "Español" or "Spanish" => "es",
            "Português" or "Portuguese" => "pt",
            "English" => "en",
            _ => null
        };
    }

    private sealed class WhisperResponse
    {
        public string Text { get; set; } = string.Empty;
    }
}
