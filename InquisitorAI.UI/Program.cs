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

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var services = new ServiceCollection();
        services.ConfigureServices(configuration);

        var serviceProvider = services.BuildServiceProvider();
        var loginForm = serviceProvider.GetRequiredService<LoginForm>();

        Application.Run(loginForm);
    }
}
