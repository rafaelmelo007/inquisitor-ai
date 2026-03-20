using FluentAssertions;
using InquisitorAI.Infrastructure.Services;

namespace InquisitorAI.Tests.Questionnaires.Services;

public class MarkdownParserServiceTests
{
    private readonly MarkdownParserService _service = new();

    [Fact]
    public void Parse_ValidMarkdown_ReturnsCorrectQuestionnaireNameAndQuestions()
    {
        // Arrange
        var markdown = """
            # .NET Technical Interview

            ## Question 1

            **Category:** C# Fundamentals
            **Difficulty:** Easy
            **Question:** What is the difference between a class and a struct in C#?
            **Ideal Answer:** A class is a reference type allocated on the heap, while a struct is a value type allocated on the stack.

            ## Question 2

            **Category:** ASP.NET Core
            **Difficulty:** Medium
            **Question:** Explain middleware in ASP.NET Core.
            **Ideal Answer:** Middleware is software assembled into an application pipeline to handle requests and responses.
            """;

        // Act
        var result = _service.Parse(markdown);

        // Assert
        result.Name.Should().Be(".NET Technical Interview");
        result.Questions.Should().HaveCount(2);

        result.Questions[0].OrderIndex.Should().Be(0);
        result.Questions[0].Category.Should().Be("C# Fundamentals");
        result.Questions[0].Difficulty.Should().Be("Easy");
        result.Questions[0].QuestionText.Should().Be("What is the difference between a class and a struct in C#?");
        result.Questions[0].IdealAnswer.Should().Be("A class is a reference type allocated on the heap, while a struct is a value type allocated on the stack.");

        result.Questions[1].OrderIndex.Should().Be(1);
        result.Questions[1].Category.Should().Be("ASP.NET Core");
        result.Questions[1].Difficulty.Should().Be("Medium");
    }

    [Fact]
    public void Parse_AllFieldTypesParsedCorrectly()
    {
        // Arrange
        var markdown = """
            # Full Field Test

            ## Section A

            **Category:** Design Patterns
            **Difficulty:** Hard
            **Question:** What is the SOLID principle?
            **Ideal Answer:** SOLID is an acronym for five design principles.
            """;

        // Act
        var result = _service.Parse(markdown);

        // Assert
        var question = result.Questions[0];
        question.Category.Should().Be("Design Patterns");
        question.Difficulty.Should().Be("Hard");
        question.QuestionText.Should().Be("What is the SOLID principle?");
        question.IdealAnswer.Should().Be("SOLID is an acronym for five design principles.");
    }

    [Fact]
    public void Parse_MultiLineIdealAnswer_CapturedCorrectly()
    {
        // Arrange
        var markdown = """
            # Multi-line Test

            ## Question 1

            **Category:** Architecture
            **Difficulty:** Medium
            **Question:** Describe clean architecture.
            **Ideal Answer:** Clean architecture separates concerns into layers.
            The inner layers contain business logic.
            The outer layers contain infrastructure and UI concerns.
            """;

        // Act
        var result = _service.Parse(markdown);

        // Assert
        var idealAnswer = result.Questions[0].IdealAnswer;
        idealAnswer.Should().Contain("Clean architecture separates concerns into layers.");
        idealAnswer.Should().Contain("The inner layers contain business logic.");
        idealAnswer.Should().Contain("The outer layers contain infrastructure and UI concerns.");
    }

    [Fact]
    public void Parse_MissingH1_ThrowsFormatException()
    {
        // Arrange
        var markdown = """
            ## Question 1

            **Category:** C#
            **Difficulty:** Easy
            **Question:** What is C#?
            **Ideal Answer:** A programming language by Microsoft.
            """;

        // Act
        var act = () => _service.Parse(markdown);

        // Assert
        act.Should().Throw<FormatException>()
            .WithMessage("*H1*");
    }

    [Fact]
    public void Parse_MissingRequiredFields_ThrowsFormatException()
    {
        // Arrange — missing Ideal Answer field
        var markdown = """
            # Incomplete Questionnaire

            ## Question 1

            **Category:** C#
            **Difficulty:** Easy
            **Question:** What is C#?
            """;

        // Act
        var act = () => _service.Parse(markdown);

        // Assert
        act.Should().Throw<FormatException>()
            .WithMessage("*Ideal Answer*");
    }
}
