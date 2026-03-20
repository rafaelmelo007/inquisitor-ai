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

public class RefreshAccessTokenHandlerTests
{
    private readonly Faker _faker = new();
    private readonly Mock<IDateTimeService> _mockClock = new();
    private readonly Mock<IJwtService> _mockJwtService = new();
    private readonly DateTimeOffset _fixedNow = new(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public RefreshAccessTokenHandlerTests()
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

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }

    private User SeedUser(AppDbContext context)
    {
        var user = new User
        {
            Provider = OAuthProvider.Google,
            ExternalId = _faker.Random.AlphaNumeric(32),
            Email = _faker.Internet.Email(),
            DisplayName = _faker.Person.FullName,
            AvatarUrl = _faker.Internet.Avatar(),
            CreatedAt = _fixedNow.AddDays(-30),
            UpdatedAt = _fixedNow.AddDays(-30)
        };
        context.Users.Add(user);
        return user;
    }

    [Fact]
    public async Task HandleAsync_ValidToken_ReturnsNewTokenPair()
    {
        // Arrange
        await using var context = CreateContext();
        var notifications = new NotificationHandler();
        var user = SeedUser(context);

        var rawToken = _faker.Random.AlphaNumeric(64);
        var tokenHash = HashToken(rawToken);

        var refreshToken = new RefreshToken
        {
            User = user,
            Token = tokenHash,
            ExpiresAt = _fixedNow.AddDays(30),
            CreatedAt = _fixedNow.AddDays(-1),
            UpdatedAt = _fixedNow.AddDays(-1)
        };
        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync();

        var newAccessToken = _faker.Random.AlphaNumeric(64);
        var newRawRefreshToken = _faker.Random.AlphaNumeric(64);

        _mockJwtService.Setup(j => j.GenerateAccessToken(It.IsAny<User>())).Returns(newAccessToken);
        _mockJwtService.Setup(j => j.GenerateRefreshToken()).Returns(newRawRefreshToken);

        var handler = new RefreshAccessTokenHandler(context, notifications, _mockClock.Object, _mockJwtService.Object);
        var command = new RefreshAccessTokenCommand(rawToken);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        notifications.HasErrors.Should().BeFalse();
        result.Should().NotBeNull();
        result!.AccessToken.Should().Be(newAccessToken);
        result.RefreshToken.Should().Be(newRawRefreshToken);
    }

    [Fact]
    public async Task HandleAsync_ExpiredToken_AddsError()
    {
        // Arrange
        await using var context = CreateContext();
        var notifications = new NotificationHandler();
        var user = SeedUser(context);

        var rawToken = _faker.Random.AlphaNumeric(64);
        var tokenHash = HashToken(rawToken);

        var refreshToken = new RefreshToken
        {
            User = user,
            Token = tokenHash,
            ExpiresAt = _fixedNow.AddDays(-1),
            CreatedAt = _fixedNow.AddDays(-31),
            UpdatedAt = _fixedNow.AddDays(-31)
        };
        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync();

        var handler = new RefreshAccessTokenHandler(context, notifications, _mockClock.Object, _mockJwtService.Object);
        var command = new RefreshAccessTokenCommand(rawToken);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        notifications.HasErrors.Should().BeTrue();
        notifications.Errors.Should().ContainSingle().Which.Should().Contain("expired");
        result.Should().BeNull();
    }

    [Fact]
    public async Task HandleAsync_RevokedToken_AddsError()
    {
        // Arrange
        await using var context = CreateContext();
        var notifications = new NotificationHandler();
        var user = SeedUser(context);

        var rawToken = _faker.Random.AlphaNumeric(64);
        var tokenHash = HashToken(rawToken);

        var refreshToken = new RefreshToken
        {
            User = user,
            Token = tokenHash,
            ExpiresAt = _fixedNow.AddDays(30),
            RevokedAt = _fixedNow.AddDays(-1),
            CreatedAt = _fixedNow.AddDays(-5),
            UpdatedAt = _fixedNow.AddDays(-1)
        };
        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync();

        var handler = new RefreshAccessTokenHandler(context, notifications, _mockClock.Object, _mockJwtService.Object);
        var command = new RefreshAccessTokenCommand(rawToken);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        notifications.HasErrors.Should().BeTrue();
        notifications.Errors.Should().ContainSingle().Which.Should().Contain("revoked");
        result.Should().BeNull();
    }
}
