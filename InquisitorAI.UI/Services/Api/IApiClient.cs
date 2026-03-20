using InquisitorAI.UI.Dtos;

namespace InquisitorAI.UI.Services.Api;

public interface IApiClient
{
    Task<TokenResponseDto> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync();
    Task<UserDto> GetCurrentUserAsync();
    Task<UserDto> UpdateProfileAsync(string displayName, string? avatarUrl);
    Task<IEnumerable<QuestionnaireDto>> GetQuestionnairesAsync();
    Task<QuestionnaireDetailDto> GetQuestionnaireByIdAsync(long id);
    Task<QuestionnaireDto> ImportQuestionnaireAsync(string filePath, bool isPublic);
    Task DeleteQuestionnaireAsync(long id);
    Task<InterviewSessionDto> StartSessionAsync(long questionnaireId);
    Task<SessionAnswerDto> SubmitAnswerAsync(long sessionId, long questionId, string transcript);
    Task<FinalResultDto> FinishSessionAsync(long sessionId);
    Task<IEnumerable<InterviewSessionDto>> GetSessionsAsync();
    Task<InterviewSessionDto> GetSessionDetailsAsync(long sessionId);
    Task DeleteSessionAsync(long sessionId);
}
