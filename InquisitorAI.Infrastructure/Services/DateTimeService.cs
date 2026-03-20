namespace InquisitorAI.Infrastructure.Services;

using InquisitorAI.Features.Shared;

public class DateTimeService : IDateTimeService
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
