namespace InquisitorAI.Features.InterviewSessions.Dtos;

public record StartSessionRequest(long QuestionnaireId, string? Language = null);
