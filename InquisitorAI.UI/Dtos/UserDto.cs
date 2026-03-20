namespace InquisitorAI.UI.Dtos;

public record UserDto(long Id, string Email, string DisplayName, string? AvatarUrl, string Provider);
