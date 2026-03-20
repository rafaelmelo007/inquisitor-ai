namespace InquisitorAI.Features.Shared;

public interface IDateTimeService
{
    DateTimeOffset UtcNow { get; }
}
