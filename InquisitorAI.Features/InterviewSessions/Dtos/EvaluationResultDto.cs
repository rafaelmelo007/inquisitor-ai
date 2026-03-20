namespace InquisitorAI.Features.InterviewSessions.Dtos;

public record EvaluationResultDto(
    decimal Score,
    string Summary,
    string Strengths,
    string Weaknesses,
    string ImprovementSuggestions);
