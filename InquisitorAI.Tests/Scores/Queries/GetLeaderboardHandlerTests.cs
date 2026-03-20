using System.Data;
using Bogus;
using Dapper;
using FluentAssertions;
using InquisitorAI.Features.Scores.Dtos;
using InquisitorAI.Features.Scores.Queries;
using Moq;
using Moq.Dapper;

namespace InquisitorAI.Tests.Scores.Queries;

public class GetLeaderboardHandlerTests
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task HandleAsync_ReturnsRankedEntriesOrderedByBestScore()
    {
        // Arrange
        var mockDb = new Mock<IDbConnection>();

        var expectedEntries = new List<LeaderboardEntryDto>
        {
            new(
                Rank: 1L,
                UserId: _faker.Random.Long(1, 10000),
                DisplayName: _faker.Person.FullName,
                AvatarUrl: _faker.Internet.Avatar(),
                BestScore: 9.5m,
                SessionCount: 5L,
                AverageScore: 8.8m),
            new(
                Rank: 2L,
                UserId: _faker.Random.Long(1, 10000),
                DisplayName: _faker.Person.FullName,
                AvatarUrl: _faker.Internet.Avatar(),
                BestScore: 8.2m,
                SessionCount: 3L,
                AverageScore: 7.5m),
            new(
                Rank: 3L,
                UserId: _faker.Random.Long(1, 10000),
                DisplayName: _faker.Person.FullName,
                AvatarUrl: null,
                BestScore: 7.0m,
                SessionCount: 2L,
                AverageScore: 6.9m)
        };

        mockDb.SetupDapper(c => c.QueryAsync<LeaderboardEntryDto>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null,
                null,
                null))
            .ReturnsAsync(expectedEntries);

        var handler = new GetLeaderboardHandler(mockDb.Object);
        var query = new GetLeaderboardQuery();

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        var resultList = result.ToList();
        resultList.Should().HaveCount(3);
        resultList[0].BestScore.Should().Be(9.5m);
        resultList[1].BestScore.Should().Be(8.2m);
        resultList[2].BestScore.Should().Be(7.0m);

        resultList.Should().BeInDescendingOrder(e => e.BestScore);
        resultList[0].Rank.Should().Be(1);
        resultList[1].Rank.Should().Be(2);
        resultList[2].Rank.Should().Be(3);
    }
}
