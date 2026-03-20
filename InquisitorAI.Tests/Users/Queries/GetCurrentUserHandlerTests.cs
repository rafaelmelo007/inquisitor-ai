using System.Data;
using Bogus;
using Dapper;
using FluentAssertions;
using InquisitorAI.Features.Users.Dtos;
using InquisitorAI.Features.Users.Queries;
using Moq;
using Moq.Dapper;

namespace InquisitorAI.Tests.Users.Queries;

public class GetCurrentUserHandlerTests
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task HandleAsync_ExistingUser_ReturnsUserDto()
    {
        // Arrange
        var mockDb = new Mock<IDbConnection>();
        var userId = _faker.Random.Long(1, 10000);
        var expectedDto = new UserDto(
            userId,
            _faker.Internet.Email(),
            _faker.Person.FullName,
            _faker.Internet.Avatar(),
            "Google",
            new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));

        mockDb.SetupDapper(c => c.QuerySingleOrDefaultAsync<UserDto>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null,
                null,
                null))
            .ReturnsAsync(expectedDto);

        var handler = new GetCurrentUserHandler(mockDb.Object);
        var query = new GetCurrentUserQuery(userId);

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(userId);
        result.Email.Should().Be(expectedDto.Email);
        result.DisplayName.Should().Be(expectedDto.DisplayName);
        result.AvatarUrl.Should().Be(expectedDto.AvatarUrl);
        result.Provider.Should().Be(expectedDto.Provider);
    }

    [Fact]
    public async Task HandleAsync_NonExistentUser_ReturnsNull()
    {
        // Arrange
        var mockDb = new Mock<IDbConnection>();

        mockDb.SetupDapper(c => c.QuerySingleOrDefaultAsync<UserDto>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null,
                null,
                null))
            .ReturnsAsync((UserDto?)null);

        var handler = new GetCurrentUserHandler(mockDb.Object);
        var query = new GetCurrentUserQuery(_faker.Random.Long(1, 10000));

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        result.Should().BeNull();
    }
}
