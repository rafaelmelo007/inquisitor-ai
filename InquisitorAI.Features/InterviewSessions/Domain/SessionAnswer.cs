namespace InquisitorAI.Features.InterviewSessions.Domain;

public class SessionAnswer
{
    public long Id { get; set; }
    public long SessionId { get; set; }
    public long QuestionId { get; set; }
    public string? Transcript { get; set; }
    public decimal? Score { get; set; }
    public string? AiFeedback { get; set; }
    public string? Strengths { get; set; }
    public string? Weaknesses { get; set; }
    public string? ImprovementSuggestions { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public uint RowVersion { get; set; }

    public InterviewSession Session { get; set; } = null!;
    public Questionnaires.Domain.Question Question { get; set; } = null!;
}
