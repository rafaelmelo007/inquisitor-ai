using System.Diagnostics;
using InquisitorAI.UI.Dtos;
using InquisitorAI.UI.Services.Api;

namespace InquisitorAI.UI.Forms;

public partial class HistoryForm : Form
{
    private readonly IApiClient _apiClient;
    private List<InterviewSessionDto> _sessions = [];

    public HistoryForm(IApiClient apiClient)
    {
        _apiClient = apiClient;
        InitializeComponent();

        btnOpenReport.Click += BtnOpenReport_Click;
        btnDelete.Click += BtnDelete_Click;
        btnClose.Click += BtnClose_Click;
        Load += HistoryForm_Load;
    }

    private async void HistoryForm_Load(object? sender, EventArgs e)
    {
        await LoadSessionsAsync();
    }

    private async Task LoadSessionsAsync()
    {
        try
        {
            var sessions = await _apiClient.GetSessionsAsync();
            _sessions = sessions.ToList();

            dgvSessions.DataSource = null;
            dgvSessions.Columns.Clear();

            var dataTable = new System.Data.DataTable();
            dataTable.Columns.Add("Id", typeof(long));
            dataTable.Columns.Add("Questionnaire", typeof(string));
            dataTable.Columns.Add("Date", typeof(string));
            dataTable.Columns.Add("Duration", typeof(string));
            dataTable.Columns.Add("Score", typeof(string));
            dataTable.Columns.Add("Classification", typeof(string));

            foreach (var session in _sessions)
            {
                dataTable.Rows.Add(
                    session.Id,
                    session.QuestionnaireName,
                    session.StartedAt.ToString("yyyy-MM-dd HH:mm"),
                    session.Duration?.ToString(@"hh\:mm\:ss") ?? "N/A",
                    session.FinalScore?.ToString("F1") ?? "N/A",
                    session.Classification ?? "In Progress");
            }

            dgvSessions.DataSource = dataTable;
            dgvSessions.Columns["Id"].Visible = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load sessions: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void BtnOpenReport_Click(object? sender, EventArgs e)
    {
        if (dgvSessions.SelectedRows.Count == 0) return;

        var sessionId = (long)dgvSessions.SelectedRows[0].Cells["Id"].Value;

        try
        {
            var session = await _apiClient.GetSessionDetailsAsync(sessionId);

            if (string.IsNullOrEmpty(session.ReportContent))
            {
                MessageBox.Show("No report available for this session.",
                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var tempPath = Path.Combine(Path.GetTempPath(), $"InquisitorAI_Report_{sessionId}.md");
            await File.WriteAllTextAsync(tempPath, session.ReportContent);
            Process.Start(new ProcessStartInfo
            {
                FileName = tempPath,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to open report: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (dgvSessions.SelectedRows.Count == 0) return;

        var sessionId = (long)dgvSessions.SelectedRows[0].Cells["Id"].Value;
        var confirm = MessageBox.Show("Are you sure you want to delete this session?",
            "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        try
        {
            await _apiClient.DeleteSessionAsync(sessionId);
            await LoadSessionsAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to delete session: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnClose_Click(object? sender, EventArgs e)
    {
        Close();
    }
}
