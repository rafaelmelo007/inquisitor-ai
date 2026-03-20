namespace InquisitorAI.Features.Scores.Dtos;

public record UserScoreSummaryDto(
    long UserId,
    string DisplayName,
    long TotalSessions,
    decimal AverageScore,
    decimal BestScore,
    DateTime? LastSessionAt);
