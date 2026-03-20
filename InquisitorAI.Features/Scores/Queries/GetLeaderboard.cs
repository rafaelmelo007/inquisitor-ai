using System.Data;
using Dapper;
using InquisitorAI.Features.Scores.Dtos;
using InquisitorAI.Features.Shared;

namespace InquisitorAI.Features.Scores.Queries;

public record GetLeaderboardQuery(int Top = 50) : IQuery<IEnumerable<LeaderboardEntryDto>>;

public class GetLeaderboardHandler(IDbConnection db)
    : IQueryHandler<GetLeaderboardQuery, IEnumerable<LeaderboardEntryDto>>
{
    public async Task<IEnumerable<LeaderboardEntryDto>> HandleAsync(GetLeaderboardQuery query, CancellationToken ct = default)
    {
        const string sql = """
            SELECT
                ROW_NUMBER() OVER (ORDER BY MAX(s.final_score) DESC) AS Rank,
                u.id AS UserId,
                u.display_name AS DisplayName,
                u.avatar_url AS AvatarUrl,
                MAX(s.final_score) AS BestScore,
                COUNT(s.id) AS SessionCount,
                ROUND(AVG(s.final_score), 2) AS AverageScore
            FROM inq_interview_sessions s
            INNER JOIN inq_users u ON u.id = s.user_id
            WHERE s.final_score IS NOT NULL
            GROUP BY u.id, u.display_name, u.avatar_url
            ORDER BY BestScore DESC
            LIMIT @Top
            """;

        return await db.QueryAsync<LeaderboardEntryDto>(sql, new { query.Top });
    }
}
