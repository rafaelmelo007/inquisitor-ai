using System.Speech.Synthesis;

namespace InquisitorAI.UI.Services.Local;

public class WindowsTtsService : ISpeechSynthesisService
{
    public async Task SpeakAsync(string text, CancellationToken ct)
    {
        await Task.Run(() =>
        {
            using var synthesizer = new SpeechSynthesizer();
            synthesizer.SetOutputToDefaultAudioDevice();
            synthesizer.Speak(text);
        }, ct);
    }

    public IEnumerable<string> GetAvailableVoices()
    {
        using var synthesizer = new SpeechSynthesizer();
        return synthesizer.GetInstalledVoices()
            .Where(v => v.Enabled)
            .Select(v => v.VoiceInfo.Name)
            .ToList();
    }
}
