namespace InquisitorAI.Features.Scores.Dtos;

public record UserScoreSummaryDto(
    long UserId,
    string DisplayName,
    int TotalSessions,
    decimal AverageScore,
    decimal BestScore,
    DateTimeOffset? LastSessionAt);
