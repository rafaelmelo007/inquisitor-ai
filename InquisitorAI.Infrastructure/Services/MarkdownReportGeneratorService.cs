using System.Text;
using InquisitorAI.Features.InterviewSessions;
using InquisitorAI.Features.InterviewSessions.Dtos;

namespace InquisitorAI.Infrastructure.Services;

public class MarkdownReportGeneratorService : IReportGeneratorService
{
    public string Generate(InterviewSessionDto session, string? language = null)
    {
        var t = GetTranslations(language);
        var sb = new StringBuilder();

        sb.AppendLine($"# {t.InterviewReport}: {session.QuestionnaireName}");
        sb.AppendLine();
        sb.AppendLine($"- **{t.Date}:** {session.StartedAt:yyyy-MM-dd HH:mm} UTC");

        if (session.DurationSeconds.HasValue)
        {
            var duration = TimeSpan.FromSeconds(session.DurationSeconds.Value);
            sb.AppendLine($"- **{t.Duration}:** {duration.Minutes}m {duration.Seconds}s");
        }

        if (session.FinalScore.HasValue)
            sb.AppendLine($"- **{t.FinalScore}:** {session.FinalScore.Value:F2} / 10");

        if (session.Classification is not null)
            sb.AppendLine($"- **{t.Classification}:** {FormatClassification(session.Classification, language)}");

        sb.AppendLine();

        sb.AppendLine($"## {t.Summary}");
        sb.AppendLine();
        sb.AppendLine(string.Format(t.SummaryText, session.Answers.Count, session.FinalScore?.ToString("F2") ?? "N/A"));
        sb.AppendLine($"{t.Classification}: **{FormatClassification(session.Classification ?? "N/A", language)}**.");
        sb.AppendLine();

        if (session.Answers.Count > 0)
        {
            sb.AppendLine($"## {t.QuestionDetails}");
            sb.AppendLine();

            for (var i = 0; i < session.Answers.Count; i++)
            {
                var answer = session.Answers[i];
                sb.AppendLine($"### {t.Question} {i + 1}: {answer.QuestionText}");
                sb.AppendLine();
                sb.AppendLine($"**{t.IdealAnswer}:** {answer.IdealAnswer}");
                sb.AppendLine();
                sb.AppendLine($"**{t.CandidateAnswer}:** {answer.Transcript ?? "N/A"}");
                sb.AppendLine();
                sb.AppendLine($"**{t.Score}:** {(answer.Score.HasValue ? $"{answer.Score.Value:F1}" : "N/A")} / 10");
                sb.AppendLine();

                if (!string.IsNullOrWhiteSpace(answer.AiFeedback))
                {
                    sb.AppendLine($"**{t.Feedback}:** {answer.AiFeedback}");
                    sb.AppendLine();
                }

                if (!string.IsNullOrWhiteSpace(answer.Strengths))
                {
                    sb.AppendLine($"**{t.Strengths}:** {answer.Strengths}");
                    sb.AppendLine();
                }

                if (!string.IsNullOrWhiteSpace(answer.Weaknesses))
                {
                    sb.AppendLine($"**{t.Weaknesses}:** {answer.Weaknesses}");
                    sb.AppendLine();
                }

                if (!string.IsNullOrWhiteSpace(answer.ImprovementSuggestions))
                {
                    sb.AppendLine($"**{t.ImprovementSuggestions}:** {answer.ImprovementSuggestions}");
                    sb.AppendLine();
                }

                sb.AppendLine("---");
                sb.AppendLine();
            }
        }

        sb.AppendLine($"## {t.OverallAssessment}");
        sb.AppendLine();

        if (session.Answers.Count > 0)
        {
            var avgScore = session.FinalScore?.ToString("F2") ?? "N/A";
            var scoredAnswers = session.Answers.Where(a => a.Score.HasValue).ToList();
            var maxScore = scoredAnswers.Count > 0 ? scoredAnswers.Max(a => a.Score!.Value) : (decimal?)null;
            var minScore = scoredAnswers.Count > 0 ? scoredAnswers.Min(a => a.Score!.Value) : (decimal?)null;

            sb.AppendLine($"- **{t.AverageScore}:** {avgScore} / 10");
            sb.AppendLine($"- **{t.HighestScore}:** {(maxScore.HasValue ? $"{maxScore.Value:F1}" : "N/A")} / 10");
            sb.AppendLine($"- **{t.LowestScore}:** {(minScore.HasValue ? $"{minScore.Value:F1}" : "N/A")} / 10");
            sb.AppendLine($"- **{t.QuestionsAnswered}:** {session.Answers.Count}");
        }
        else
        {
            sb.AppendLine(t.NoAnswers);
        }

        return sb.ToString();
    }

