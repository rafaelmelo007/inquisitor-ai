using System.Diagnostics;
using InquisitorAI.UI.Dtos;
using InquisitorAI.UI.Services.Api;
using InquisitorAI.UI.Services.Local;
using Microsoft.Extensions.DependencyInjection;

namespace InquisitorAI.UI.Forms;

public partial class MainForm : Form
{
    private readonly IApiClient _apiClient;
    private readonly ITokenStore _tokenStore;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISpeechSynthesisService _ttsService;
    private readonly IAudioRecordingService _recordingService;
    private readonly ISpeechToTextService _speechToTextService;
    private readonly UserPreferencesService _preferencesService;

    // Data
    private List<QuestionnaireDto> _questionnaires = [];
    private List<InterviewSessionDto> _sessions = [];
    private UserDto? _currentUser;

    // Interview state
    private InterviewSessionDto? _activeSession;
    private QuestionnaireDetailDto? _activeQuestionnaire;
    private int _currentQuestionIndex;
    private string? _currentRecordingPath;
    private readonly Dictionary<int, SessionAnswerDto> _answers = new();
    private readonly HashSet<int> _feedbackRevealed = new();
    private bool _isInterviewing;

    // Timers
    private System.Windows.Forms.Timer? _countdownTimer;
    private int _countdownRemaining;
    private bool _isThinkingPhase;

    // Localization
    private UiTranslations _t = UiStrings.Get("English");
    private string CurrentLanguage => _preferencesService.Language;

    // Tree node identification
    private record TreeNodeTag(string Type, long Id, long QuestionnaireId = 0);

    public MainForm(
        IApiClient apiClient,
        ITokenStore tokenStore,
        IServiceProvider serviceProvider,
        ISpeechSynthesisService ttsService,
        IAudioRecordingService recordingService,
        ISpeechToTextService speechToTextService,
        UserPreferencesService preferencesService)
    {
        _apiClient = apiClient;
        _tokenStore = tokenStore;
        _serviceProvider = serviceProvider;
        _ttsService = ttsService;
        _recordingService = recordingService;
        _speechToTextService = speechToTextService;
        _preferencesService = preferencesService;
        InitializeComponent();

        // Toolbar events
        btnImport.Click += BtnImport_Click;
        btnSettings.Click += BtnSettings_Click;
        btnLogout.Click += BtnLogout_Click;

        // Tree events
        tvExplorer.AfterSelect += TvExplorer_AfterSelect;
        tvExplorer.KeyDown += TvExplorer_KeyDown;

        // Questionnaire view events
        btnStartSession.Click += BtnStartSession_Click;
        btnDeleteQuestionnaire.Click += BtnDeleteQuestionnaire_Click;

        // Session overview events
        btnResumeSession.Click += BtnResumeSession_Click;
        btnViewReport.Click += BtnViewReport_Click;
        btnDeleteSession.Click += BtnDeleteSession_Click;

        // Interview events
        btnListen.Click += BtnListen_Click;
        btnRecord.Click += BtnRecord_Click;
        btnStop.Click += BtnStop_Click;
        btnPrevious.Click += BtnPrevious_Click;
        btnNext.Click += BtnNext_Click;
        btnShowFeedback.Click += BtnShowFeedback_Click;
        btnFinishEarly.Click += BtnFinishEarly_Click;

        // Right panel question navigation
        lvQuestionStatus.DoubleClick += LvQuestionStatus_DoubleClick;

        // Settings events
        btnSaveSettings.Click += BtnSaveSettings_Click;

        Load += MainForm_Load;
    }

    // ───────────────────────────────────────────────────────────
    // Localization
    // ───────────────────────────────────────────────────────────

    private void ApplyLocalization()
    {
        _t = UiStrings.Get(CurrentLanguage);

        // Toolbar
        lblToolbarTitle.Text = _t.AppTitle;
        btnImport.Text = _t.ImportQuestionnaire;
        btnSettings.Text = _t.Settings;
        btnLogout.Text = _t.Logout;

        // Left panel
        lblLeftTitle.Text = _t.Questionnaires;

        // Welcome
        lblWelcomeTitle.Text = _t.WelcomeTitle;
        lblWelcomeSubtitle.Text = _t.WelcomeSubtitle;

        // Questionnaire view
        btnStartSession.Text = _t.StartNewSession;
        btnDeleteQuestionnaire.Text = _t.DeleteQuestionnaire;

        // Session overview
        lblSessionTitle.Text = _t.SessionOverview;
        btnResumeSession.Text = _t.ResumeSession;
        btnViewReport.Text = _t.ViewReport;
        btnDeleteSession.Text = _t.DeleteSession;
        grpStrengths.Text = _t.Strengths;
        grpImprovements.Text = _t.ImprovementAreas;

        // Interview buttons
        btnListen.Text = _t.Listen;
        btnRecord.Text = _t.Record;
        btnStop.Text = _t.Stop;
        btnPrevious.Text = _t.Previous;
        btnNext.Text = _t.Next;
        btnShowFeedback.Text = _t.ShowAnswer;
        btnFinishEarly.Text = _t.Finish;

        // Right panel
        lblRightTitle.Text = _t.SessionProgress;
        colQStatus.Text = _t.Status;
        colQScore.Text = _t.Score;

        // Settings
        lblSettingsTitle.Text = _t.UserSettings;
        lblSettingsEmail.Text = _t.Email;
        lblSettingsName.Text = _t.DisplayName;
        lblSettingsLanguage.Text = _t.Language;
        chkSpeakQuestions.Text = _t.SpeakQuestions;
        lblThinkingTime.Text = _t.ThinkingTime;
        lblThinkingTimeUnit.Text = _t.TimeUnit;
        lblAnswerTime.Text = _t.AnswerTime;
        lblAnswerTimeUnit.Text = _t.TimeUnit;
        btnSaveSettings.Text = _t.SaveSettings;
    }

