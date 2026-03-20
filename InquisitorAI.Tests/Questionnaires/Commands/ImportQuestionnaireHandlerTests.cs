using Bogus;
using FluentAssertions;
using InquisitorAI.Features.Auth.Domain;
using InquisitorAI.Features.Questionnaires;
using InquisitorAI.Features.Questionnaires.Commands;
using InquisitorAI.Features.Questionnaires.Dtos;
using InquisitorAI.Features.Shared;
using InquisitorAI.Features.Users.Domain;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace InquisitorAI.Tests.Questionnaires.Commands;

public class ImportQuestionnaireHandlerTests
{
    private readonly Faker _faker = new();
    private readonly Mock<IDateTimeService> _mockClock = new();
    private readonly Mock<IMarkdownParserService> _mockParser = new();
    private readonly DateTimeOffset _fixedNow = new(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public ImportQuestionnaireHandlerTests()
    {
        _mockClock.Setup(c => c.UtcNow).Returns(_fixedNow);
    }

    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private async Task<User> SeedUser(AppDbContext context)
    {
        var user = new User
        {
            Provider = OAuthProvider.Google,
            ExternalId = _faker.Random.AlphaNumeric(32),
            Email = _faker.Internet.Email(),
            DisplayName = _faker.Person.FullName,
            CreatedAt = _fixedNow.AddDays(-10),
            UpdatedAt = _fixedNow.AddDays(-10)
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    [Fact]
    public async Task HandleAsync_ValidMarkdown_PersistsQuestionnaireAndQuestions()
    {
        // Arrange
        await using var context = CreateContext();
        var notifications = new NotificationHandler();
        var user = await SeedUser(context);

        var parsedQuestions = new List<ParsedQuestionDto>
        {
            new(0, "C#", "Easy", "What is a class?", "A blueprint for creating objects."),
            new(1, "C#", "Medium", "Explain polymorphism.", "The ability of objects to take many forms.")
        };

        var parsedResult = new ParsedQuestionnaireDto(".NET Interview", parsedQuestions.AsReadOnly());
        _mockParser.Setup(p => p.Parse(It.IsAny<string>())).Returns(parsedResult);

        var handler = new ImportQuestionnaireHandler(context, notifications, _mockClock.Object, _mockParser.Object);
        var command = new ImportQuestionnaireCommand(user.Id, "# .NET Interview\n## Q1\n...", IsPublic: true);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        notifications.HasErrors.Should().BeFalse();
        result.Should().NotBeNull();
        result!.Name.Should().Be(".NET Interview");
        result.QuestionCount.Should().Be(2);
        result.IsPublic.Should().BeTrue();

        var questionnaire = await context.Questionnaires
            .Include(q => q.Questions)
            .FirstOrDefaultAsync();
        questionnaire.Should().NotBeNull();
        questionnaire!.Name.Should().Be(".NET Interview");
        questionnaire.Questions.Should().HaveCount(2);
        questionnaire.CreatedAt.Should().Be(_fixedNow);
    }

    [Fact]
    public async Task HandleAsync_EmptyQuestionList_AddsError()
    {
        // Arrange
        await using var context = CreateContext();
        var notifications = new NotificationHandler();

        var parsedResult = new ParsedQuestionnaireDto("Empty Questionnaire", new List<ParsedQuestionDto>().AsReadOnly());
        _mockParser.Setup(p => p.Parse(It.IsAny<string>())).Returns(parsedResult);

        var handler = new ImportQuestionnaireHandler(context, notifications, _mockClock.Object, _mockParser.Object);
        var command = new ImportQuestionnaireCommand(_faker.Random.Long(1, 10000), "# Empty\n", IsPublic: false);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        notifications.HasErrors.Should().BeTrue();
        notifications.Errors.Should().ContainSingle().Which.Should().Contain("at least one question");
        result.Should().BeNull();
    }

    [Fact]
    public async Task HandleAsync_ParserThrowsFormatException_AddsError()
    {
        // Arrange
        await using var context = CreateContext();
        var notifications = new NotificationHandler();

        var errorMessage = "No H1 heading found. The questionnaire name is required.";
        _mockParser.Setup(p => p.Parse(It.IsAny<string>())).Throws(new FormatException(errorMessage));

        var handler = new ImportQuestionnaireHandler(context, notifications, _mockClock.Object, _mockParser.Object);
        var command = new ImportQuestionnaireCommand(_faker.Random.Long(1, 10000), "invalid markdown", IsPublic: false);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        notifications.HasErrors.Should().BeTrue();
        notifications.Errors.Should().ContainSingle().Which.Should().Be(errorMessage);
        result.Should().BeNull();
    }
}
