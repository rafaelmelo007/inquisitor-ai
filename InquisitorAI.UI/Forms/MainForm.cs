using InquisitorAI.UI.Dtos;
using InquisitorAI.UI.Services.Api;
using Microsoft.Extensions.DependencyInjection;

namespace InquisitorAI.UI.Forms;

public partial class MainForm : Form
{
    private readonly IApiClient _apiClient;
    private readonly ITokenStore _tokenStore;
    private readonly IServiceProvider _serviceProvider;
    private readonly List<QuestionnaireDto> _questionnaires = [];

    public MainForm(IApiClient apiClient, ITokenStore tokenStore, IServiceProvider serviceProvider)
    {
        _apiClient = apiClient;
        _tokenStore = tokenStore;
        _serviceProvider = serviceProvider;
        InitializeComponent();

        btnUpload.Click += BtnUpload_Click;
        btnStartInterview.Click += BtnStartInterview_Click;
        btnHistory.Click += BtnHistory_Click;
        btnProfile.Click += BtnProfile_Click;
        btnLogout.Click += BtnLogout_Click;
        Load += MainForm_Load;
    }

    private async void MainForm_Load(object? sender, EventArgs e)
    {
        await LoadQuestionnairesAsync();
    }

    private async Task LoadQuestionnairesAsync()
    {
        try
        {
            var questionnaires = await _apiClient.GetQuestionnairesAsync();
            _questionnaires.Clear();
            _questionnaires.AddRange(questionnaires);

            lvQuestionnaires.Items.Clear();
            foreach (var q in _questionnaires)
            {
                var item = new ListViewItem(q.Name)
                {
                    Tag = q.Id
                };
                item.SubItems.Add(q.CreatedByDisplayName);
                item.SubItems.Add(q.IsPublic ? "Yes" : "No");
                item.SubItems.Add(q.QuestionCount.ToString());
                lvQuestionnaires.Items.Add(item);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load questionnaires: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void BtnUpload_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Markdown files (*.md)|*.md",
            Title = "Select Questionnaire File"
        };

        if (dialog.ShowDialog() != DialogResult.OK)
            return;

        try
        {
            await _apiClient.ImportQuestionnaireAsync(dialog.FileName, isPublic: false);
            MessageBox.Show("Questionnaire imported successfully.",
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadQuestionnairesAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to import questionnaire: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnStartInterview_Click(object? sender, EventArgs e)
    {
        if (lvQuestionnaires.SelectedItems.Count == 0)
        {
            MessageBox.Show("Please select a questionnaire first.",
                "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var selectedId = (long)lvQuestionnaires.SelectedItems[0].Tag;
        var interviewForm = _serviceProvider.GetRequiredService<InterviewForm>();
        interviewForm.QuestionnaireId = selectedId;
        interviewForm.ShowDialog(this);
    }

    private void BtnHistory_Click(object? sender, EventArgs e)
    {
        var historyForm = _serviceProvider.GetRequiredService<HistoryForm>();
        historyForm.ShowDialog(this);
    }

    private void BtnProfile_Click(object? sender, EventArgs e)
    {
        var profileForm = _serviceProvider.GetRequiredService<ProfileForm>();
        profileForm.ShowDialog(this);
    }

    private void BtnLogout_Click(object? sender, EventArgs e)
    {
        _tokenStore.Clear();
        Hide();
        var loginForm = _serviceProvider.GetRequiredService<LoginForm>();
        loginForm.FormClosed += (_, _) => Close();
        loginForm.Show();
    }
}
