using InquisitorAI.Features.InterviewSessions.Dtos;

namespace InquisitorAI.Features.InterviewSessions;

public interface IAiEvaluationService
{
    Task<EvaluationResultDto> EvaluateAsync(EvaluateAnswerRequest request, CancellationToken ct = default);
}
