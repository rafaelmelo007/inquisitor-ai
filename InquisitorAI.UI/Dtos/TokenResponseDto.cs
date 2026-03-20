namespace InquisitorAI.UI.Dtos;

public record TokenResponseDto(string AccessToken, string RefreshToken, int ExpiresIn);
