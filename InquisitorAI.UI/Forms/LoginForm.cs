using InquisitorAI.UI.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace InquisitorAI.UI.Forms;

public partial class LoginForm : Form
{
    private readonly OAuthHandler _oAuthHandler;
    private readonly IServiceProvider _serviceProvider;

    public LoginForm(OAuthHandler oAuthHandler, IServiceProvider serviceProvider)
    {
        _oAuthHandler = oAuthHandler;
        _serviceProvider = serviceProvider;
        InitializeComponent();

        btnGoogle.Click += BtnGoogle_Click;
        btnGitHub.Click += BtnGitHub_Click;
        btnLinkedIn.Click += BtnLinkedIn_Click;
    }

    private async void BtnGoogle_Click(object? sender, EventArgs e)
    {
        await AuthenticateAsync(OAuthProvider.Google);
    }

    private async void BtnGitHub_Click(object? sender, EventArgs e)
    {
        await AuthenticateAsync(OAuthProvider.GitHub);
    }

    private async void BtnLinkedIn_Click(object? sender, EventArgs e)
    {
        await AuthenticateAsync(OAuthProvider.LinkedIn);
    }

    private async Task AuthenticateAsync(OAuthProvider provider)
    {
        try
        {
            SetButtonsEnabled(false);
            var result = await _oAuthHandler.AuthenticateAsync(provider);

            if (result != null)
            {
                Hide();
                var mainForm = _serviceProvider.GetRequiredService<MainForm>();
                mainForm.FormClosed += (_, _) => Close();
                mainForm.Show();
            }
            else
            {
                MessageBox.Show("Authentication failed. Please try again.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Authentication error: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            SetButtonsEnabled(true);
        }
    }

    private void SetButtonsEnabled(bool enabled)
    {
        btnGoogle.Enabled = enabled;
        btnGitHub.Enabled = enabled;
        btnLinkedIn.Enabled = enabled;
    }
}
