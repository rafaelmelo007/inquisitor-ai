using System.Globalization;
using System.Speech.Synthesis;

namespace InquisitorAI.UI.Services.Local;

public class WindowsTtsService : ISpeechSynthesisService
{
    public async Task SpeakAsync(string text, CancellationToken ct, string? language = null)
    {
        await Task.Run(() =>
        {
            using var synthesizer = new SpeechSynthesizer();
            synthesizer.SetOutputToDefaultAudioDevice();

            var culture = GetCultureForLanguage(language);
            if (culture != null)
            {
                var voice = synthesizer.GetInstalledVoices()
                    .FirstOrDefault(v => v.Enabled && v.VoiceInfo.Culture.TwoLetterISOLanguageName == culture);

                if (voice != null)
                    synthesizer.SelectVoice(voice.VoiceInfo.Name);
            }

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

    private static string? GetCultureForLanguage(string? language)
    {
        return language switch
        {
            "Español" or "Spanish" => "es",
            "Português" or "Portuguese" => "pt",
            "English" => "en",
            _ => null
        };
    }
}
