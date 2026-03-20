using System.ComponentModel.DataAnnotations;
using InquisitorAI.Features.Users.Domain;

namespace InquisitorAI.Features.Questionnaires.Domain;

public class Questionnaire
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long CreatedByUserId { get; set; }
    public bool IsPublic { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = [];

    public User User { get; set; } = null!;
    public ICollection<Question> Questions { get; set; } = [];
}
