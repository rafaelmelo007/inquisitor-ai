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

public class SubmitAnswerHandlerTests
{
    private readonly Faker _faker = new();
    private readonly Mock<IDateTimeService> _mockClock = new();
    private readonly Mock<IAiEvaluationService> _mockAiEvaluation = new();
    private readonly DateTimeOffset _fixedNow = new(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public SubmitAnswerHandlerTests()
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

    private async Task<(User user, Questionnaire questionnaire, Question question, InterviewSession session)> SeedSessionData(AppDbContext context)
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

        var question = new Question
        {
            Questionnaire = questionnaire,
            OrderIndex = 0,
            Category = _faker.Lorem.Word(),
            Difficulty = Difficulty.Medium,
            QuestionText = _faker.Lorem.Sentence(),
            IdealAnswer = _faker.Lorem.Paragraph(),
            CreatedAt = _fixedNow.AddDays(-5),
            UpdatedAt = _fixedNow.AddDays(-5)
        };
        context.Questions.Add(question);

        var session = new InterviewSession
        {
            User = user,
            Questionnaire = questionnaire,
            StartedAt = _fixedNow.AddMinutes(-30),
            CreatedAt = _fixedNow.AddMinutes(-30),
            UpdatedAt = _fixedNow.AddMinutes(-30)
        };
        context.InterviewSessions.Add(session);
        await context.SaveChangesAsync();

        return (user, questionnaire, question, session);
    }

    [Fact]
    public async Task HandleAsync_ValidSubmission_PersistsSessionAnswerWithScoreAndFeedback()
    {
        // Arrange
        await using var context = CreateContext();
        var notifications = new NotificationHandler();
        var (user, _, question, session) = await SeedSessionData(context);

        var transcript = _faker.Lorem.Paragraph();
        var evaluationResult = new EvaluationResultDto(
            Score: 8.5m,
            Summary: _faker.Lorem.Sentence(),
            Strengths: _faker.Lorem.Sentence(),
            Weaknesses: _faker.Lorem.Sentence(),
            ImprovementSuggestions: _faker.Lorem.Sentence());

        _mockAiEvaluation
            .Setup(a => a.EvaluateAsync(It.IsAny<EvaluateAnswerRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(evaluationResult);

        var handler = new SubmitAnswerHandler(context, notifications, _mockClock.Object, _mockAiEvaluation.Object);
        var command = new SubmitAnswerCommand(session.Id, user.Id, question.Id, transcript);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        notifications.HasErrors.Should().BeFalse();
        result.Should().NotBeNull();
        result!.Score.Should().Be(8.5m);
        result.AiFeedback.Should().Be(evaluationResult.Summary);
        result.Transcript.Should().Be(transcript);

        var answer = await context.SessionAnswers.FirstOrDefaultAsync();
        answer.Should().NotBeNull();
        answer!.Score.Should().Be(8.5m);
        answer.AiFeedback.Should().Be(evaluationResult.Summary);
        answer.Strengths.Should().Be(evaluationResult.Strengths);
        answer.Weaknesses.Should().Be(evaluationResult.Weaknesses);
        answer.ImprovementSuggestions.Should().Be(evaluationResult.ImprovementSuggestions);
    }

    [Fact]
    public async Task HandleAsync_SessionNotOwnedByUser_AddsError()
    {
        // Arrange
        await using var context = CreateContext();
        var notifications = new NotificationHandler();
        var (_, _, question, session) = await SeedSessionData(context);

        var otherUser = new User
        {
            Provider = OAuthProvider.GitHub,
            ExternalId = _faker.Random.AlphaNumeric(32),
            Email = _faker.Internet.Email(),
            DisplayName = _faker.Person.FullName,
            CreatedAt = _fixedNow.AddDays(-10),
            UpdatedAt = _fixedNow.AddDays(-10)
        };
        context.Users.Add(otherUser);
        await context.SaveChangesAsync();

        var handler = new SubmitAnswerHandler(context, notifications, _mockClock.Object, _mockAiEvaluation.Object);
        var command = new SubmitAnswerCommand(session.Id, otherUser.Id, question.Id, _faker.Lorem.Paragraph());

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        notifications.HasErrors.Should().BeTrue();
        notifications.Errors.Should().ContainSingle().Which.Should().Contain("permission");
        result.Should().BeNull();
    }
}
