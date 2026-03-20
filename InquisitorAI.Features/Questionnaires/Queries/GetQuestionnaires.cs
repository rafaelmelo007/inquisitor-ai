using System.Data;
using Dapper;
using InquisitorAI.Features.Questionnaires.Dtos;
using InquisitorAI.Features.Shared;

namespace InquisitorAI.Features.Questionnaires.Queries;

public record GetQuestionnairesQuery(long? UserId) : IQuery<IEnumerable<QuestionnaireDto>>;

public class GetQuestionnairesHandler(IDbConnection db)
    : IQueryHandler<GetQuestionnairesQuery, IEnumerable<QuestionnaireDto>>
{
    public async Task<IEnumerable<QuestionnaireDto>> HandleAsync(GetQuestionnairesQuery query, CancellationToken ct = default)
    {
        const string sql = """
            SELECT
                q.id AS Id,
                q.name AS Name,
                q.created_by_user_id AS CreatedByUserId,
                u.display_name AS CreatedByDisplayName,
                q.is_public AS IsPublic,
                COUNT(qu.id) AS QuestionCount,
                q.created_at AS CreatedAt
            FROM inq_questionnaires q
            INNER JOIN inq_users u ON u.id = q.created_by_user_id
            LEFT JOIN inq_questions qu ON qu.questionnaire_id = q.id
            WHERE q.is_public = true
               OR (@UserId IS NOT NULL AND q.created_by_user_id = @UserId)
            GROUP BY q.id, q.name, q.created_by_user_id, u.display_name, q.is_public, q.created_at
            ORDER BY q.created_at DESC
            """;

        return await db.QueryAsync<QuestionnaireDto>(sql, new { query.UserId });
    }
}
