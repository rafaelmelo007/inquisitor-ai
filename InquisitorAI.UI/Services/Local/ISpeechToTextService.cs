namespace InquisitorAI.UI.Services.Local;

public interface ISpeechToTextService
{
    Task<string> TranscribeAsync(string audioFilePath, CancellationToken ct, string? language = null);
}
