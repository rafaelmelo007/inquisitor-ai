using Bogus;
using FluentAssertions;
using InquisitorAI.Features.Auth.Domain;
using InquisitorAI.Features.Questionnaires.Commands;
using InquisitorAI.Features.Questionnaires.Domain;
using InquisitorAI.Features.Shared;
using InquisitorAI.Features.Users.Domain;
using Microsoft.EntityFrameworkCore;

namespace InquisitorAI.Tests.Questionnaires.Commands;

public class DeleteQuestionnaireHandlerTests
{
    private readonly Faker _faker = new();
    private readonly DateTimeOffset _fixedNow = new(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task HandleAsync_OwnerCanDelete_RemovesFromDb()
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
            CreatedByUserId = 0,
            User = user,
            IsPublic = true,
            CreatedAt = _fixedNow.AddDays(-5),
            UpdatedAt = _fixedNow.AddDays(-5)
        };
        context.Questionnaires.Add(questionnaire);
        await context.SaveChangesAsync();

        var handler = new DeleteQuestionnaireHandler(context, notifications);
        var command = new DeleteQuestionnaireCommand(questionnaire.Id, user.Id);

        // Act
        await handler.HandleAsync(command);

        // Assert
        notifications.HasErrors.Should().BeFalse();
        var deleted = await context.Questionnaires.FirstOrDefaultAsync(q => q.Id == questionnaire.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task HandleAsync_NonOwner_AddsError()
    {
        // Arrange
        await using var context = CreateContext();
        var notifications = new NotificationHandler();

        var owner = new User
        {
            Provider = OAuthProvider.Google,
            ExternalId = _faker.Random.AlphaNumeric(32),
            Email = _faker.Internet.Email(),
            DisplayName = _faker.Person.FullName,
            CreatedAt = _fixedNow.AddDays(-10),
            UpdatedAt = _fixedNow.AddDays(-10)
        };
        context.Users.Add(owner);

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

        var questionnaire = new Questionnaire
        {
            Name = _faker.Lorem.Sentence(),
            User = owner,
            IsPublic = true,
            CreatedAt = _fixedNow.AddDays(-5),
            UpdatedAt = _fixedNow.AddDays(-5)
        };
        context.Questionnaires.Add(questionnaire);
        await context.SaveChangesAsync();

        var handler = new DeleteQuestionnaireHandler(context, notifications);
        var command = new DeleteQuestionnaireCommand(questionnaire.Id, otherUser.Id);

        // Act
        await handler.HandleAsync(command);

        // Assert
        notifications.HasErrors.Should().BeTrue();
        notifications.Errors.Should().ContainSingle().Which.Should().Contain("permission");

        var stillExists = await context.Questionnaires.FirstOrDefaultAsync(q => q.Id == questionnaire.Id);
        stillExists.Should().NotBeNull();
    }
}