    // ───────────────────────────────────────────────────────────
    // Form Load & Data Loading
    // ───────────────────────────────────────────────────────────

    private async void MainForm_Load(object? sender, EventArgs e)
    {
        ApplyLocalization();
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var questionnairesTask = _apiClient.GetQuestionnairesAsync();
            var sessionsTask = _apiClient.GetSessionsAsync();
            var userTask = _apiClient.GetCurrentUserAsync();

            await Task.WhenAll(questionnairesTask, sessionsTask, userTask);

            _questionnaires = questionnairesTask.Result.ToList();
            _sessions = sessionsTask.Result.ToList();
            _currentUser = userTask.Result;

            BuildTreeView();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{_t.FailedToLoadData}: {ex.Message}",
                _t.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ───────────────────────────────────────────────────────────
    // TreeView
    // ───────────────────────────────────────────────────────────

    private void BuildTreeView()
    {
        tvExplorer.BeginUpdate();
        tvExplorer.Nodes.Clear();

        foreach (var q in _questionnaires)
        {
            var qNode = new TreeNode($"{q.Name}  ({q.QuestionCount} {_t.Questions})")
            {
                Tag = new TreeNodeTag("Questionnaire", q.Id),
                NodeFont = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            var questionnaireSessions = _sessions
                .Where(s => s.QuestionnaireId == q.Id)
                .OrderByDescending(s => s.StartedAt)
                .ToList();

            foreach (var session in questionnaireSessions)
            {
                var isInProgress = session.FinishedAt == null && session.Classification == null;
                var statusIcon = isInProgress ? $"[{_t.InProgress}]" : $"[{_t.Completed}]";
                var scoreText = session.FinalScore.HasValue ? $" — {session.FinalScore:F1}" : "";
                var dateText = session.StartedAt.ToString("yyyy-MM-dd HH:mm");

                var sNode = new TreeNode($"{statusIcon} {dateText}{scoreText}")
                {
                    Tag = new TreeNodeTag("Session", session.Id, q.Id),
                    ForeColor = isInProgress ? Color.FromArgb(0, 122, 204) : Color.FromArgb(60, 60, 60)
                };

                qNode.Nodes.Add(sNode);
            }

            tvExplorer.Nodes.Add(qNode);
        }

        tvExplorer.ExpandAll();
        tvExplorer.EndUpdate();
    }

    private void TvExplorer_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        if (_isInterviewing || e.Node?.Tag is not TreeNodeTag tag) return;

        if (tag.Type == "Questionnaire")
        {
            ShowQuestionnaireView(tag.Id);
        }
        else if (tag.Type == "Session")
        {
            ShowSessionOverview(tag.Id);
        }
    }

    private async void TvExplorer_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Delete || _isInterviewing) return;
        if (tvExplorer.SelectedNode?.Tag is not TreeNodeTag tag) return;

