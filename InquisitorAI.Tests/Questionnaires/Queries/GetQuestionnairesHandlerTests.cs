using System.Data;
using Bogus;
using Dapper;
using FluentAssertions;
using InquisitorAI.Features.Questionnaires.Dtos;
using InquisitorAI.Features.Questionnaires.Queries;
using Moq;
using Moq.Dapper;

namespace InquisitorAI.Tests.Questionnaires.Queries;

public class GetQuestionnairesHandlerTests
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task HandleAsync_ReturnsExpectedDtos()
    {
        // Arrange
        var mockDb = new Mock<IDbConnection>();
        var fixedNow = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);

        var expectedDtos = new List<QuestionnaireDto>
        {
            new(_faker.Random.Long(1, 10000), _faker.Lorem.Sentence(), _faker.Random.Long(1, 100),
                _faker.Person.FullName, true, _faker.Random.Int(1, 20), fixedNow.AddDays(-1)),
            new(_faker.Random.Long(1, 10000), _faker.Lorem.Sentence(), _faker.Random.Long(1, 100),
                _faker.Person.FullName, false, _faker.Random.Int(1, 20), fixedNow.AddDays(-2))
        };

        mockDb.SetupDapper(c => c.QueryAsync<QuestionnaireDto>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null,
                null,
                null))
            .ReturnsAsync(expectedDtos);

        var handler = new GetQuestionnairesHandler(mockDb.Object);
        var query = new GetQuestionnairesQuery(UserId: _faker.Random.Long(1, 10000));

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        var resultList = result.ToList();
        resultList.Should().HaveCount(2);
        resultList[0].Name.Should().Be(expectedDtos[0].Name);
        resultList[1].Name.Should().Be(expectedDtos[1].Name);
    }
}
