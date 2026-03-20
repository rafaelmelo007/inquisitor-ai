using System.ComponentModel.DataAnnotations;
using InquisitorAI.Features.Questionnaires.Domain;
using InquisitorAI.Features.Users.Domain;

namespace InquisitorAI.Features.InterviewSessions.Domain;

public class InterviewSession
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long QuestionnaireId { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }
    public int? DurationSeconds { get; set; }
    public decimal? FinalScore { get; set; }
    public Classification? Classification { get; set; }
    public string? ReportContent { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = [];

    public User User { get; set; } = null!;
    public Questionnaire Questionnaire { get; set; } = null!;
    public ICollection<SessionAnswer> SessionAnswers { get; set; } = [];
}
