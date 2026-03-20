using System.Data;
using Dapper;
using InquisitorAI.Features.InterviewSessions.Dtos;
using InquisitorAI.Features.Shared;

namespace InquisitorAI.Features.InterviewSessions.Queries;

public record GetInterviewSessionsQuery(long UserId) : IQuery<IEnumerable<InterviewSessionDto>>;

public class GetInterviewSessionsHandler(IDbConnection db)
    : IQueryHandler<GetInterviewSessionsQuery, IEnumerable<InterviewSessionDto>>
{
    public async Task<IEnumerable<InterviewSessionDto>> HandleAsync(GetInterviewSessionsQuery query, CancellationToken ct = default)
    {
        const string sql = """
            SELECT
                s.id AS Id,
                s.user_id AS UserId,
                s.questionnaire_id AS QuestionnaireId,
                q.name AS QuestionnaireName,
                s.started_at AS StartedAt,
                s.ended_at AS EndedAt,
                s.duration_seconds AS DurationSeconds,
                s.final_score AS FinalScore,
                s.classification AS Classification,
                s.report_content AS ReportContent
            FROM inq_interview_sessions s
            INNER JOIN inq_questionnaires q ON q.id = s.questionnaire_id
            WHERE s.user_id = @UserId
            ORDER BY s.started_at DESC
            """;

        var sessions = await db.QueryAsync<InterviewSessionRow>(sql, new { query.UserId });

        return sessions.Select(s => new InterviewSessionDto(
            s.Id,
            s.UserId,
            s.QuestionnaireId,
            s.QuestionnaireName,
            s.StartedAt,
            s.EndedAt,
            s.DurationSeconds,
            s.FinalScore,
            s.Classification,
            s.ReportContent,
            []));
    }

    private record InterviewSessionRow(
        long Id,
        long UserId,
        long QuestionnaireId,
        string QuestionnaireName,
        DateTimeOffset StartedAt,
        DateTimeOffset? EndedAt,
        int? DurationSeconds,
        decimal? FinalScore,
        string? Classification,
        string? ReportContent);
}
