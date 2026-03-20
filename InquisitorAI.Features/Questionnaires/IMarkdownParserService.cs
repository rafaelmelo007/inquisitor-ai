using InquisitorAI.Features.Questionnaires.Dtos;

namespace InquisitorAI.Features.Questionnaires;

/// <summary>
/// Parses raw Markdown content into a structured questionnaire.
/// Throws <see cref="FormatException"/> when the Markdown structure is invalid.
/// </summary>
public interface IMarkdownParserService
{
    ParsedQuestionnaireDto Parse(string content);
}
