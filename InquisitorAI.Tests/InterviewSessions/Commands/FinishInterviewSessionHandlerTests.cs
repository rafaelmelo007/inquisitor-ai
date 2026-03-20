using Bogus;
using FluentAssertions;
using InquisitorAI.Features.Auth.Domain;
using InquisitorAI.Features.InterviewSessions;
using InquisitorAI.Features.InterviewSessions.Commands;
using InquisitorAI.Features.InterviewSessions.Domain;
using InquisitorAI.Features.InterviewSessions.Dtos;
using InquisitorAI.Features.Questionnaires.Domain;
using InquisitorAI.Features.Shared;
using InquisitorAI.Features.Users.Domain;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace InquisitorAI.Tests.InterviewSessions.Commands;

public class FinishInterviewSessionHandlerTests
{
    private readonly Faker _faker = new();
    private readonly Mock<IDateTimeService> _mockClock = new();
    private readonly Mock<IReportGeneratorService> _mockReportGenerator = new();
    private readonly DateTimeOffset _fixedNow = new(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public FinishInterviewSessionHandlerTests()
    {
        _mockClock.Setup(c => c.UtcNow).Returns(_fixedNow);
        _mockReportGenerator
            .Setup(r => r.Generate(It.IsAny<InterviewSessionDto>()))
            .Returns("# Report Content");
    }

    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private async Task<(User user, InterviewSession session)> SeedSessionWithAnswers(
        AppDbContext context, params decimal[] scores)
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

        var questionnaire = new Questionnaire
        {
            Name = _faker.Lorem.Sentence(),
            User = user,
            IsPublic = true,
            CreatedAt = _fixedNow.AddDays(-5),
            UpdatedAt = _fixedNow.AddDays(-5)
        };
        context.Questionnaires.Add(questionnaire);

        var session = new InterviewSession
        {
            User = user,
            Questionnaire = questionnaire,
            StartedAt = _fixedNow.AddMinutes(-60),
            CreatedAt = _fixedNow.AddMinutes(-60),
            UpdatedAt = _fixedNow.AddMinutes(-60)
        };
        context.InterviewSessions.Add(session);

        foreach (var score in scores)
        {
            var question = new Question
            {
                Questionnaire = questionnaire,
                OrderIndex = questionnaire.Questions.Count,
                Category = _faker.Lorem.Word(),
                Difficulty = Difficulty.Medium,
                QuestionText = _faker.Lorem.Sentence(),
                IdealAnswer = _faker.Lorem.Paragraph(),
                CreatedAt = _fixedNow.AddDays(-5),
                UpdatedAt = _fixedNow.AddDays(-5)
            };
            context.Questions.Add(question);

            var answer = new SessionAnswer
            {
                Session = session,
                Question = question,
                Transcript = _faker.Lorem.Paragraph(),
                Score = score,
                AiFeedback = _faker.Lorem.Sentence(),
                Strengths = _faker.Lorem.Sentence(),
                Weaknesses = _faker.Lorem.Sentence(),
                ImprovementSuggestions = _faker.Lorem.Sentence(),
                CreatedAt = _fixedNow.AddMinutes(-30),
                UpdatedAt = _fixedNow.AddMinutes(-30)
            };
            context.SessionAnswers.Add(answer);
        }

        await context.SaveChangesAsync();
        return (user, session);
    }

    [Fact]
    public async Task HandleAsync_AveragesScoresCorrectly_TwoDecimalPlaces()
    {
        // Arrange
        await using var context = CreateContext();
        var notifications = new NotificationHandler();
        var (user, session) = await SeedSessionWithAnswers(context, 7.5m, 8.3m, 9.1m);

        var handler = new FinishInterviewSessionHandler(context, notifications, _mockClock.Object, _mockReportGenerator.Object);
        var command = new FinishInterviewSessionCommand(session.Id, user.Id);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        notifications.HasErrors.Should().BeFalse();
        result.Should().NotBeNull();

        var expectedAverage = Math.Round((7.5m + 8.3m + 9.1m) / 3, 2);
        result!.FinalScore.Should().Be(expectedAverage);
    }

    [Fact]
    public async Task HandleAsync_ScoreGte8_ClassificationApproved()
    {
        // Arrange
        await using var context = CreateContext();
        var notifications = new NotificationHandler();
        var (user, session) = await SeedSessionWithAnswers(context, 8.0m, 9.0m, 8.5m);

        var handler = new FinishInterviewSessionHandler(context, notifications, _mockClock.Object, _mockReportGenerator.Object);
        var command = new FinishInterviewSessionCommand(session.Id, user.Id);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result!.FinalScore.Should().BeGreaterThanOrEqualTo(8.0m);
        result.Classification.Should().Be(Classification.Approved.ToString());
    }

    [Fact]
    public async Task HandleAsync_ScoreGte6_5AndLt8_ClassificationApprovedWithReservations()
    {
        // Arrange
        await using var context = CreateContext();
        var notifications = new NotificationHandler();
        var (user, session) = await SeedSessionWithAnswers(context, 6.5m, 7.0m, 7.5m);

        var handler = new FinishInterviewSessionHandler(context, notifications, _mockClock.Object, _mockReportGenerator.Object);
        var command = new FinishInterviewSessionCommand(session.Id, user.Id);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result!.FinalScore.Should().BeGreaterThanOrEqualTo(6.5m);
        result.FinalScore.Should().BeLessThan(8.0m);
        result.Classification.Should().Be(Classification.ApprovedWithReservations.ToString());
    }

    [Fact]
    public async Task HandleAsync_ScoreLt6_5_ClassificationFailed()
    {
        // Arrange
        await using var context = CreateContext();
        var notifications = new NotificationHandler();
        var (user, session) = await SeedSessionWithAnswers(context, 4.0m, 5.5m, 6.0m);

        var handler = new FinishInterviewSessionHandler(context, notifications, _mockClock.Object, _mockReportGenerator.Object);
        var command = new FinishInterviewSessionCommand(session.Id, user.Id);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result!.FinalScore.Should().BeLessThan(6.5m);
        result.Classification.Should().Be(Classification.Failed.ToString());
    }

    [Fact]
    public async Task HandleAsync_CallsReportGeneratorService()
    {
        // Arrange
        await using var context = CreateContext();
        var notifications = new NotificationHandler();
        var (user, session) = await SeedSessionWithAnswers(context, 8.0m, 9.0m);

        var handler = new FinishInterviewSessionHandler(context, notifications, _mockClock.Object, _mockReportGenerator.Object);
        var command = new FinishInterviewSessionCommand(session.Id, user.Id);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        _mockReportGenerator.Verify(r => r.Generate(It.IsAny<InterviewSessionDto>()), Times.Once);
        result.Should().NotBeNull();
        result!.ReportContent.Should().Be("# Report Content");
    }
}
