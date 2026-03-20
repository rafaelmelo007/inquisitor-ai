using InquisitorAI.UI.Auth;
using InquisitorAI.UI.Forms;
using InquisitorAI.UI.Services.Api;
using InquisitorAI.UI.Services.Local;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InquisitorAI.UI.Setup;

public static class ApplicationSetup
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration config)
    {
        // Configuration
        services.AddSingleton(config);

        // Token store
        services.AddSingleton<ITokenStore, WindowsCredentialTokenStore>();

        // API client
        services.AddHttpClient<IApiClient, ApiClient>(client =>
        {
            var baseUrl = config["ApiBaseUrl"] ?? "http://localhost:8080";
            client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
        });

        // Local services
        services.AddSingleton<ISpeechSynthesisService, WindowsTtsService>();
        services.AddSingleton<IAudioRecordingService, NAudioRecordingService>();
        services.AddHttpClient<ISpeechToTextService, OpenAiWhisperService>();

        // Auth
        services.AddTransient<OAuthHandler>();

        // Forms
        services.AddTransient<LoginForm>();
        services.AddTransient<MainForm>();
        services.AddTransient<InterviewForm>();
        services.AddTransient<ResultForm>();
        services.AddTransient<HistoryForm>();
        services.AddTransient<ProfileForm>();

        return services;
    }
}
