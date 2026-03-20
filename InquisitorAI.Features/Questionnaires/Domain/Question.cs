using System.ComponentModel.DataAnnotations;

namespace InquisitorAI.Features.Questionnaires.Domain;

public class Question
{
    public long Id { get; set; }
    public long QuestionnaireId { get; set; }
    public int OrderIndex { get; set; }
    public string? Category { get; set; }
    public Difficulty? Difficulty { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string IdealAnswer { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = [];

    public Questionnaire Questionnaire { get; set; } = null!;
}
