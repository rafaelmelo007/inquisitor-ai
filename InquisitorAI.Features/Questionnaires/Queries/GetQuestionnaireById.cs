using System.Data;
using Dapper;
using InquisitorAI.Features.Questionnaires.Dtos;
using InquisitorAI.Features.Shared;

namespace InquisitorAI.Features.Questionnaires.Queries;

public record GetQuestionnaireByIdQuery(long Id, long? RequestingUserId) : IQuery<QuestionnaireDetailDto?>;

public class GetQuestionnaireByIdHandler(IDbConnection db)
    : IQueryHandler<GetQuestionnaireByIdQuery, QuestionnaireDetailDto?>
{
    public async Task<QuestionnaireDetailDto?> HandleAsync(GetQuestionnaireByIdQuery query, CancellationToken ct = default)
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

        var questionnaire = await db.QuerySingleOrDefaultAsync<QuestionnaireDto>(
            questionnaireSql,
            new { query.Id, query.RequestingUserId });

        if (questionnaire is null)
            return null;

        const string questionsSql = """
            SELECT
                id AS Id,
                questionnaire_id AS QuestionnaireId,
                order_index AS OrderIndex,
                category AS Category,
                difficulty AS Difficulty,
                question_text AS QuestionText,
                ideal_answer AS IdealAnswer
            FROM inq_questions
            WHERE questionnaire_id = @Id
            ORDER BY order_index
            """;

        var questions = (await db.QueryAsync<QuestionDto>(questionsSql, new { query.Id })).ToList();

        return new QuestionnaireDetailDto(
            questionnaire.Id,
            questionnaire.Name,
            questionnaire.CreatedByUserId,
            questionnaire.CreatedByDisplayName,
            questionnaire.IsPublic,
            questionnaire.QuestionCount,
            questionnaire.CreatedAt,
            questions);
    }
}
