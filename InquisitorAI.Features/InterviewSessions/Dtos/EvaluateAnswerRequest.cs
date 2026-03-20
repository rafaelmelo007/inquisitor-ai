namespace InquisitorAI.Features.InterviewSessions.Dtos;

public record EvaluateAnswerRequest(
    string QuestionText,
    string IdealAnswer,
    string Transcript);
