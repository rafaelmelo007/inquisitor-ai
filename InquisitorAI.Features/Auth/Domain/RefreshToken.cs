using System.ComponentModel.DataAnnotations;
using InquisitorAI.Features.Users.Domain;

namespace InquisitorAI.Features.Auth.Domain;

public class RefreshToken
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = [];

    public User User { get; set; } = null!;
}
