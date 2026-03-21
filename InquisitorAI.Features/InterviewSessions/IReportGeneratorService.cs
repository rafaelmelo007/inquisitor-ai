using InquisitorAI.Features.InterviewSessions.Dtos;

namespace InquisitorAI.Features.InterviewSessions;

public interface IReportGeneratorService
{
    string Generate(InterviewSessionDto session, string? language = null);
}
