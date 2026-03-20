using System.Text;
using InquisitorAI.Features.InterviewSessions;
using InquisitorAI.Features.InterviewSessions.Dtos;

namespace InquisitorAI.Infrastructure.Services;

public class MarkdownReportGeneratorService : IReportGeneratorService
{
    public string Generate(InterviewSessionDto session)
    {
        var sb = new StringBuilder();

        // Header
        sb.AppendLine($"# Interview Report: {session.QuestionnaireName}");
        sb.AppendLine();
        sb.AppendLine($"- **Date:** {session.StartedAt:yyyy-MM-dd HH:mm} UTC");

        if (session.DurationSeconds.HasValue)
        {
            var duration = TimeSpan.FromSeconds(session.DurationSeconds.Value);
            sb.AppendLine($"- **Duration:** {duration.Minutes}m {duration.Seconds}s");
        }

        if (session.FinalScore.HasValue)
            sb.AppendLine($"- **Final Score:** {session.FinalScore.Value:F2} / 10");

        if (session.Classification is not null)
            sb.AppendLine($"- **Classification:** {FormatClassification(session.Classification)}");

        sb.AppendLine();

        // Summary
        sb.AppendLine("## Summary");
        sb.AppendLine();
        sb.AppendLine($"The candidate completed **{session.Answers.Count}** question(s) with a final score of **{session.FinalScore?.ToString("F2") ?? "N/A"}** out of 10.");
        sb.AppendLine($"Classification: **{FormatClassification(session.Classification ?? "N/A")}**.");
        sb.AppendLine();

        // Per-Question Details
        if (session.Answers.Count > 0)
        {
            sb.AppendLine("## Question Details");
            sb.AppendLine();

            for (var i = 0; i < session.Answers.Count; i++)
            {
                var answer = session.Answers[i];
                sb.AppendLine($"### Question {i + 1}: {answer.QuestionText}");
                sb.AppendLine();
                sb.AppendLine($"**Ideal Answer:** {answer.IdealAnswer}");
                sb.AppendLine();
                sb.AppendLine($"**Candidate's Answer:** {answer.Transcript ?? "N/A"}");
                sb.AppendLine();
                sb.AppendLine($"**Score:** {(answer.Score.HasValue ? $"{answer.Score.Value:F1}" : "N/A")} / 10");
                sb.AppendLine();

                if (!string.IsNullOrWhiteSpace(answer.AiFeedback))
                {
                    sb.AppendLine($"**Feedback:** {answer.AiFeedback}");
                    sb.AppendLine();
                }

                if (!string.IsNullOrWhiteSpace(answer.Strengths))
                {
                    sb.AppendLine($"**Strengths:** {answer.Strengths}");
                    sb.AppendLine();
                }

                if (!string.IsNullOrWhiteSpace(answer.Weaknesses))
                {
                    sb.AppendLine($"**Weaknesses:** {answer.Weaknesses}");
                    sb.AppendLine();
                }

                if (!string.IsNullOrWhiteSpace(answer.ImprovementSuggestions))
                {
                    sb.AppendLine($"**Improvement Suggestions:** {answer.ImprovementSuggestions}");
                    sb.AppendLine();
                }

                sb.AppendLine("---");
                sb.AppendLine();
            }
        }

        // Overall Assessment
        sb.AppendLine("## Overall Assessment");
        sb.AppendLine();

        if (session.Answers.Count > 0)
        {
            var avgScore = session.FinalScore?.ToString("F2") ?? "N/A";
            var scoredAnswers = session.Answers.Where(a => a.Score.HasValue).ToList();
            var maxScore = scoredAnswers.Count > 0 ? scoredAnswers.Max(a => a.Score!.Value) : (decimal?)null;
            var minScore = scoredAnswers.Count > 0 ? scoredAnswers.Min(a => a.Score!.Value) : (decimal?)null;

            sb.AppendLine($"- **Average Score:** {avgScore} / 10");
            sb.AppendLine($"- **Highest Score:** {(maxScore.HasValue ? $"{maxScore.Value:F1}" : "N/A")} / 10");
            sb.AppendLine($"- **Lowest Score:** {(minScore.HasValue ? $"{minScore.Value:F1}" : "N/A")} / 10");
            sb.AppendLine($"- **Questions Answered:** {session.Answers.Count}");
        }
        else
        {
            sb.AppendLine("No answers were submitted for this session.");
        }

        return sb.ToString();
    }

    private static string FormatClassification(string classification)
    {
        return classification switch
        {
            "Approved" => "Approved",
            "ApprovedWithReservations" => "Approved with Reservations",
            "Failed" => "Failed",
            _ => classification
        };
    }
}
