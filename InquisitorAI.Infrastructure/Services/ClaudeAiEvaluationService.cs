using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using InquisitorAI.Features.InterviewSessions;
using InquisitorAI.Features.InterviewSessions.Dtos;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace InquisitorAI.Infrastructure.Services;

public class ClaudeAiEvaluationService(HttpClient httpClient, IConfiguration configuration) : IAiEvaluationService
{
    private static readonly ILogger AiLogger = Log.ForContext("AiInteraction", true);
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

        AiLogger.Information(
            "[AI REQUEST] Model={Model} Question={Question} IdealAnswer={IdealAnswer} CandidateTranscript={Transcript} FullPrompt={Prompt}",
            Model, request.QuestionText, request.IdealAnswer, request.Transcript, prompt);

        var requestBody = new
        {
            model = Model,
            max_tokens = 4096,
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

        AiLogger.Information(
            "[AI RESPONSE] RawResponse={RawResponse}",
            textContent);

        var jsonText = ExtractJson(textContent);
        var evaluation = JsonSerializer.Deserialize<EvaluationJsonResponse>(jsonText, JsonOptions)
            ?? throw new InvalidOperationException("Failed to parse evaluation JSON from Claude response.");

        var result = new EvaluationResultDto(
            Score: evaluation.Score,
            Summary: evaluation.Summary ?? string.Empty,
            Strengths: evaluation.Strengths is { Count: > 0 } ? string.Join("\n", evaluation.Strengths) : string.Empty,
            Weaknesses: evaluation.Weaknesses is { Count: > 0 } ? string.Join("\n", evaluation.Weaknesses) : string.Empty,
            ImprovementSuggestions: evaluation.ImprovementSuggestions is { Count: > 0 } ? string.Join("\n", evaluation.ImprovementSuggestions) : string.Empty);

        AiLogger.Information(
            "[AI PARSED] Score={Score} Summary={Summary} Strengths={Strengths} Weaknesses={Weaknesses} Suggestions={Suggestions}",
            result.Score, result.Summary, result.Strengths, result.Weaknesses, result.ImprovementSuggestions);

        return result;
    }

    private static string BuildPrompt(EvaluateAnswerRequest request)
    {
        return $"""
            You are an expert technical mentor evaluating a candidate's answer to an interview question.
            Your goal is not just to grade — it is to **teach**. Help the candidate truly understand the topic.

            **Question:** {request.QuestionText}

            **Ideal Answer:** {request.IdealAnswer}

            **Candidate's Answer (Transcript):** {request.Transcript}

            Evaluate the candidate's answer and respond with ONLY a JSON object (no markdown, no code fences) with the following fields:
            - "Score": a number from 0 to 10 (decimal allowed, e.g. 7.5)
            - "Summary": a thorough educational explanation that:
              1. Explains the correct/complete answer to the question so the candidate learns the topic.
              2. Points out what the candidate got right and wrong, and why.
              3. Clarifies any misconceptions from the candidate's answer.
              Keep this detailed (2-4 paragraphs). Write as if you are teaching, not just judging.
            - "Strengths": an array of strings listing specific strengths (or null if none)
            - "Weaknesses": an array of strings listing specific weaknesses (or null if none)
            - "ImprovementSuggestions": an array of strings with concrete, actionable suggestions for improvement (or null if none)
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
        public List<string>? Strengths { get; set; }
        public List<string>? Weaknesses { get; set; }
        public List<string>? ImprovementSuggestions { get; set; }
    }
}
