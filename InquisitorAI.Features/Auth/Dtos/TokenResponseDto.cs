namespace InquisitorAI.Features.Auth.Dtos;

public record TokenResponseDto(string AccessToken, string RefreshToken, int ExpiresIn);
