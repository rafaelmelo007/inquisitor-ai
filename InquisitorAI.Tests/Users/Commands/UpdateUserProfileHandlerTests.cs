using Bogus;
using FluentAssertions;
using InquisitorAI.Features.Auth.Domain;
using InquisitorAI.Features.Shared;
using InquisitorAI.Features.Users.Commands;
using InquisitorAI.Features.Users.Domain;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace InquisitorAI.Tests.Users.Commands;

public class UpdateUserProfileHandlerTests
{
    private readonly Faker _faker = new();
    private readonly Mock<IDateTimeService> _mockClock = new();
    private readonly DateTimeOffset _fixedNow = new(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public UpdateUserProfileHandlerTests()
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
    public async Task HandleAsync_ValidUpdate_ChangesDisplayNameAndAvatarUrl()
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
            AvatarUrl = _faker.Internet.Avatar(),
            CreatedAt = _fixedNow.AddDays(-10),
            UpdatedAt = _fixedNow.AddDays(-10)
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var newDisplayName = _faker.Person.FullName;
        var newAvatarUrl = _faker.Internet.Avatar();

        var handler = new UpdateUserProfileHandler(context, notifications, _mockClock.Object);
        var command = new UpdateUserProfileCommand(user.Id, newDisplayName, newAvatarUrl);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        notifications.HasErrors.Should().BeFalse();
        result.Should().NotBeNull();
        result!.DisplayName.Should().Be(newDisplayName);
        result.AvatarUrl.Should().Be(newAvatarUrl);

        var updatedUser = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        updatedUser!.DisplayName.Should().Be(newDisplayName);
        updatedUser.AvatarUrl.Should().Be(newAvatarUrl);
        updatedUser.UpdatedAt.Should().Be(_fixedNow);
    }

    [Fact]
    public async Task HandleAsync_NonExistentUser_AddsError()
    {
        // Arrange
        await using var context = CreateContext();
        var notifications = new NotificationHandler();

        var handler = new UpdateUserProfileHandler(context, notifications, _mockClock.Object);
        var command = new UpdateUserProfileCommand(
            UserId: _faker.Random.Long(1, 10000),
            DisplayName: _faker.Person.FullName,
            AvatarUrl: _faker.Internet.Avatar());

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        notifications.HasErrors.Should().BeTrue();
        notifications.Errors.Should().ContainSingle().Which.Should().Contain("not found");
        result.Should().BeNull();
    }
}
