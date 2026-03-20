namespace InquisitorAI.Features.Scores.Dtos;

public record LeaderboardEntryDto(
    long Rank,
    long UserId,
    string DisplayName,
    string? AvatarUrl,
    decimal BestScore,
    long SessionCount,
    decimal AverageScore);