        if (tag.Type == "Session")
        {
            var confirm = MessageBox.Show(_t.ConfirmDeleteSession,
                _t.ConfirmDelete, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                await _apiClient.DeleteSessionAsync(tag.Id);
                ShowCenterView(pnlWelcome);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{_t.FailedToDelete}: {ex.Message}",
                    _t.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        else if (tag.Type == "Questionnaire")
        {
            var q = _questionnaires.FirstOrDefault(x => x.Id == tag.Id);
            if (q == null || _currentUser == null || q.CreatedByUserId != _currentUser.Id) return;

            var confirm = MessageBox.Show(_t.ConfirmDeleteQuestionnaire,
                _t.ConfirmDelete, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                await _apiClient.DeleteQuestionnaireAsync(tag.Id);
                ShowCenterView(pnlWelcome);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{_t.FailedToDelete}: {ex.Message}",
                    _t.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // ───────────────────────────────────────────────────────────
    // Center View Switching
    // ───────────────────────────────────────────────────────────

    private void ShowCenterView(Panel viewToShow)
    {
        pnlWelcome.Visible = false;
        pnlQuestionnaireView.Visible = false;
        pnlSessionOverview.Visible = false;
        pnlInterview.Visible = false;
        pnlSettingsView.Visible = false;
        viewToShow.Visible = true;
    }

    private void ShowQuestionnaireView(long questionnaireId)
    {
        var q = _questionnaires.FirstOrDefault(x => x.Id == questionnaireId);
        if (q == null) return;

        lblQuestionnaireName.Text = q.Name;
        lblQuestionnaireInfo.Text = $"{q.QuestionCount} {_t.Questions}  |  {_t.CreatedBy}: {q.CreatedByDisplayName}  |  {(q.IsPublic ? _t.Public : _t.Private)}";

        btnDeleteQuestionnaire.Visible = _currentUser != null && q.CreatedByUserId == _currentUser.Id;
        btnDeleteQuestionnaire.Tag = questionnaireId;
        btnStartSession.Tag = questionnaireId;

        splitContent.Panel2Collapsed = true;
        ShowCenterView(pnlQuestionnaireView);
    }

    private async void ShowSessionOverview(long sessionId)
    {
        try
        {
            var session = _sessions.FirstOrDefault(s => s.Id == sessionId);
            if (session == null) return;

            var isInProgress = session.FinishedAt == null && session.Classification == null;

            lblSessionTitle.Text = _t.SessionOverview;
            lblSessionInfo.Text = $"{session.QuestionnaireName}  |  {session.StartedAt:yyyy-MM-dd HH:mm}" +
                (session.Duration.HasValue ? $"  |  {session.Duration:hh\\:mm\\:ss}" : "");

            lblSessionScoreValue.Text = session.FinalScore.HasValue
                ? session.FinalScore.Value.ToString("F1")
                : "—";

            lblSessionClassification.Text = session.Classification ?? (isInProgress ? _t.InProgress : "");
            lblSessionClassification.ForeColor = (session.Classification?.ToLower()) switch
            {
                "approved" => Color.Green,
                "approvedwithreservations" or "approved with reservations" => Color.Orange,
                "failed" => Color.Red,
                _ => isInProgress ? Color.FromArgb(0, 122, 204) : Color.Black
            };

            btnResumeSession.Visible = isInProgress;
            btnResumeSession.Tag = sessionId;
            btnViewReport.Visible = !isInProgress && session.ReportContent != null;
            btnViewReport.Tag = session;
            btnDeleteSession.Tag = sessionId;

            if (!isInProgress && session.ReportContent != null)
            {
                var detail = await _apiClient.GetSessionDetailsAsync(sessionId);
                grpStrengths.Visible = true;
                grpImprovements.Visible = true;

                var strengths = detail.Answers?
                    .Where(a => a.Score >= 7 && !string.IsNullOrEmpty(a.Strengths))
                    .Select(a => a.Strengths) ?? [];
                var improvements = detail.Answers?
                    .Where(a => a.Score < 7 && !string.IsNullOrEmpty(a.ImprovementSuggestions))
                    .Select(a => a.ImprovementSuggestions) ?? [];

                txtStrengths.Text = string.Join(Environment.NewLine + Environment.NewLine, strengths);
                txtImprovements.Text = string.Join(Environment.NewLine + Environment.NewLine, improvements);
            }
            else
            {
                grpStrengths.Visible = false;
                grpImprovements.Visible = false;
            }

            splitContent.Panel2Collapsed = true;
            ShowCenterView(pnlSessionOverview);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{_t.FailedToLoadData}: {ex.Message}",
                _t.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ───────────────────────────────────────────────────────────
    // Toolbar Actions
    // ───────────────────────────────────────────────────────────

    private async void BtnImport_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = _t.MarkdownFilter,
            Title = _t.SelectFileTitle
        };

        if (dialog.ShowDialog() != DialogResult.OK)
            return;

        try
        {
            await _apiClient.ImportQuestionnaireAsync(dialog.FileName, isPublic: false);
            MessageBox.Show(_t.ImportSuccess,
                _t.SuccessTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{_t.FailedToImport}: {ex.Message}",
                _t.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnSettings_Click(object? sender, EventArgs e)
    {
        if (_isInterviewing)
        {
            MessageBox.Show(_t.FinishInterviewFirst,
                _t.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        lblSettingsEmailValue.Text = _currentUser?.Email ?? "";
        txtSettingsName.Text = _currentUser?.DisplayName ?? "";
        cboLanguage.SelectedItem = _preferencesService.Language;
        if (cboLanguage.SelectedIndex < 0) cboLanguage.SelectedIndex = 0;
        chkSpeakQuestions.Checked = _preferencesService.SpeakQuestions;
        nudThinkingTime.Value = _preferencesService.ThinkingTimeSeconds;
        nudAnswerTime.Value = _preferencesService.AnswerTimeSeconds;

        splitContent.Panel2Collapsed = true;
        ShowCenterView(pnlSettingsView);
    }

    private void BtnLogout_Click(object? sender, EventArgs e)
    {
        if (_isInterviewing)
        {
            var confirm = MessageBox.Show(_t.ConfirmLogout,
                _t.ConfirmTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;
            ExitInterviewMode();
        }

        _tokenStore.Clear();
        Hide();
        var loginForm = _serviceProvider.GetRequiredService<LoginForm>();
        loginForm.FormClosed += (_, _) => Close();
        loginForm.Show();
    }

    // ───────────────────────────────────────────────────────────
    // Questionnaire View Actions
    // ───────────────────────────────────────────────────────────

    private async void BtnStartSession_Click(object? sender, EventArgs e)
    {
        if (btnStartSession.Tag is not long questionnaireId) return;

        try
        {
            aiOverlay.ShowOverlay();
            _activeSession = await _apiClient.StartSessionAsync(questionnaireId, CurrentLanguage);
            _activeQuestionnaire = await _apiClient.GetQuestionnaireByIdAsync(questionnaireId);

            if (_activeQuestionnaire.Questions.Count == 0)
            {
                MessageBox.Show(_t.NoQuestions,
                    _t.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _answers.Clear();
            _feedbackRevealed.Clear();
            _currentQuestionIndex = 0;
            EnterInterviewMode();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{_t.FailedToStart}: {ex.Message}",
                _t.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            aiOverlay.HideOverlay();
        }
    }

    private async void BtnDeleteQuestionnaire_Click(object? sender, EventArgs e)
    {
        if (btnDeleteQuestionnaire.Tag is not long questionnaireId) return;

        var confirm = MessageBox.Show(_t.ConfirmDeleteQuestionnaire,
            _t.ConfirmDelete, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;

        try
        {
            await _apiClient.DeleteQuestionnaireAsync(questionnaireId);
            ShowCenterView(pnlWelcome);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{_t.FailedToDelete}: {ex.Message}",
                _t.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ───────────────────────────────────────────────────────────
    // Session Overview Actions
    // ───────────────────────────────────────────────────────────

    private async void BtnResumeSession_Click(object? sender, EventArgs e)
    {
        if (btnResumeSession.Tag is not long sessionId) return;

        try
        {
            aiOverlay.ShowOverlay();

            var sessionDetail = await _apiClient.GetSessionDetailsAsync(sessionId);
            _activeSession = sessionDetail;

            var questionnaire = _questionnaires.FirstOrDefault(q => q.Id == sessionDetail.QuestionnaireId);
            if (questionnaire == null) return;

            _activeQuestionnaire = await _apiClient.GetQuestionnaireByIdAsync(questionnaire.Id);

            if (_activeQuestionnaire.Questions.Count == 0)
            {
                MessageBox.Show(_t.NoQuestions,
                    _t.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _answers.Clear();
            _feedbackRevealed.Clear();

            if (sessionDetail.Answers != null)
            {
                foreach (var answer in sessionDetail.Answers)
                {
                    var qIndex = _activeQuestionnaire.Questions
                        .Select((q, i) => new { q, i })
                        .FirstOrDefault(x => x.q.Id == answer.QuestionId)?.i ?? -1;

                    if (qIndex >= 0)
                        _answers[qIndex] = answer;
                }
            }

            _currentQuestionIndex = 0;
            for (var i = 0; i < _activeQuestionnaire.Questions.Count; i++)
            {
                if (!_answers.ContainsKey(i))
                {
                    _currentQuestionIndex = i;
                    break;
                }
            }

            EnterInterviewMode();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{_t.FailedToResume}: {ex.Message}",
                _t.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            aiOverlay.HideOverlay();
        }
    }

    private void BtnViewReport_Click(object? sender, EventArgs e)
    {
        if (btnViewReport.Tag is not InterviewSessionDto session ||
            string.IsNullOrEmpty(session.ReportContent)) return;

        try
        {
            var tempPath = Path.Combine(Path.GetTempPath(), $"InquisitorAI_Report_{session.Id}.md");
            File.WriteAllText(tempPath, session.ReportContent);
            Process.Start(new ProcessStartInfo
            {
                FileName = tempPath,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{_t.ErrorTitle}: {ex.Message}",
                _t.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void BtnDeleteSession_Click(object? sender, EventArgs e)
    {
        if (btnDeleteSession.Tag is not long sessionId) return;

        var confirm = MessageBox.Show(_t.ConfirmDeleteSession,
            _t.ConfirmDelete, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;

        try
        {
            await _apiClient.DeleteSessionAsync(sessionId);
            ShowCenterView(pnlWelcome);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{_t.FailedToDelete}: {ex.Message}",
                _t.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ───────────────────────────────────────────────────────────
    // Interview Mode
    // ───────────────────────────────────────────────────────────

    private void EnterInterviewMode()
    {
        _isInterviewing = true;
        splitContent.Panel2Collapsed = false;
        splitContent.Panel2MinSize = 300;

        // Defer splitter distance until layout is complete
        BeginInvoke(() =>
        {
            if (splitContent.Width > 600)
                splitContent.SplitterDistance = splitContent.Width - 300;
        });
        ShowCenterView(pnlInterview);

        progressBar.Maximum = _activeQuestionnaire!.Questions.Count;
        InitializeRightPanel();
        DisplayCurrentQuestion();
    }

    private void ExitInterviewMode()
    {
        StopCountdown();
        _isInterviewing = false;
        _activeSession = null;
        _activeQuestionnaire = null;
        _answers.Clear();
        _feedbackRevealed.Clear();
        splitContent.Panel2Collapsed = true;
        ShowCenterView(pnlWelcome);
    }

    private async void DisplayCurrentQuestion()
    {
        if (_activeQuestionnaire == null) return;

        StopCountdown();

        var question = _activeQuestionnaire.Questions[_currentQuestionIndex];
        progressBar.Value = _currentQuestionIndex + 1;
        lblProgress.Text = $"{_t.Question} {_currentQuestionIndex + 1} {_t.Of} {_activeQuestionnaire.Questions.Count}";
        lblCategory.Text = $"{_t.Category}: {question.Category ?? "N/A"}";
        lblDifficulty.Text = $"{_t.Difficulty}: {question.Difficulty ?? "N/A"}";
        RenderQuestionText(question.QuestionText);

        btnPrevious.Enabled = _currentQuestionIndex > 0;
        btnStop.Enabled = false;

        var wasAnswered = _answers.ContainsKey(_currentQuestionIndex);
        var wasFeedbackRevealed = _feedbackRevealed.Contains(_currentQuestionIndex);

        if (wasAnswered)
        {
            DisplayFeedbackSections(_answers[_currentQuestionIndex]);
            btnRecord.Enabled = false;
            btnListen.Enabled = true;
            btnNext.Enabled = true;
            btnShowFeedback.Enabled = false;
        }
        else if (wasFeedbackRevealed)
        {
            rtbFeedback.Clear();
            AppendSection(_t.FeedbackRevealed, "", Color.Gray);
            AppendSection(_t.IdealAnswer, question.IdealAnswer, Color.FromArgb(0, 100, 180));
            btnRecord.Enabled = false;
            btnListen.Enabled = true;
            btnNext.Enabled = true;
            btnShowFeedback.Enabled = false;
        }
        else
        {
            rtbFeedback.Clear();
            btnRecord.Enabled = true;
            btnListen.Enabled = true;
            btnNext.Enabled = false;
            btnShowFeedback.Enabled = true;

            // Auto-speak question if enabled
            if (_preferencesService.SpeakQuestions)
            {
                try
                {
                    btnListen.Enabled = false;
                    await _ttsService.SpeakAsync(question.QuestionText, CancellationToken.None, CurrentLanguage);
                }
                catch { /* TTS failure is non-critical */ }
                finally
                {
                    btnListen.Enabled = true;
                }
            }

            // Start thinking countdown if configured
            var thinkingTime = _preferencesService.ThinkingTimeSeconds;
            if (thinkingTime > 0)
            {
                StartCountdown(thinkingTime, isThinking: true);
            }
        }

        UpdateRightPanel();

        if (_currentQuestionIndex < lvQuestionStatus.Items.Count)
        {
            lvQuestionStatus.Items[_currentQuestionIndex].Selected = true;
            lvQuestionStatus.Items[_currentQuestionIndex].EnsureVisible();
        }
    }

    // ───────────────────────────────────────────────────────────
    // Countdown Timer
    // ───────────────────────────────────────────────────────────

    private void StartCountdown(int seconds, bool isThinking)
    {
        StopCountdown();
        _countdownRemaining = seconds;
        _isThinkingPhase = isThinking;

        lblTimer.Visible = true;
        UpdateTimerDisplay();

        _countdownTimer = new System.Windows.Forms.Timer { Interval = 1000 };
        _countdownTimer.Tick += CountdownTimer_Tick;
        _countdownTimer.Start();
    }

    private void StopCountdown()
    {
        if (_countdownTimer != null)
        {
            _countdownTimer.Stop();
            _countdownTimer.Tick -= CountdownTimer_Tick;
            _countdownTimer.Dispose();
            _countdownTimer = null;
        }
        lblTimer.Visible = false;
    }

    private void CountdownTimer_Tick(object? sender, EventArgs e)
    {
        _countdownRemaining--;

        if (_countdownRemaining <= 0)
        {
            StopCountdown();

            if (_isThinkingPhase)
            {
                // Thinking time expired — flash a brief visual hint
                lblTimer.Text = _t.TimeUp;
                lblTimer.Visible = true;
            }
            else
            {
                // Answer time expired — auto-stop recording
                if (btnStop.Enabled)
                {
                    BtnStop_Click(this, EventArgs.Empty);
                }
            }
            return;
        }

        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        var prefix = _isThinkingPhase ? _t.ThinkingLabel : _t.RecordingLabel;
        var minutes = _countdownRemaining / 60;
        var seconds = _countdownRemaining % 60;
        lblTimer.Text = $"{prefix} {minutes}:{seconds:D2}";
        lblTimer.ForeColor = _countdownRemaining <= 10
            ? Color.FromArgb(200, 50, 50)
            : _isThinkingPhase
                ? Color.FromArgb(0, 122, 204)
                : Color.FromArgb(200, 50, 50);
    }

    private async void BtnListen_Click(object? sender, EventArgs e)
    {
        if (_activeQuestionnaire == null) return;

        try
        {
            btnListen.Enabled = false;
            var question = _activeQuestionnaire.Questions[_currentQuestionIndex];
            await _ttsService.SpeakAsync(question.QuestionText, CancellationToken.None, CurrentLanguage);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"TTS: {ex.Message}",
                _t.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnListen.Enabled = true;
        }
    }

    private async void BtnRecord_Click(object? sender, EventArgs e)
    {
        try
        {
            // Stop any thinking countdown
            StopCountdown();

            var tempDir = Path.Combine(Path.GetTempPath(), "InquisitorAI");
            Directory.CreateDirectory(tempDir);
            _currentRecordingPath = Path.Combine(tempDir, $"recording_{DateTime.Now:yyyyMMdd_HHmmss}.wav");

            await _recordingService.StartRecordingAsync(_currentRecordingPath, CancellationToken.None);

            btnRecord.Enabled = false;
            btnStop.Enabled = true;
            btnListen.Enabled = false;
            btnShowFeedback.Enabled = false;
            btnPrevious.Enabled = false;
            recordingIndicator.StartPulsing();

            // Start answer countdown if configured
            var answerTime = _preferencesService.AnswerTimeSeconds;
            if (answerTime > 0)
            {
                StartCountdown(answerTime, isThinking: false);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{_t.Record}: {ex.Message}",
                _t.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void BtnStop_Click(object? sender, EventArgs e)
    {
        if (_activeQuestionnaire == null || _activeSession == null || _currentRecordingPath == null) return;

        try
        {
            StopCountdown();
            await _recordingService.StopRecordingAsync();
            recordingIndicator.StopPulsing();
            btnStop.Enabled = false;

            aiOverlay.ShowOverlay();

            var transcript = await _speechToTextService.TranscribeAsync(
                _currentRecordingPath, CancellationToken.None, CurrentLanguage);

            var question = _activeQuestionnaire.Questions[_currentQuestionIndex];
            var answer = await _apiClient.SubmitAnswerAsync(_activeSession.Id, question.Id, transcript);

            _answers[_currentQuestionIndex] = answer;
            DisplayFeedbackSections(answer);
            UpdateRightPanel();

            btnRecord.Enabled = false;
            btnShowFeedback.Enabled = false;
            btnNext.Enabled = true;
            btnPrevious.Enabled = _currentQuestionIndex > 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{_t.ErrorTitle}: {ex.Message}",
                _t.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            btnRecord.Enabled = true;
            btnPrevious.Enabled = _currentQuestionIndex > 0;
        }
        finally
        {
            aiOverlay.HideOverlay();
            CleanupRecording();
        }
    }

    private async void BtnNext_Click(object? sender, EventArgs e)
    {
        if (_activeQuestionnaire == null || _activeSession == null) return;

        _currentQuestionIndex++;

        if (_currentQuestionIndex >= _activeQuestionnaire.Questions.Count)
        {
            await FinishSession();
        }
        else
        {
            DisplayCurrentQuestion();
        }
    }

    private void BtnPrevious_Click(object? sender, EventArgs e)
    {
        if (_currentQuestionIndex > 0)
        {
            _currentQuestionIndex--;
            DisplayCurrentQuestion();
        }
    }

    private void BtnShowFeedback_Click(object? sender, EventArgs e)
    {
        if (_activeQuestionnaire == null) return;

        var question = _activeQuestionnaire.Questions[_currentQuestionIndex];
        _feedbackRevealed.Add(_currentQuestionIndex);

        rtbFeedback.Clear();
        AppendSection(_t.FeedbackRevealed, "", Color.Gray);
        AppendSection(_t.IdealAnswer, question.IdealAnswer, Color.FromArgb(0, 100, 180));

        btnRecord.Enabled = false;
        btnShowFeedback.Enabled = false;
        btnNext.Enabled = true;

        UpdateRightPanel();
    }

    private async void BtnFinishEarly_Click(object? sender, EventArgs e)
    {
        if (_activeSession == null) return;

        var confirm = MessageBox.Show(_t.ConfirmFinishEarly,
            _t.FinishSessionTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;

        await FinishSession();
    }

    private async Task FinishSession()
    {
        if (_activeSession == null) return;

        try
        {
            aiOverlay.ShowOverlay();
            var result = await _apiClient.FinishSessionAsync(_activeSession.Id);
            aiOverlay.HideOverlay();

            ShowFinalResult(result);

            _isInterviewing = false;
            splitContent.Panel2Collapsed = true;

            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            aiOverlay.HideOverlay();
            MessageBox.Show($"{_t.ErrorTitle}: {ex.Message}",
                _t.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ShowFinalResult(FinalResultDto result)
    {
        _activeSession = null;
        _activeQuestionnaire = null;

        lblSessionTitle.Text = _t.InterviewComplete;
        lblSessionInfo.Text = "";
        lblSessionScoreValue.Text = result.FinalScore.ToString("F1");
        lblSessionClassification.Text = result.Classification;
        lblSessionClassification.ForeColor = result.Classification.ToLower() switch
        {
            "approved" => Color.Green,
            "approved with reservations" => Color.Orange,
            "failed" => Color.Red,
            _ => Color.Black
        };

        btnResumeSession.Visible = false;
        btnViewReport.Visible = true;
        btnViewReport.Tag = new InterviewSessionDto(0, 0, "", 0, "Finished",
            DateTimeOffset.Now, DateTimeOffset.Now, null, result.FinalScore,
            result.Classification, result.ReportContent);
        btnDeleteSession.Visible = false;

        grpStrengths.Visible = true;
        txtStrengths.Text = result.Strengths;
        grpImprovements.Visible = true;
        txtImprovements.Text = result.ImprovementAreas;

        ShowCenterView(pnlSessionOverview);
    }

    // ───────────────────────────────────────────────────────────
    // Right Panel (Session Progress)
    // ───────────────────────────────────────────────────────────

    private void InitializeRightPanel()
    {
        if (_activeQuestionnaire == null) return;

        lvQuestionStatus.Items.Clear();
        for (var i = 0; i < _activeQuestionnaire.Questions.Count; i++)
        {
            var item = new ListViewItem((i + 1).ToString());
            item.SubItems.Add(_t.Pending);
            item.SubItems.Add("—");
            item.Tag = i;
            lvQuestionStatus.Items.Add(item);
        }

        UpdateRightPanel();
    }

    private void UpdateRightPanel()
    {
        if (_activeQuestionnaire == null) return;

        var answeredCount = 0;
        decimal totalScore = 0;

        for (var i = 0; i < _activeQuestionnaire.Questions.Count; i++)
        {
            if (i >= lvQuestionStatus.Items.Count) break;
            var item = lvQuestionStatus.Items[i];

            if (_answers.TryGetValue(i, out var answer))
            {
                item.SubItems[1].Text = _t.Answered;
                item.SubItems[2].Text = answer.Score?.ToString("F1") ?? "—";
                item.ForeColor = Color.FromArgb(34, 139, 34);
                answeredCount++;
                totalScore += answer.Score ?? 0;
            }
            else if (_feedbackRevealed.Contains(i))
            {
                item.SubItems[1].Text = _t.Skipped;
                item.SubItems[2].Text = "0.0";
                item.ForeColor = Color.Gray;
            }
            else if (i == _currentQuestionIndex)
            {
                item.SubItems[1].Text = _t.Current;
                item.SubItems[2].Text = "—";
                item.ForeColor = Color.FromArgb(0, 122, 204);
            }
            else
            {
                item.SubItems[1].Text = _t.Pending;
                item.SubItems[2].Text = "—";
                item.ForeColor = Color.FromArgb(60, 60, 60);
            }
        }

        lblAnsweredCount.Text = $"{_t.Answered}: {answeredCount} / {_activeQuestionnaire.Questions.Count}";

        if (answeredCount > 0)
        {
            var avgScore = totalScore / answeredCount;
            lblCurrentScore.Text = $"{_t.Score}: {avgScore:F1} / 10";
        }
        else
        {
            lblCurrentScore.Text = $"{_t.Score}: —";
        }
    }

    private void LvQuestionStatus_DoubleClick(object? sender, EventArgs e)
    {
        if (!_isInterviewing || _activeQuestionnaire == null) return;
        if (lvQuestionStatus.SelectedItems.Count == 0) return;

        var item = lvQuestionStatus.SelectedItems[0];
        if (item.Tag is int index)
        {
            _currentQuestionIndex = index;
            DisplayCurrentQuestion();
        }
    }

    // ───────────────────────────────────────────────────────────
    // Settings
    // ───────────────────────────────────────────────────────────

    private async void BtnSaveSettings_Click(object? sender, EventArgs e)
    {
        try
        {
            btnSaveSettings.Enabled = false;

            var displayName = txtSettingsName.Text.Trim();
            if (string.IsNullOrEmpty(displayName))
            {
                MessageBox.Show(_t.DisplayNameRequired,
                    _t.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _currentUser = await _apiClient.UpdateProfileAsync(displayName, _currentUser?.AvatarUrl);

            if (cboLanguage.SelectedItem is string lang)
                _preferencesService.Language = lang;

            _preferencesService.SpeakQuestions = chkSpeakQuestions.Checked;
            _preferencesService.ThinkingTimeSeconds = (int)nudThinkingTime.Value;
            _preferencesService.AnswerTimeSeconds = (int)nudAnswerTime.Value;

            // Re-apply localization with new language
            ApplyLocalization();
            BuildTreeView();

            MessageBox.Show(_t.SettingsSaved,
                _t.SuccessTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{_t.FailedToSave}: {ex.Message}",
                _t.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnSaveSettings.Enabled = true;
        }
    }

    // ───────────────────────────────────────────────────────────
    // Feedback Display Helpers
    // ───────────────────────────────────────────────────────────

    private void DisplayFeedbackSections(SessionAnswerDto answer)
    {
        rtbFeedback.Clear();

        AppendSection(_t.YourAnswer, answer.Transcript ?? _t.NoTranscript, Color.FromArgb(80, 80, 80));
        AppendSection(_t.Score, $"{answer.Score:F1} / 10", Color.FromArgb(120, 60, 180));

        if (!string.IsNullOrWhiteSpace(answer.AiFeedback))
            AppendSection(_t.OverallFeedback, answer.AiFeedback, Color.FromArgb(0, 100, 180));

        if (!string.IsNullOrWhiteSpace(answer.Strengths))
            AppendSection(_t.Strengths, answer.Strengths, Color.FromArgb(34, 139, 34));

        if (!string.IsNullOrWhiteSpace(answer.Weaknesses))
            AppendSection(_t.Weaknesses, answer.Weaknesses, Color.FromArgb(200, 50, 50));

        if (!string.IsNullOrWhiteSpace(answer.ImprovementSuggestions))
            AppendSection(_t.HowToImprove, answer.ImprovementSuggestions, Color.FromArgb(180, 120, 0));

        if (!string.IsNullOrWhiteSpace(answer.IdealAnswer))
            AppendSection(_t.IdealAnswer, answer.IdealAnswer, Color.FromArgb(0, 130, 100));

        rtbFeedback.SelectionStart = 0;
        rtbFeedback.ScrollToCaret();
    }

    private void AppendSection(string title, string content, Color titleColor)
    {
        if (rtbFeedback.TextLength > 0)
            rtbFeedback.AppendText("\n\n");

        var titleStart = rtbFeedback.TextLength;
        rtbFeedback.AppendText(title);
        rtbFeedback.Select(titleStart, title.Length);
        rtbFeedback.SelectionFont = new Font("Segoe UI", 12F, FontStyle.Bold);
        rtbFeedback.SelectionColor = titleColor;

        if (!string.IsNullOrEmpty(content))
        {
            rtbFeedback.AppendText("\n");
            var contentStart = rtbFeedback.TextLength;
            rtbFeedback.AppendText(content);
            rtbFeedback.Select(contentStart, content.Length);
            rtbFeedback.SelectionFont = new Font("Segoe UI", 10F);
            rtbFeedback.SelectionColor = Color.FromArgb(50, 50, 50);
        }

        rtbFeedback.DeselectAll();
    }

    // ───────────────────────────────────────────────────────────
    // Audio Cleanup
    // ───────────────────────────────────────────────────────────

    private void RenderQuestionText(string text)
    {
        rtbQuestion.Clear();

        var normalFont = new Font("Segoe UI", 12F);
        var codeFont = new Font("Cascadia Code", 11F);
        var normalColor = Color.FromArgb(30, 30, 30);
        var codeColor = Color.FromArgb(0, 100, 180);
        var codeBgColor = Color.FromArgb(240, 240, 240);

        var i = 0;
        while (i < text.Length)
        {
            var backtickPos = text.IndexOf('`', i);
            if (backtickPos < 0)
            {
                // No more backticks — append the rest as normal text
                AppendRtbText(rtbQuestion, text[i..], normalFont, normalColor);
                break;
            }

            // Append text before the backtick
            if (backtickPos > i)
                AppendRtbText(rtbQuestion, text[i..backtickPos], normalFont, normalColor);

            // Find the closing backtick
            var closePos = text.IndexOf('`', backtickPos + 1);
            if (closePos < 0)
            {
                // No closing backtick — append the rest as normal
                AppendRtbText(rtbQuestion, text[backtickPos..], normalFont, normalColor);
                break;
            }

            // Append code span
            var code = text[(backtickPos + 1)..closePos];
            AppendRtbText(rtbQuestion, code, codeFont, codeColor, codeBgColor);

            i = closePos + 1;
        }

        rtbQuestion.SelectionStart = 0;
        rtbQuestion.ScrollToCaret();
    }

    private static void AppendRtbText(RichTextBox rtb, string text, Font font, Color color, Color? bgColor = null)
    {
        var start = rtb.TextLength;
        rtb.AppendText(text);
        rtb.Select(start, text.Length);
        rtb.SelectionFont = font;
        rtb.SelectionColor = color;
        if (bgColor.HasValue)
            rtb.SelectionBackColor = bgColor.Value;
        rtb.DeselectAll();
    }

    private void CleanupRecording()
    {
        if (_currentRecordingPath != null && File.Exists(_currentRecordingPath))
        {
            try { File.Delete(_currentRecordingPath); } catch { }
        }
        _currentRecordingPath = null;
    }
}
