using InquisitorAI.Features.Questionnaires;
using InquisitorAI.Features.Questionnaires.Dtos;

namespace InquisitorAI.Infrastructure.Services;

public class MarkdownParserService : IMarkdownParserService
{
    public ParsedQuestionnaireDto Parse(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new FormatException("Markdown content is empty.");

        var lines = content.Split('\n');
        string? questionnaireName = null;
        var questions = new List<ParsedQuestionDto>();

        var currentCategory = (string?)null;
        var currentDifficulty = (string?)null;
        var currentQuestion = (string?)null;
        var currentIdealAnswer = (string?)null;
        var activeField = (string?)null;
        var inQuestionBlock = false;
        var orderIndex = 0;

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].TrimEnd('\r');

            // H1 — questionnaire name
            if (line.StartsWith("# ") && !line.StartsWith("## "))
            {
                questionnaireName = line[2..].Trim();
                continue;
            }

            // H2 — new question block
            if (line.StartsWith("## "))
            {
                // Save previous question block if any
                if (inQuestionBlock)
                {
                    AppendMultiLineField(ref currentQuestion, ref currentIdealAnswer, activeField, null);
                    questions.Add(BuildQuestion(orderIndex++, currentCategory, currentDifficulty, currentQuestion, currentIdealAnswer));
                }

                inQuestionBlock = true;
                currentCategory = null;
                currentDifficulty = null;
                currentQuestion = null;
                currentIdealAnswer = null;
                activeField = null;
                continue;
            }

            if (!inQuestionBlock)
                continue;

            // Bold-key fields
            if (TryExtractBoldKey(line, "**Category:**", out var categoryValue))
            {
                AppendMultiLineField(ref currentQuestion, ref currentIdealAnswer, activeField, null);
                currentCategory = categoryValue;
                activeField = "Category";
                continue;
            }

            if (TryExtractBoldKey(line, "**Difficulty:**", out var difficultyValue))
            {
                AppendMultiLineField(ref currentQuestion, ref currentIdealAnswer, activeField, null);
                currentDifficulty = difficultyValue;
                activeField = "Difficulty";
                continue;
            }

            if (TryExtractBoldKey(line, "**Question:**", out var questionValue))
            {
                AppendMultiLineField(ref currentQuestion, ref currentIdealAnswer, activeField, null);
                currentQuestion = questionValue;
                activeField = "Question";
                continue;
            }

            if (TryExtractBoldKey(line, "**Ideal Answer:**", out var idealAnswerValue))
            {
                AppendMultiLineField(ref currentQuestion, ref currentIdealAnswer, activeField, null);
                currentIdealAnswer = idealAnswerValue;
                activeField = "IdealAnswer";
                continue;
            }

            // Continuation line for multi-line Question or Ideal Answer
            AppendMultiLineField(ref currentQuestion, ref currentIdealAnswer, activeField, line);
        }

        // Save last question block
        if (inQuestionBlock)
        {
            AppendMultiLineField(ref currentQuestion, ref currentIdealAnswer, activeField, null);
            questions.Add(BuildQuestion(orderIndex++, currentCategory, currentDifficulty, currentQuestion, currentIdealAnswer));
        }

        if (questionnaireName is null)
            throw new FormatException("No H1 heading found. The questionnaire name is required.");

        return new ParsedQuestionnaireDto(questionnaireName, questions.AsReadOnly());
    }

    private static bool TryExtractBoldKey(string line, string key, out string value)
    {
        var trimmed = line.TrimStart();
        if (trimmed.StartsWith(key, StringComparison.Ordinal))
        {
            value = trimmed[key.Length..].Trim();
            return true;
        }

        value = string.Empty;
        return false;
    }

    private static void AppendMultiLineField(
        ref string? currentQuestion,
        ref string? currentIdealAnswer,
        string? activeField,
        string? line)
    {
        if (line is null || activeField is null)
            return;

        switch (activeField)
        {
            case "Question":
                currentQuestion = string.IsNullOrEmpty(currentQuestion)
                    ? line
                    : $"{currentQuestion}\n{line}";
                break;
            case "IdealAnswer":
                currentIdealAnswer = string.IsNullOrEmpty(currentIdealAnswer)
                    ? line
                    : $"{currentIdealAnswer}\n{line}";
                break;
        }
    }

    private static ParsedQuestionDto BuildQuestion(
        int orderIndex,
        string? category,
        string? difficulty,
        string? questionText,
        string? idealAnswer)
    {
        if (string.IsNullOrWhiteSpace(questionText))
            throw new FormatException($"Question at index {orderIndex} is missing the required 'Question' field.");

        if (string.IsNullOrWhiteSpace(idealAnswer))
            throw new FormatException($"Question at index {orderIndex} is missing the required 'Ideal Answer' field.");

        return new ParsedQuestionDto(
            OrderIndex: orderIndex,
            Category: string.IsNullOrWhiteSpace(category) ? null : category,
            Difficulty: string.IsNullOrWhiteSpace(difficulty) ? null : difficulty,
            QuestionText: questionText.Trim(),
            IdealAnswer: idealAnswer.Trim());
    }
}
