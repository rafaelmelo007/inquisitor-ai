using Bogus;
using FluentAssertions;
using InquisitorAI.Features.Auth.Domain;
using InquisitorAI.Features.InterviewSessions.Commands;
using InquisitorAI.Features.Questionnaires.Domain;
using InquisitorAI.Features.Shared;
using InquisitorAI.Features.Users.Domain;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace InquisitorAI.Tests.InterviewSessions.Commands;

public class StartInterviewSessionHandlerTests
{
    private readonly Faker _faker = new();
    private readonly Mock<IDateTimeService> _mockClock = new();
    private readonly DateTimeOffset _fixedNow = new(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public StartInterviewSessionHandlerTests()
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

    [Fact]
    public async Task HandleAsync_ValidStart_CreatesSessionWithStartedAt()
    {
        // Arrange
        await using var context = CreateContext();
        var notifications = new NotificationHandler();

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
        await context.SaveChangesAsync();

        var handler = new StartInterviewSessionHandler(context, notifications, _mockClock.Object);
        var command = new StartInterviewSessionCommand(user.Id, questionnaire.Id);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        notifications.HasErrors.Should().BeFalse();
        result.Should().NotBeNull();
        result!.StartedAt.Should().Be(_fixedNow);
        result.QuestionnaireId.Should().Be(questionnaire.Id);
        result.UserId.Should().Be(user.Id);

        var session = await context.InterviewSessions.FirstOrDefaultAsync();
        session.Should().NotBeNull();
        session!.StartedAt.Should().Be(_fixedNow);
        session.CreatedAt.Should().Be(_fixedNow);
    }

    [Fact]
    public async Task HandleAsync_NonExistentQuestionnaire_AddsError()
    {
        // Arrange
        await using var context = CreateContext();
        var notifications = new NotificationHandler();

        var handler = new StartInterviewSessionHandler(context, notifications, _mockClock.Object);
        var command = new StartInterviewSessionCommand(
            UserId: _faker.Random.Long(1, 10000),
            QuestionnaireId: _faker.Random.Long(1, 10000));

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        notifications.HasErrors.Should().BeTrue();
        notifications.Errors.Should().ContainSingle().Which.Should().Contain("not found");
        result.Should().BeNull();
    }
}
