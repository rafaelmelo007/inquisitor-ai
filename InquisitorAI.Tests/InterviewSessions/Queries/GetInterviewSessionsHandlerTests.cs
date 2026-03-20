using System.Data;
using Bogus;
using Dapper;
using FluentAssertions;
using InquisitorAI.Features.InterviewSessions.Dtos;
using InquisitorAI.Features.InterviewSessions.Queries;
using Moq;
using Moq.Dapper;

namespace InquisitorAI.Tests.InterviewSessions.Queries;

public class GetInterviewSessionsHandlerTests
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task HandleAsync_ReturnsExpectedDtos()
    {
        // Arrange
        var mockDb = new Mock<IDbConnection>();
        var fixedNow = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var userId = _faker.Random.Long(1, 10000);

        var expectedDtos = new List<InterviewSessionDto>
        {
            new(
                Id: _faker.Random.Long(1, 10000),
                UserId: userId,
                QuestionnaireId: _faker.Random.Long(1, 100),
                QuestionnaireName: _faker.Lorem.Sentence(),
                StartedAt: fixedNow.AddDays(-2),
                EndedAt: fixedNow.AddDays(-2).AddHours(1),
                DurationSeconds: 3600,
                FinalScore: 8.5m,
                Classification: "Approved",
                ReportContent: null,
                Answers: []),
            new(
                Id: _faker.Random.Long(1, 10000),
                UserId: userId,
                QuestionnaireId: _faker.Random.Long(1, 100),
                QuestionnaireName: _faker.Lorem.Sentence(),
                StartedAt: fixedNow.AddDays(-1),
                EndedAt: null,
                DurationSeconds: null,
                FinalScore: null,
                Classification: null,
                ReportContent: null,
                Answers: [])
        };

        mockDb.SetupDapper(c => c.QueryAsync<InterviewSessionDto>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null,
                null,
                null))
            .ReturnsAsync(expectedDtos);

        var handler = new GetInterviewSessionsHandler(mockDb.Object);
        var query = new GetInterviewSessionsQuery(userId);

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        var resultList = result.ToList();
        resultList.Should().HaveCount(2);
        resultList[0].QuestionnaireName.Should().Be(expectedDtos[0].QuestionnaireName);
        resultList[1].QuestionnaireName.Should().Be(expectedDtos[1].QuestionnaireName);
    }
}
