using InquisitorAI.UI.Forms;
using InquisitorAI.UI.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InquisitorAI.UI;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Load .env from solution root — walk up from current directory until found
        var dir = AppContext.BaseDirectory;
        while (dir is not null && !File.Exists(Path.Combine(dir, ".env")))
            dir = Directory.GetParent(dir)?.FullName;
        if (dir is not null)
            DotNetEnv.Env.Load(Path.Combine(dir, ".env"));

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection();
        services.ConfigureServices(configuration);

        var serviceProvider = services.BuildServiceProvider();
        var loginForm = serviceProvider.GetRequiredService<LoginForm>();

        Application.Run(loginForm);
    }
}
