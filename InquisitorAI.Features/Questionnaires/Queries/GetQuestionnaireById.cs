using System.Data;
using Dapper;
using InquisitorAI.Features.Questionnaires.Dtos;
using InquisitorAI.Features.Shared;

namespace InquisitorAI.Features.Questionnaires.Queries;

public record GetQuestionnaireByIdQuery(long Id, long? RequestingUserId) : IQuery<QuestionnaireDto?>;

public class GetQuestionnaireByIdHandler(IDbConnection db)
    : IQueryHandler<GetQuestionnaireByIdQuery, QuestionnaireDto?>
{
    public async Task<QuestionnaireDto?> HandleAsync(GetQuestionnaireByIdQuery query, CancellationToken ct = default)
    {
        const string questionnaireSql = """
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
            WHERE q.id = @Id
              AND (q.is_public = true OR (@RequestingUserId IS NOT NULL AND q.created_by_user_id = @RequestingUserId))
            GROUP BY q.id, q.name, q.created_by_user_id, u.display_name, q.is_public, q.created_at
            """;

        return await db.QuerySingleOrDefaultAsync<QuestionnaireDto>(
            questionnaireSql,
            new { query.Id, query.RequestingUserId });
    }
}
