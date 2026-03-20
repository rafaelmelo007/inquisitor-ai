namespace InquisitorAI.Features.Users.Dtos;

public record UserDto(
    long Id,
    string Email,
    string DisplayName,
    string? AvatarUrl,
    string Provider,
    DateTimeOffset CreatedAt);
