namespace InquisitorAI.Features.Scores.Dtos;

public record LeaderboardEntryDto(
    int Rank,
    long UserId,
    string DisplayName,
    string? AvatarUrl,
    decimal BestScore,
    int SessionCount,
    decimal AverageScore);
