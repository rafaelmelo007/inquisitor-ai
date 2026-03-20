namespace InquisitorAI.UI.Services.Local;

public interface IAudioRecordingService
{
    Task StartRecordingAsync(string outputFilePath, CancellationToken ct);
    Task StopRecordingAsync();
    bool IsRecording { get; }
}
