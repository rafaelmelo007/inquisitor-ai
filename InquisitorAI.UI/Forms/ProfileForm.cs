using InquisitorAI.UI.Dtos;
using InquisitorAI.UI.Services.Api;

namespace InquisitorAI.UI.Forms;

public partial class ProfileForm : Form
{
    private readonly IApiClient _apiClient;
    private UserDto? _currentUser;

    public ProfileForm(IApiClient apiClient)
    {
        _apiClient = apiClient;
        InitializeComponent();

        btnSave.Click += BtnSave_Click;
        btnClose.Click += BtnClose_Click;
        Load += ProfileForm_Load;
    }

    private async void ProfileForm_Load(object? sender, EventArgs e)
    {
        try
        {
            _currentUser = await _apiClient.GetCurrentUserAsync();
            lblEmailValue.Text = _currentUser.Email;
            lblProviderValue.Text = _currentUser.Provider;
            txtDisplayName.Text = _currentUser.DisplayName;

            if (!string.IsNullOrEmpty(_currentUser.AvatarUrl))
            {
                try
                {
                    using var httpClient = new HttpClient();
                    var imageBytes = await httpClient.GetByteArrayAsync(_currentUser.AvatarUrl);
                    using var ms = new MemoryStream(imageBytes);
                    picAvatar.Image = Image.FromStream(ms);
                }
                catch
                {
                    // Avatar load failure is non-critical
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load profile: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void BtnSave_Click(object? sender, EventArgs e)
    {
        try
        {
            btnSave.Enabled = false;
            var displayName = txtDisplayName.Text.Trim();

            if (string.IsNullOrEmpty(displayName))
            {
                MessageBox.Show("Display name cannot be empty.",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _currentUser = await _apiClient.UpdateProfileAsync(displayName, _currentUser?.AvatarUrl);
            MessageBox.Show("Profile updated successfully.",
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to update profile: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnSave.Enabled = true;
        }
    }

    private void BtnClose_Click(object? sender, EventArgs e)
    {
        Close();
    }
}
