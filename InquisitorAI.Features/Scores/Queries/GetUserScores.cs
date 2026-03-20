using System.Data;
using Dapper;
using InquisitorAI.Features.Scores.Dtos;
using InquisitorAI.Features.Shared;

namespace InquisitorAI.Features.Scores.Queries;

public record GetUserScoresQuery(long UserId) : IQuery<UserScoreSummaryDto?>;

public class GetUserScoresHandler(IDbConnection db)
    : IQueryHandler<GetUserScoresQuery, UserScoreSummaryDto?>
{
    public async Task<UserScoreSummaryDto?> HandleAsync(GetUserScoresQuery query, CancellationToken ct = default)
    {
        const string sql = """
            SELECT
                u.id AS UserId,
                u.display_name AS DisplayName,
                COUNT(s.id) AS TotalSessions,
                COALESCE(ROUND(AVG(s.final_score), 2), 0) AS AverageScore,
                COALESCE(MAX(s.final_score), 0) AS BestScore,
                MAX(s.started_at) AS LastSessionAt
            FROM inq_users u
            LEFT JOIN inq_interview_sessions s ON s.user_id = u.id AND s.final_score IS NOT NULL
            WHERE u.id = @UserId
            GROUP BY u.id, u.display_name
            """;

        return await db.QuerySingleOrDefaultAsync<UserScoreSummaryDto>(sql, new { query.UserId });
    }
}