    private static string FormatClassification(string classification, string? language)
    {
        return (classification, language) switch
        {
            ("Approved", "Español") => "Aprobado",
            ("ApprovedWithReservations", "Español") => "Aprobado con Reservas",
            ("Failed", "Español") => "Reprobado",
            ("Approved", "Português") => "Aprovado",
            ("ApprovedWithReservations", "Português") => "Aprovado com Ressalvas",
            ("Failed", "Português") => "Reprovado",
            ("Approved", _) => "Approved",
            ("ApprovedWithReservations", _) => "Approved with Reservations",
            ("Failed", _) => "Failed",
            _ => classification
        };
    }

    private static ReportTranslations GetTranslations(string? language)
    {
        return language switch
        {
            "Español" or "Spanish" => new ReportTranslations
            {
                InterviewReport = "Informe de Entrevista",
                Date = "Fecha",
                Duration = "Duración",
                FinalScore = "Puntuación Final",
                Classification = "Clasificación",
                Summary = "Resumen",
                SummaryText = "El candidato completó **{0}** pregunta(s) con una puntuación final de **{1}** de 10.",
                QuestionDetails = "Detalle de Preguntas",
                Question = "Pregunta",
                IdealAnswer = "Respuesta Ideal",
                CandidateAnswer = "Respuesta del Candidato",
                Score = "Puntuación",
                Feedback = "Retroalimentación",
                Strengths = "Fortalezas",
                Weaknesses = "Debilidades",
                ImprovementSuggestions = "Sugerencias de Mejora",
                OverallAssessment = "Evaluación General",
                AverageScore = "Puntuación Promedio",
                HighestScore = "Puntuación Más Alta",
                LowestScore = "Puntuación Más Baja",
                QuestionsAnswered = "Preguntas Respondidas",
                NoAnswers = "No se enviaron respuestas para esta sesión."
            },
            "Português" or "Portuguese" => new ReportTranslations
            {
                InterviewReport = "Relatório da Entrevista",
                Date = "Data",
                Duration = "Duração",
                FinalScore = "Pontuação Final",
                Classification = "Classificação",
                Summary = "Resumo",
                SummaryText = "O candidato completou **{0}** pergunta(s) com uma pontuação final de **{1}** de 10.",
                QuestionDetails = "Detalhes das Perguntas",
                Question = "Pergunta",
                IdealAnswer = "Resposta Ideal",
                CandidateAnswer = "Resposta do Candidato",
                Score = "Pontuação",
                Feedback = "Feedback",
                Strengths = "Pontos Fortes",
                Weaknesses = "Pontos Fracos",
                ImprovementSuggestions = "Sugestões de Melhoria",
                OverallAssessment = "Avaliação Geral",
                AverageScore = "Pontuação Média",
                HighestScore = "Maior Pontuação",
                LowestScore = "Menor Pontuação",
                QuestionsAnswered = "Perguntas Respondidas",
                NoAnswers = "Nenhuma resposta foi enviada para esta sessão."
            },
            _ => new ReportTranslations()
        };
    }

    private class ReportTranslations
    {
        public string InterviewReport { get; set; } = "Interview Report";
        public string Date { get; set; } = "Date";
        public string Duration { get; set; } = "Duration";
        public string FinalScore { get; set; } = "Final Score";
        public string Classification { get; set; } = "Classification";
        public string Summary { get; set; } = "Summary";
        public string SummaryText { get; set; } = "The candidate completed **{0}** question(s) with a final score of **{1}** out of 10.";
        public string QuestionDetails { get; set; } = "Question Details";
        public string Question { get; set; } = "Question";
        public string IdealAnswer { get; set; } = "Ideal Answer";
        public string CandidateAnswer { get; set; } = "Candidate's Answer";
        public string Score { get; set; } = "Score";
        public string Feedback { get; set; } = "Feedback";
        public string Strengths { get; set; } = "Strengths";
        public string Weaknesses { get; set; } = "Weaknesses";
        public string ImprovementSuggestions { get; set; } = "Improvement Suggestions";
        public string OverallAssessment { get; set; } = "Overall Assessment";
        public string AverageScore { get; set; } = "Average Score";
        public string HighestScore { get; set; } = "Highest Score";
        public string LowestScore { get; set; } = "Lowest Score";
        public string QuestionsAnswered { get; set; } = "Questions Answered";
        public string NoAnswers { get; set; } = "No answers were submitted for this session.";
    }
}
