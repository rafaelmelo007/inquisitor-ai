using System.Security.Cryptography;
using System.Text;
using Bogus;
using FluentAssertions;
using InquisitorAI.Features.Auth;
using InquisitorAI.Features.Auth.Commands;
using InquisitorAI.Features.Auth.Domain;
using InquisitorAI.Features.Shared;
using InquisitorAI.Features.Users.Domain;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace InquisitorAI.Tests.Auth.Commands;

public class IssueTokensHandlerTests
{
    private readonly Faker _faker = new();
    private readonly Mock<IDateTimeService> _mockClock = new();
    private readonly Mock<IJwtService> _mockJwtService = new();
    private readonly DateTimeOffset _fixedNow = new(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public IssueTokensHandlerTests()
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
    public async Task HandleAsync_NewUser_CreatesUserWithCorrectFields()
    {
        // Arrange
        await using var context = CreateContext();
        var notifications = new NotificationHandler();

        var accessToken = _faker.Random.AlphaNumeric(64);
        var rawRefreshToken = _faker.Random.AlphaNumeric(64);

        _mockJwtService.Setup(j => j.GenerateAccessToken(It.IsAny<User>())).Returns(accessToken);
        _mockJwtService.Setup(j => j.GenerateRefreshToken()).Returns(rawRefreshToken);

        var handler = new IssueTokensHandler(context, notifications, _mockClock.Object, _mockJwtService.Object);

        var command = new IssueTokensCommand(
            Provider: "Google",
            ExternalId: _faker.Random.AlphaNumeric(32),
            Email: _faker.Internet.Email(),
            DisplayName: _faker.Person.FullName,
            AvatarUrl: _faker.Internet.Avatar());

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        notifications.HasErrors.Should().BeFalse();
        result.Should().NotBeNull();
        result!.AccessToken.Should().Be(accessToken);
        result.RefreshToken.Should().Be(rawRefreshToken);

        var user = await context.Users.FirstOrDefaultAsync();
        user.Should().NotBeNull();
        user!.Provider.Should().Be(OAuthProvider.Google);
        user.ExternalId.Should().Be(command.ExternalId);
        user.Email.Should().Be(command.Email);
        user.DisplayName.Should().Be(command.DisplayName);
        user.AvatarUrl.Should().Be(command.AvatarUrl);
        user.CreatedAt.Should().Be(_fixedNow);
        user.UpdatedAt.Should().Be(_fixedNow);
    }

    [Fact]
    public async Task HandleAsync_ExistingUser_UpdatesUserOnSubsequentLogin()
    {
        // Arrange
        await using var context = CreateContext();
        var notifications = new NotificationHandler();

        var externalId = _faker.Random.AlphaNumeric(32);
        var existingUser = new User
        {
            Provider = OAuthProvider.GitHub,
            ExternalId = externalId,
            Email = _faker.Internet.Email(),
            DisplayName = _faker.Person.FullName,
            AvatarUrl = _faker.Internet.Avatar(),
            CreatedAt = _fixedNow.AddDays(-10),
            UpdatedAt = _fixedNow.AddDays(-10)
        };
        context.Users.Add(existingUser);
        await context.SaveChangesAsync();

        var newEmail = _faker.Internet.Email();
        var newDisplayName = _faker.Person.FullName;
        var newAvatarUrl = _faker.Internet.Avatar();

        _mockJwtService.Setup(j => j.GenerateAccessToken(It.IsAny<User>())).Returns("access-token");
        _mockJwtService.Setup(j => j.GenerateRefreshToken()).Returns("refresh-token");

        var handler = new IssueTokensHandler(context, notifications, _mockClock.Object, _mockJwtService.Object);

        var command = new IssueTokensCommand(
            Provider: "GitHub",
            ExternalId: externalId,
            Email: newEmail,
            DisplayName: newDisplayName,
            AvatarUrl: newAvatarUrl);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        notifications.HasErrors.Should().BeFalse();
        result.Should().NotBeNull();

        var user = await context.Users.FirstOrDefaultAsync(u => u.ExternalId == externalId);
        user.Should().NotBeNull();
        user!.Email.Should().Be(newEmail);
        user.DisplayName.Should().Be(newDisplayName);
        user.AvatarUrl.Should().Be(newAvatarUrl);
        user.UpdatedAt.Should().Be(_fixedNow);
        user.CreatedAt.Should().Be(_fixedNow.AddDays(-10));
    }

    [Fact]
    public async Task HandleAsync_RefreshTokenStoredHashed_NotRawToken()
    {
        // Arrange
        await using var context = CreateContext();
        var notifications = new NotificationHandler();

        var rawRefreshToken = _faker.Random.AlphaNumeric(64);

        _mockJwtService.Setup(j => j.GenerateAccessToken(It.IsAny<User>())).Returns("access-token");
        _mockJwtService.Setup(j => j.GenerateRefreshToken()).Returns(rawRefreshToken);

        var handler = new IssueTokensHandler(context, notifications, _mockClock.Object, _mockJwtService.Object);

        var command = new IssueTokensCommand(
            Provider: "Google",
            ExternalId: _faker.Random.AlphaNumeric(32),
            Email: _faker.Internet.Email(),
            DisplayName: _faker.Person.FullName,
            AvatarUrl: null);

        // Act
        await handler.HandleAsync(command);

        // Assert
        var storedToken = await context.RefreshTokens.FirstOrDefaultAsync();
        storedToken.Should().NotBeNull();
        storedToken!.Token.Should().NotBe(rawRefreshToken);

        var expectedHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(rawRefreshToken)));
        storedToken.Token.Should().Be(expectedHash);
    }
}
