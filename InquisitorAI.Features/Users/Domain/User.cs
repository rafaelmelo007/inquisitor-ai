using InquisitorAI.Features.Auth.Domain;
using InquisitorAI.Features.InterviewSessions.Domain;
using InquisitorAI.Features.Questionnaires.Domain;

namespace InquisitorAI.Features.Users.Domain;

public class User
{
    public long Id { get; set; }
    public OAuthProvider Provider { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public uint RowVersion { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<Questionnaire> Questionnaires { get; set; } = [];
    public ICollection<InterviewSession> InterviewSessions { get; set; } = [];
}
