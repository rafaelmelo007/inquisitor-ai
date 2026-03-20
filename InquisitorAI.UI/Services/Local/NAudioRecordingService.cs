using NAudio.Wave;

namespace InquisitorAI.UI.Services.Local;

public class NAudioRecordingService : IAudioRecordingService, IDisposable
{
    private WaveInEvent? _waveIn;
    private WaveFileWriter? _writer;
    private TaskCompletionSource? _recordingTcs;

    public bool IsRecording => _waveIn != null;

    public Task StartRecordingAsync(string outputFilePath, CancellationToken ct)
    {
        if (IsRecording)
            throw new InvalidOperationException("Already recording.");

        var directory = Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        _waveIn = new WaveInEvent
        {
            WaveFormat = new WaveFormat(16000, 16, 1)
        };

        _writer = new WaveFileWriter(outputFilePath, _waveIn.WaveFormat);
        _recordingTcs = new TaskCompletionSource();

        _waveIn.DataAvailable += (_, e) =>
        {
            _writer.Write(e.Buffer, 0, e.BytesRecorded);
        };

        _waveIn.RecordingStopped += (_, _) =>
        {
            _recordingTcs?.TrySetResult();
        };

        ct.Register(() =>
        {
            if (IsRecording)
                _ = StopRecordingAsync();
        });

        _waveIn.StartRecording();
        return Task.CompletedTask;
    }

    public async Task StopRecordingAsync()
    {
        if (!IsRecording)
            return;

        var tcs = _recordingTcs;
        _waveIn?.StopRecording();

        if (tcs != null)
            await tcs.Task;

        Cleanup();
    }

    private void Cleanup()
    {
        _writer?.Dispose();
        _writer = null;

        _waveIn?.Dispose();
        _waveIn = null;

        _recordingTcs = null;
    }

    public void Dispose()
    {
        Cleanup();
        GC.SuppressFinalize(this);
    }
}
