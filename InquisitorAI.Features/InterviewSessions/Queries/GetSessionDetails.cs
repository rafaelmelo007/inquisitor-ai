using System.Data;
using Dapper;
using InquisitorAI.Features.InterviewSessions.Dtos;
using InquisitorAI.Features.Shared;

namespace InquisitorAI.Features.InterviewSessions.Queries;

public record GetSessionDetailsQuery(long SessionId, long UserId) : IQuery<InterviewSessionDto?>;

public class GetSessionDetailsHandler(IDbConnection db)
    : IQueryHandler<GetSessionDetailsQuery, InterviewSessionDto?>
{
    public async Task<InterviewSessionDto?> HandleAsync(GetSessionDetailsQuery query, CancellationToken ct = default)
    {
        const string sessionSql = """
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
            WHERE s.id = @SessionId AND s.user_id = @UserId
            """;

        const string answersSql = """
            SELECT
                a.id AS Id,
                a.session_id AS SessionId,
                a.question_id AS QuestionId,
                qu.question_text AS QuestionText,
                qu.ideal_answer AS IdealAnswer,
                a.transcript AS Transcript,
                a.score AS Score,
                a.ai_feedback AS AiFeedback,
                a.strengths AS Strengths,
                a.weaknesses AS Weaknesses,
                a.improvement_suggestions AS ImprovementSuggestions
            FROM inq_session_answers a
            INNER JOIN inq_questions qu ON qu.id = a.question_id
            WHERE a.session_id = @SessionId
            ORDER BY qu.order_index
            """;

        var session = await db.QuerySingleOrDefaultAsync<SessionRow>(sessionSql, new { query.SessionId, query.UserId });
        if (session is null)
            return null;

        var answers = await db.QueryAsync<SessionAnswerDto>(answersSql, new { query.SessionId });

        return new InterviewSessionDto(
            session.Id,
            session.UserId,
            session.QuestionnaireId,
            session.QuestionnaireName,
            session.StartedAt,
            session.EndedAt,
            session.DurationSeconds,
            session.FinalScore,
            session.Classification,
            session.ReportContent,
            answers.ToList());
    }

    private record SessionRow(
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
