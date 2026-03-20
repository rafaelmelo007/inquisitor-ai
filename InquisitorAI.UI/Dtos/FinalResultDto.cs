namespace InquisitorAI.UI.Dtos;

public record FinalResultDto(
    decimal FinalScore,
    string Classification,
    string Strengths,
    string ImprovementAreas,
    string ReportContent);
