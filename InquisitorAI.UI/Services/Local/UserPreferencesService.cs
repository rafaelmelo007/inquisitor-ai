using System.Text.Json;

namespace InquisitorAI.UI.Services.Local;

public class UserPreferencesService
{
    private readonly string _filePath;
    private UserPreferences _preferences;

    public UserPreferencesService()
    {
        var appData = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "InquisitorAI");
        Directory.CreateDirectory(appData);
        _filePath = Path.Combine(appData, "preferences.json");
        _preferences = Load();
    }

    public string Language
    {
        get => _preferences.Language;
        set { _preferences = _preferences with { Language = value }; Save(); }
    }

    public bool SpeakQuestions
    {
        get => _preferences.SpeakQuestions;
        set { _preferences = _preferences with { SpeakQuestions = value }; Save(); }
    }

    public int ThinkingTimeSeconds
    {
        get => _preferences.ThinkingTimeSeconds;
        set { _preferences = _preferences with { ThinkingTimeSeconds = value }; Save(); }
    }

    public int AnswerTimeSeconds
    {
        get => _preferences.AnswerTimeSeconds;
        set { _preferences = _preferences with { AnswerTimeSeconds = value }; Save(); }
    }

    private UserPreferences Load()
    {
        if (!File.Exists(_filePath))
            return new UserPreferences();

        try
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<UserPreferences>(json) ?? new UserPreferences();
        }
        catch
        {
            return new UserPreferences();
        }
    }

    private void Save()
    {
        var json = JsonSerializer.Serialize(_preferences, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }

    private record UserPreferences(
        string Language = "English",
        bool SpeakQuestions = false,
        int ThinkingTimeSeconds = 0,
        int AnswerTimeSeconds = 0);
}
