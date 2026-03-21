namespace InquisitorAI.UI.Services.Local;

public interface ISpeechSynthesisService
{
    Task SpeakAsync(string text, CancellationToken ct, string? language = null);
    IEnumerable<string> GetAvailableVoices();
}
