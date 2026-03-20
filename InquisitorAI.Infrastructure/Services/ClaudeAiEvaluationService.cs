using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using InquisitorAI.Features.InterviewSessions;
using InquisitorAI.Features.InterviewSessions.Dtos;
using Microsoft.Extensions.Configuration;

namespace InquisitorAI.Infrastructure.Services;

public class ClaudeAiEvaluationService(HttpClient httpClient, IConfiguration configuration) : IAiEvaluationService
{
    private const string Model = "claude-sonnet-4-20250514";
    private const string ApiUrl = "https://api.anthropic.com/v1/messages";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<EvaluationResultDto> EvaluateAsync(EvaluateAnswerRequest request, CancellationToken ct = default)
    {
        var apiKey = configuration["Anthropic:ApiKey"]
            ?? throw new InvalidOperationException("Anthropic:ApiKey is not configured.");

        var prompt = BuildPrompt(request);

        var requestBody = new
        {
            model = Model,
            max_tokens = 1024,
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, ApiUrl);
        httpRequest.Headers.Add("x-api-key", apiKey);
        httpRequest.Headers.Add("anthropic-version", "2023-06-01");
        httpRequest.Content = JsonContent.Create(requestBody);

        using var response = await httpClient.SendAsync(httpRequest, ct);
        response.EnsureSuccessStatusCode();

        var claudeResponse = await response.Content.ReadFromJsonAsync<ClaudeResponse>(JsonOptions, ct)
            ?? throw new InvalidOperationException("Failed to deserialize Claude API response.");

        var textContent = claudeResponse.Content?.FirstOrDefault(c => c.Type == "text")?.Text
            ?? throw new InvalidOperationException("No text content in Claude API response.");

        var jsonText = ExtractJson(textContent);
        var evaluation = JsonSerializer.Deserialize<EvaluationJsonResponse>(jsonText, JsonOptions)
            ?? throw new InvalidOperationException("Failed to parse evaluation JSON from Claude response.");

        return new EvaluationResultDto(
            Score: evaluation.Score,
            Summary: evaluation.Summary ?? string.Empty,
            Strengths: evaluation.Strengths ?? string.Empty,
            Weaknesses: evaluation.Weaknesses ?? string.Empty,
            ImprovementSuggestions: evaluation.ImprovementSuggestions ?? string.Empty);
    }

    private static string BuildPrompt(EvaluateAnswerRequest request)
    {
        return $"""
            You are an expert interviewer evaluating a candidate's answer.

            **Question:** {request.QuestionText}

            **Ideal Answer:** {request.IdealAnswer}

            **Candidate's Answer (Transcript):** {request.Transcript}

            Evaluate the candidate's answer and respond with ONLY a JSON object (no markdown, no code fences) with the following fields:
            - "Score": a number from 0 to 10 (decimal allowed, e.g. 7.5)
            - "Summary": a brief overall assessment of the answer
            - "Strengths": specific strengths identified in the answer (or null if none)
            - "Weaknesses": specific weaknesses identified in the answer (or null if none)
            - "ImprovementSuggestions": concrete suggestions for improving the answer (or null if none)
            """;
    }

    private static string ExtractJson(string text)
    {
        var trimmed = text.Trim();

        // Handle markdown code fences
        if (trimmed.StartsWith("```"))
        {
            var firstNewline = trimmed.IndexOf('\n');
            if (firstNewline >= 0)
                trimmed = trimmed[(firstNewline + 1)..];

            var lastFence = trimmed.LastIndexOf("```", StringComparison.Ordinal);
            if (lastFence >= 0)
                trimmed = trimmed[..lastFence];

            return trimmed.Trim();
        }

        // Try to find JSON object boundaries
        var start = trimmed.IndexOf('{');
        var end = trimmed.LastIndexOf('}');
        if (start >= 0 && end > start)
            return trimmed[start..(end + 1)];

        return trimmed;
    }

    private sealed class ClaudeResponse
    {
        [JsonPropertyName("content")]
        public List<ContentBlock>? Content { get; set; }
    }

    private sealed class ContentBlock
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }

    private sealed class EvaluationJsonResponse
    {
        public decimal Score { get; set; }
        public string? Summary { get; set; }
        public string? Strengths { get; set; }
        public string? Weaknesses { get; set; }
        public string? ImprovementSuggestions { get; set; }
    }
}
