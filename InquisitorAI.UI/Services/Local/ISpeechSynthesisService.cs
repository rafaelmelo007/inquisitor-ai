namespace InquisitorAI.UI.Services.Local;

public interface ISpeechSynthesisService
{
    Task SpeakAsync(string text, CancellationToken ct);
    IEnumerable<string> GetAvailableVoices();
}
