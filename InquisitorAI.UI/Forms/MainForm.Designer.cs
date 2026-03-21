using InquisitorAI.UI.Controls;

namespace InquisitorAI.UI.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    // Toolbar
    private Panel pnlToolbar;
    private Button btnImport;
    private Button btnSettings;
    private Button btnLogout;
    private Label lblToolbarTitle;

    // Layout
    private SplitContainer splitMain;
    private SplitContainer splitContent;

    // Left panel
    private Panel pnlLeft;
    private Label lblLeftTitle;
    private TreeView tvExplorer;

    // Center panel container
    private Panel pnlCenter;

    // Center: Welcome view
    private Panel pnlWelcome;
    private Label lblWelcomeTitle;
    private Label lblWelcomeSubtitle;

    // Center: Questionnaire view
    private Panel pnlQuestionnaireView;
    private Label lblQuestionnaireName;
    private Label lblQuestionnaireInfo;
    private Button btnStartSession;
    private Button btnDeleteQuestionnaire;

    // Center: Session overview
    private Panel pnlSessionOverview;
    private Label lblSessionTitle;
    private Label lblSessionInfo;
    private Label lblSessionScoreLabel;
    private Label lblSessionScoreValue;
    private Label lblSessionClassification;
    private Button btnResumeSession;
    private Button btnViewReport;
    private Button btnDeleteSession;
    private GroupBox grpStrengths;
    private TextBox txtStrengths;
    private GroupBox grpImprovements;
    private TextBox txtImprovements;

    // Center: Interview view
    private Panel pnlInterview;
    private ProgressBar progressBar;
    private Label lblProgress;
    private Panel pnlQuestionMeta;
    private Label lblCategory;
    private Label lblDifficulty;
    private SplitContainer splitInterview;
    private RichTextBox rtbQuestion;
    private RichTextBox rtbFeedback;
    private Panel pnlInterviewButtons;
    private Button btnListen;
    private Button btnRecord;
    private Button btnStop;
    private Button btnPrevious;
    private Button btnNext;
    private Button btnShowFeedback;
    private Button btnFinishEarly;
    private RecordingIndicatorControl recordingIndicator;
    private AiProcessingOverlayControl aiOverlay;
    private Label lblTimer;

    // Center: Settings view
    private Panel pnlSettingsView;
    private Label lblSettingsTitle;
    private Label lblSettingsEmail;
    private Label lblSettingsEmailValue;
    private Label lblSettingsName;
    private TextBox txtSettingsName;
    private Label lblSettingsLanguage;
    private ComboBox cboLanguage;
    private CheckBox chkSpeakQuestions;
    private Label lblThinkingTime;
    private NumericUpDown nudThinkingTime;
    private Label lblThinkingTimeUnit;
    private Label lblAnswerTime;
    private NumericUpDown nudAnswerTime;
    private Label lblAnswerTimeUnit;
    private Button btnSaveSettings;

    // Right panel
    private Panel pnlRight;
    private Label lblRightTitle;
    private ListView lvQuestionStatus;
    private ColumnHeader colQNum;
    private ColumnHeader colQStatus;
    private ColumnHeader colQScore;
    private Panel pnlScoreSummary;
    private Label lblAnsweredCount;
    private Label lblCurrentScore;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlToolbar = new Panel();
        btnImport = new Button();
        btnSettings = new Button();
        btnLogout = new Button();
        lblToolbarTitle = new Label();
        splitMain = new SplitContainer();
        splitContent = new SplitContainer();
        pnlLeft = new Panel();
        lblLeftTitle = new Label();
        tvExplorer = new TreeView();
        pnlCenter = new Panel();

        // Welcome
        pnlWelcome = new Panel();
        lblWelcomeTitle = new Label();
        lblWelcomeSubtitle = new Label();

        // Questionnaire view
        pnlQuestionnaireView = new Panel();
        lblQuestionnaireName = new Label();
        lblQuestionnaireInfo = new Label();
        btnStartSession = new Button();
        btnDeleteQuestionnaire = new Button();

        // Session overview
        pnlSessionOverview = new Panel();
        lblSessionTitle = new Label();
        lblSessionInfo = new Label();
        lblSessionScoreLabel = new Label();
        lblSessionScoreValue = new Label();
        lblSessionClassification = new Label();
        btnResumeSession = new Button();
        btnViewReport = new Button();
        btnDeleteSession = new Button();
        grpStrengths = new GroupBox();
        txtStrengths = new TextBox();
        grpImprovements = new GroupBox();
        txtImprovements = new TextBox();

        // Interview
        pnlInterview = new Panel();
        progressBar = new ProgressBar();
        lblProgress = new Label();
        pnlQuestionMeta = new Panel();
        lblCategory = new Label();
        lblDifficulty = new Label();
        splitInterview = new SplitContainer();
        rtbQuestion = new RichTextBox();
        rtbFeedback = new RichTextBox();
        pnlInterviewButtons = new Panel();
        btnListen = new Button();
        btnRecord = new Button();
        btnStop = new Button();
        btnPrevious = new Button();
        btnNext = new Button();
        btnShowFeedback = new Button();
        btnFinishEarly = new Button();
        recordingIndicator = new RecordingIndicatorControl();
        aiOverlay = new AiProcessingOverlayControl();
        lblTimer = new Label();

        // Settings
        pnlSettingsView = new Panel();
        lblSettingsTitle = new Label();
        lblSettingsEmail = new Label();
        lblSettingsEmailValue = new Label();
        lblSettingsName = new Label();
        txtSettingsName = new TextBox();
        lblSettingsLanguage = new Label();
        cboLanguage = new ComboBox();
        chkSpeakQuestions = new CheckBox();
        lblThinkingTime = new Label();
        nudThinkingTime = new NumericUpDown();
        lblThinkingTimeUnit = new Label();
        lblAnswerTime = new Label();
        nudAnswerTime = new NumericUpDown();
        lblAnswerTimeUnit = new Label();
        btnSaveSettings = new Button();

        // Right panel
        pnlRight = new Panel();
        lblRightTitle = new Label();
        lvQuestionStatus = new ListView();
        colQNum = new ColumnHeader();
        colQStatus = new ColumnHeader();
        colQScore = new ColumnHeader();
        pnlScoreSummary = new Panel();
        lblAnsweredCount = new Label();
        lblCurrentScore = new Label();

        // Suspend layouts
        pnlToolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
        splitMain.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContent).BeginInit();
        splitContent.SuspendLayout();
        pnlLeft.SuspendLayout();
        pnlCenter.SuspendLayout();
        pnlWelcome.SuspendLayout();
        pnlQuestionnaireView.SuspendLayout();
        pnlSessionOverview.SuspendLayout();
        pnlInterview.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitInterview).BeginInit();
        splitInterview.SuspendLayout();
        pnlInterviewButtons.SuspendLayout();
        pnlQuestionMeta.SuspendLayout();
        pnlSettingsView.SuspendLayout();
        pnlRight.SuspendLayout();
        pnlScoreSummary.SuspendLayout();
        grpStrengths.SuspendLayout();
        grpImprovements.SuspendLayout();
        SuspendLayout();

        // ─── Toolbar ───────────────────────────────────────────
        pnlToolbar.Dock = DockStyle.Top;
        pnlToolbar.Height = 45;
        pnlToolbar.Padding = new Padding(8, 6, 8, 6);
        pnlToolbar.BackColor = Color.FromArgb(45, 45, 48);
        pnlToolbar.Controls.Add(btnLogout);
        pnlToolbar.Controls.Add(btnSettings);
        pnlToolbar.Controls.Add(btnImport);
        pnlToolbar.Controls.Add(lblToolbarTitle);

        // lblToolbarTitle
        lblToolbarTitle.Text = "Inquisitor AI";
        lblToolbarTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblToolbarTitle.ForeColor = Color.White;
        lblToolbarTitle.Dock = DockStyle.Left;
        lblToolbarTitle.AutoSize = true;
        lblToolbarTitle.Padding = new Padding(4, 4, 20, 0);

        // btnImport
        btnImport.Text = "Import Questionnaire";
        btnImport.Dock = DockStyle.Left;
        btnImport.Width = 155;
        btnImport.Font = new Font("Segoe UI", 9F);
        btnImport.FlatStyle = FlatStyle.Flat;
        btnImport.ForeColor = Color.White;
        btnImport.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 85);
        btnImport.Cursor = Cursors.Hand;

        // btnSettings
        btnSettings.Text = "Settings";
        btnSettings.Dock = DockStyle.Right;
        btnSettings.Width = 90;
        btnSettings.Font = new Font("Segoe UI", 9F);
        btnSettings.FlatStyle = FlatStyle.Flat;
        btnSettings.ForeColor = Color.White;
        btnSettings.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 85);
        btnSettings.Cursor = Cursors.Hand;

        // btnLogout
        btnLogout.Text = "Logout";
        btnLogout.Dock = DockStyle.Right;
        btnLogout.Width = 80;
        btnLogout.Font = new Font("Segoe UI", 9F);
        btnLogout.FlatStyle = FlatStyle.Flat;
        btnLogout.ForeColor = Color.White;
        btnLogout.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 85);
        btnLogout.Cursor = Cursors.Hand;

        // ─── Split Main (Left | Center+Right) ──────────────────
        splitMain.Dock = DockStyle.Fill;
        splitMain.SplitterWidth = 4;
        splitMain.FixedPanel = FixedPanel.Panel1;
        splitMain.SplitterDistance = 350;
        splitMain.Panel1MinSize = 300;

        // ─── Left Panel ────────────────────────────────────────
        pnlLeft.Dock = DockStyle.Fill;
        pnlLeft.Padding = new Padding(0);
        pnlLeft.Controls.Add(tvExplorer);
        pnlLeft.Controls.Add(lblLeftTitle);

        lblLeftTitle.Text = "  Questionnaires";
        lblLeftTitle.Dock = DockStyle.Top;
        lblLeftTitle.Height = 30;
        lblLeftTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblLeftTitle.BackColor = Color.FromArgb(230, 230, 234);
        lblLeftTitle.TextAlign = ContentAlignment.MiddleLeft;

        tvExplorer.Dock = DockStyle.Fill;
        tvExplorer.Font = new Font("Segoe UI", 10F);
        tvExplorer.ItemHeight = 26;
        tvExplorer.ShowPlusMinus = true;
        tvExplorer.ShowLines = true;
        tvExplorer.HideSelection = false;
        tvExplorer.BorderStyle = BorderStyle.None;

        splitMain.Panel1.Controls.Add(pnlLeft);

        // ─── Split Content (Center | Right) ─────────────────────
        splitContent.Dock = DockStyle.Fill;
        splitContent.SplitterWidth = 4;
        splitContent.FixedPanel = FixedPanel.Panel2;
        splitContent.SplitterDistance = 500;
        splitContent.Panel2Collapsed = true;

        splitMain.Panel2.Controls.Add(splitContent);

        // ─── Center Panel ──────────────────────────────────────
        pnlCenter.Dock = DockStyle.Fill;
        pnlCenter.Padding = new Padding(15);

        pnlCenter.Controls.Add(pnlWelcome);
        pnlCenter.Controls.Add(pnlQuestionnaireView);
        pnlCenter.Controls.Add(pnlSessionOverview);
        pnlCenter.Controls.Add(pnlInterview);
        pnlCenter.Controls.Add(pnlSettingsView);
        pnlCenter.Controls.Add(aiOverlay);

        splitContent.Panel1.Controls.Add(pnlCenter);

        // ─── Welcome View ──────────────────────────────────────
        pnlWelcome.Dock = DockStyle.Fill;
        pnlWelcome.Visible = true;

        lblWelcomeTitle.Text = "Welcome to Inquisitor AI";
        lblWelcomeTitle.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
        lblWelcomeTitle.ForeColor = Color.FromArgb(45, 45, 48);
        lblWelcomeTitle.Dock = DockStyle.Top;
        lblWelcomeTitle.Height = 60;
        lblWelcomeTitle.TextAlign = ContentAlignment.MiddleCenter;
        lblWelcomeTitle.Padding = new Padding(0, 80, 0, 0);

        lblWelcomeSubtitle.Text = "Select a questionnaire or session from the left panel to get started.\nUse 'Import Questionnaire' to add a new questionnaire.";
        lblWelcomeSubtitle.Font = new Font("Segoe UI", 11F);
        lblWelcomeSubtitle.ForeColor = Color.Gray;
        lblWelcomeSubtitle.Dock = DockStyle.Top;
        lblWelcomeSubtitle.Height = 60;
        lblWelcomeSubtitle.TextAlign = ContentAlignment.TopCenter;

        pnlWelcome.Controls.Add(lblWelcomeSubtitle);
        pnlWelcome.Controls.Add(lblWelcomeTitle);

        // ─── Questionnaire View ────────────────────────────────
        pnlQuestionnaireView.Dock = DockStyle.Fill;
        pnlQuestionnaireView.Visible = false;

        lblQuestionnaireName.Text = "";
        lblQuestionnaireName.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblQuestionnaireName.Dock = DockStyle.Top;
        lblQuestionnaireName.Height = 50;
        lblQuestionnaireName.Padding = new Padding(0, 10, 0, 0);

        lblQuestionnaireInfo.Text = "";
        lblQuestionnaireInfo.Font = new Font("Segoe UI", 10F);
        lblQuestionnaireInfo.ForeColor = Color.Gray;
        lblQuestionnaireInfo.Dock = DockStyle.Top;
        lblQuestionnaireInfo.Height = 30;

        btnStartSession.Text = "Start New Session";
        btnStartSession.Size = new Size(180, 45);
        btnStartSession.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnStartSession.BackColor = Color.FromArgb(0, 122, 204);
        btnStartSession.ForeColor = Color.White;
        btnStartSession.FlatStyle = FlatStyle.Flat;
        btnStartSession.FlatAppearance.BorderSize = 0;
        btnStartSession.Location = new Point(0, 100);
        btnStartSession.Cursor = Cursors.Hand;

        btnDeleteQuestionnaire.Text = "Delete Questionnaire";
        btnDeleteQuestionnaire.Size = new Size(170, 35);
        btnDeleteQuestionnaire.Font = new Font("Segoe UI", 9F);
        btnDeleteQuestionnaire.ForeColor = Color.FromArgb(200, 50, 50);
        btnDeleteQuestionnaire.FlatStyle = FlatStyle.Flat;
        btnDeleteQuestionnaire.FlatAppearance.BorderColor = Color.FromArgb(200, 50, 50);
        btnDeleteQuestionnaire.Location = new Point(0, 160);
        btnDeleteQuestionnaire.Cursor = Cursors.Hand;

        pnlQuestionnaireView.Controls.Add(btnDeleteQuestionnaire);
        pnlQuestionnaireView.Controls.Add(btnStartSession);
        pnlQuestionnaireView.Controls.Add(lblQuestionnaireInfo);
        pnlQuestionnaireView.Controls.Add(lblQuestionnaireName);

        // ─── Session Overview ──────────────────────────────────
        pnlSessionOverview.Dock = DockStyle.Fill;
        pnlSessionOverview.Visible = false;
        pnlSessionOverview.AutoScroll = true;

        lblSessionTitle.Text = "Session Overview";
        lblSessionTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblSessionTitle.Dock = DockStyle.Top;
        lblSessionTitle.Height = 45;
        lblSessionTitle.Padding = new Padding(0, 8, 0, 0);

        lblSessionInfo.Text = "";
        lblSessionInfo.Font = new Font("Segoe UI", 10F);
        lblSessionInfo.ForeColor = Color.Gray;
        lblSessionInfo.Dock = DockStyle.Top;
        lblSessionInfo.Height = 25;

        lblSessionScoreLabel.Text = "Score";
        lblSessionScoreLabel.Font = new Font("Segoe UI", 12F);
        lblSessionScoreLabel.Dock = DockStyle.Top;
        lblSessionScoreLabel.Height = 30;
        lblSessionScoreLabel.TextAlign = ContentAlignment.BottomCenter;

        lblSessionScoreValue.Text = "—";
        lblSessionScoreValue.Font = new Font("Segoe UI", 40F, FontStyle.Bold);
        lblSessionScoreValue.Dock = DockStyle.Top;
        lblSessionScoreValue.Height = 65;
        lblSessionScoreValue.TextAlign = ContentAlignment.MiddleCenter;

        lblSessionClassification.Text = "";
        lblSessionClassification.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblSessionClassification.Dock = DockStyle.Top;
        lblSessionClassification.Height = 35;
        lblSessionClassification.TextAlign = ContentAlignment.TopCenter;

        btnResumeSession.Text = "Resume Session";
        btnResumeSession.Size = new Size(160, 40);
        btnResumeSession.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnResumeSession.BackColor = Color.FromArgb(0, 122, 204);
        btnResumeSession.ForeColor = Color.White;
        btnResumeSession.FlatStyle = FlatStyle.Flat;
        btnResumeSession.FlatAppearance.BorderSize = 0;
        btnResumeSession.Dock = DockStyle.Top;
        btnResumeSession.Cursor = Cursors.Hand;
        btnResumeSession.Visible = false;

        btnViewReport.Text = "View Report";
        btnViewReport.Size = new Size(140, 35);
        btnViewReport.Font = new Font("Segoe UI", 10F);
        btnViewReport.Dock = DockStyle.Top;
        btnViewReport.Cursor = Cursors.Hand;
        btnViewReport.Visible = false;

        btnDeleteSession.Text = "Delete Session";
        btnDeleteSession.Size = new Size(140, 35);
        btnDeleteSession.Font = new Font("Segoe UI", 9F);
        btnDeleteSession.ForeColor = Color.FromArgb(200, 50, 50);
        btnDeleteSession.FlatStyle = FlatStyle.Flat;
        btnDeleteSession.FlatAppearance.BorderColor = Color.FromArgb(200, 50, 50);
        btnDeleteSession.Dock = DockStyle.Top;
        btnDeleteSession.Cursor = Cursors.Hand;

        // grpStrengths
        grpStrengths.Text = "Strengths";
        grpStrengths.Dock = DockStyle.Top;
        grpStrengths.Height = 120;
        grpStrengths.Padding = new Padding(8);
        grpStrengths.Font = new Font("Segoe UI", 10F);
        grpStrengths.Visible = false;
        txtStrengths.Dock = DockStyle.Fill;
        txtStrengths.Multiline = true;
        txtStrengths.ReadOnly = true;
        txtStrengths.ScrollBars = ScrollBars.Vertical;
        txtStrengths.Font = new Font("Segoe UI", 9F);
        txtStrengths.BorderStyle = BorderStyle.None;
        grpStrengths.Controls.Add(txtStrengths);

        // grpImprovements
        grpImprovements.Text = "Improvement Areas";
        grpImprovements.Dock = DockStyle.Top;
        grpImprovements.Height = 120;
        grpImprovements.Padding = new Padding(8);
        grpImprovements.Font = new Font("Segoe UI", 10F);
        grpImprovements.Visible = false;
        txtImprovements.Dock = DockStyle.Fill;
        txtImprovements.Multiline = true;
        txtImprovements.ReadOnly = true;
        txtImprovements.ScrollBars = ScrollBars.Vertical;
        txtImprovements.Font = new Font("Segoe UI", 9F);
        txtImprovements.BorderStyle = BorderStyle.None;
        grpImprovements.Controls.Add(txtImprovements);

        // Session overview - add controls bottom to top (Dock=Top stacks top-down)
        pnlSessionOverview.Controls.Add(grpImprovements);
        pnlSessionOverview.Controls.Add(grpStrengths);
        pnlSessionOverview.Controls.Add(btnDeleteSession);
        pnlSessionOverview.Controls.Add(btnViewReport);
        pnlSessionOverview.Controls.Add(btnResumeSession);
        pnlSessionOverview.Controls.Add(lblSessionClassification);
        pnlSessionOverview.Controls.Add(lblSessionScoreValue);
        pnlSessionOverview.Controls.Add(lblSessionScoreLabel);
        pnlSessionOverview.Controls.Add(lblSessionInfo);
        pnlSessionOverview.Controls.Add(lblSessionTitle);

        // ─── Interview View ────────────────────────────────────
        pnlInterview.Dock = DockStyle.Fill;
        pnlInterview.Visible = false;

        // progressBar
        progressBar.Dock = DockStyle.Top;
        progressBar.Height = 22;
        progressBar.Minimum = 0;

        // lblProgress
        lblProgress.Dock = DockStyle.Top;
        lblProgress.Height = 22;
        lblProgress.Font = new Font("Segoe UI", 9F);
        lblProgress.TextAlign = ContentAlignment.MiddleCenter;
        lblProgress.Text = "Question 0 of 0";

        // pnlQuestionMeta
        pnlQuestionMeta.Dock = DockStyle.Top;
        pnlQuestionMeta.Height = 22;
        lblCategory.Text = "Category: ";
        lblCategory.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblCategory.Dock = DockStyle.Left;
        lblCategory.AutoSize = true;
        lblCategory.Padding = new Padding(5, 3, 15, 0);
        lblDifficulty.Text = "Difficulty: ";
        lblDifficulty.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblDifficulty.Dock = DockStyle.Left;
        lblDifficulty.AutoSize = true;
        lblDifficulty.Padding = new Padding(0, 3, 0, 0);
        pnlQuestionMeta.Controls.Add(lblDifficulty);
        pnlQuestionMeta.Controls.Add(lblCategory);

        // splitInterview
        splitInterview.Dock = DockStyle.Fill;
        splitInterview.Orientation = Orientation.Horizontal;
        splitInterview.SplitterWidth = 5;

        // rtbQuestion
        rtbQuestion.Dock = DockStyle.Fill;
        rtbQuestion.ReadOnly = true;
        rtbQuestion.Font = new Font("Segoe UI", 12F);
        rtbQuestion.BackColor = Color.White;
        rtbQuestion.BorderStyle = BorderStyle.None;
        rtbQuestion.Padding = new Padding(8);
        rtbQuestion.ScrollBars = RichTextBoxScrollBars.Vertical;
        splitInterview.Panel1.Controls.Add(rtbQuestion);

        // rtbFeedback
        rtbFeedback.Dock = DockStyle.Fill;
        rtbFeedback.ReadOnly = true;
        rtbFeedback.Font = new Font("Segoe UI", 11F);
        rtbFeedback.BackColor = Color.FromArgb(250, 250, 250);
        rtbFeedback.BorderStyle = BorderStyle.None;
        rtbFeedback.ScrollBars = RichTextBoxScrollBars.Vertical;
        rtbFeedback.Text = "";
        splitInterview.Panel2.Controls.Add(rtbFeedback);

        // lblTimer
        lblTimer.Text = "";
        lblTimer.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblTimer.ForeColor = Color.FromArgb(200, 50, 50);
        lblTimer.Dock = DockStyle.Right;
        lblTimer.Width = 80;
        lblTimer.TextAlign = ContentAlignment.MiddleCenter;
        lblTimer.Visible = false;

        // pnlInterviewButtons
        pnlInterviewButtons.Dock = DockStyle.Bottom;
        pnlInterviewButtons.Height = 45;
        pnlInterviewButtons.Padding = new Padding(5, 4, 5, 4);
        pnlInterviewButtons.Controls.Add(lblTimer);
        pnlInterviewButtons.Controls.Add(btnFinishEarly);
        pnlInterviewButtons.Controls.Add(btnNext);
        pnlInterviewButtons.Controls.Add(btnShowFeedback);
        pnlInterviewButtons.Controls.Add(btnStop);
        pnlInterviewButtons.Controls.Add(recordingIndicator);
        pnlInterviewButtons.Controls.Add(btnRecord);
        pnlInterviewButtons.Controls.Add(btnListen);
        pnlInterviewButtons.Controls.Add(btnPrevious);

        // btnPrevious
        btnPrevious.Text = "Previous";
        btnPrevious.Dock = DockStyle.Left;
        btnPrevious.Width = 85;
        btnPrevious.Font = new Font("Segoe UI", 9F);
        btnPrevious.Enabled = false;
        btnPrevious.Cursor = Cursors.Hand;

        // btnListen
        btnListen.Text = "Listen";
        btnListen.Dock = DockStyle.Left;
        btnListen.Width = 80;
        btnListen.Font = new Font("Segoe UI", 9F);
        btnListen.Cursor = Cursors.Hand;

        // btnRecord
        btnRecord.Text = "Record";
        btnRecord.Dock = DockStyle.Left;
        btnRecord.Width = 80;
        btnRecord.Font = new Font("Segoe UI", 9F);
        btnRecord.Cursor = Cursors.Hand;

        // recordingIndicator
        recordingIndicator.Dock = DockStyle.Left;
        recordingIndicator.Size = new Size(28, 24);
        recordingIndicator.Visible = false;

        // btnStop
        btnStop.Text = "Stop";
        btnStop.Dock = DockStyle.Left;
        btnStop.Width = 70;
        btnStop.Font = new Font("Segoe UI", 9F);
        btnStop.Enabled = false;
        btnStop.Cursor = Cursors.Hand;

        // btnShowFeedback
        btnShowFeedback.Text = "Show Answer";
        btnShowFeedback.Dock = DockStyle.Right;
        btnShowFeedback.Width = 110;
        btnShowFeedback.Font = new Font("Segoe UI", 9F);
        btnShowFeedback.Cursor = Cursors.Hand;

        // btnNext
        btnNext.Text = "Next";
        btnNext.Dock = DockStyle.Right;
        btnNext.Width = 80;
        btnNext.Font = new Font("Segoe UI", 9F);
        btnNext.Enabled = false;
        btnNext.Cursor = Cursors.Hand;

        // btnFinishEarly
        btnFinishEarly.Text = "Finish";
        btnFinishEarly.Dock = DockStyle.Right;
        btnFinishEarly.Width = 75;
        btnFinishEarly.Font = new Font("Segoe UI", 9F);
        btnFinishEarly.ForeColor = Color.FromArgb(180, 80, 0);
        btnFinishEarly.Cursor = Cursors.Hand;

        // aiOverlay
        aiOverlay.Dock = DockStyle.Fill;
        aiOverlay.Visible = false;

        // Build interview panel
        pnlInterview.Controls.Add(splitInterview);
        pnlInterview.Controls.Add(pnlQuestionMeta);
        pnlInterview.Controls.Add(lblProgress);
        pnlInterview.Controls.Add(progressBar);
        pnlInterview.Controls.Add(pnlInterviewButtons);

        // ─── Settings View ─────────────────────────────────────
        pnlSettingsView.Dock = DockStyle.Fill;
        pnlSettingsView.Visible = false;

        lblSettingsTitle.Text = "User Settings";
        lblSettingsTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblSettingsTitle.Dock = DockStyle.Top;
        lblSettingsTitle.Height = 50;
        lblSettingsTitle.Padding = new Padding(0, 10, 0, 0);

        lblSettingsEmail.Text = "Email:";
        lblSettingsEmail.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblSettingsEmail.Location = new Point(0, 70);
        lblSettingsEmail.AutoSize = true;

        lblSettingsEmailValue.Text = "";
        lblSettingsEmailValue.Font = new Font("Segoe UI", 10F);
        lblSettingsEmailValue.Location = new Point(140, 70);
        lblSettingsEmailValue.AutoSize = true;

        lblSettingsName.Text = "Display Name:";
        lblSettingsName.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblSettingsName.Location = new Point(0, 105);
        lblSettingsName.AutoSize = true;

        txtSettingsName.Font = new Font("Segoe UI", 10F);
        txtSettingsName.Location = new Point(140, 102);
        txtSettingsName.Size = new Size(250, 25);

        lblSettingsLanguage.Text = "Language:";
        lblSettingsLanguage.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblSettingsLanguage.Location = new Point(0, 145);
        lblSettingsLanguage.AutoSize = true;

        cboLanguage.Font = new Font("Segoe UI", 10F);
        cboLanguage.Location = new Point(140, 142);
        cboLanguage.Size = new Size(200, 25);
        cboLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
        cboLanguage.Items.AddRange(new object[] { "English", "Español", "Português" });

        // chkSpeakQuestions
        chkSpeakQuestions.Text = "Speak questions aloud";
        chkSpeakQuestions.Font = new Font("Segoe UI", 10F);
        chkSpeakQuestions.Location = new Point(0, 185);
        chkSpeakQuestions.AutoSize = true;
        chkSpeakQuestions.Checked = false;

        // lblThinkingTime
        lblThinkingTime.Text = "Thinking time:";
        lblThinkingTime.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblThinkingTime.Location = new Point(0, 225);
        lblThinkingTime.AutoSize = true;

        // nudThinkingTime
        nudThinkingTime.Font = new Font("Segoe UI", 10F);
        nudThinkingTime.Location = new Point(140, 222);
        nudThinkingTime.Size = new Size(80, 25);
        nudThinkingTime.Minimum = 0;
        nudThinkingTime.Maximum = 600;
        nudThinkingTime.Value = 0;
        nudThinkingTime.Increment = 5;

        // lblThinkingTimeUnit
        lblThinkingTimeUnit.Text = "seconds (0 = no limit)";
        lblThinkingTimeUnit.Font = new Font("Segoe UI", 9F);
        lblThinkingTimeUnit.ForeColor = Color.Gray;
        lblThinkingTimeUnit.Location = new Point(228, 226);
        lblThinkingTimeUnit.AutoSize = true;

        // lblAnswerTime
        lblAnswerTime.Text = "Answer time:";
        lblAnswerTime.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblAnswerTime.Location = new Point(0, 260);
        lblAnswerTime.AutoSize = true;

        // nudAnswerTime
        nudAnswerTime.Font = new Font("Segoe UI", 10F);
        nudAnswerTime.Location = new Point(140, 257);
        nudAnswerTime.Size = new Size(80, 25);
        nudAnswerTime.Minimum = 0;
        nudAnswerTime.Maximum = 600;
        nudAnswerTime.Value = 0;
        nudAnswerTime.Increment = 10;

        // lblAnswerTimeUnit
        lblAnswerTimeUnit.Text = "seconds (0 = no limit)";
        lblAnswerTimeUnit.Font = new Font("Segoe UI", 9F);
        lblAnswerTimeUnit.ForeColor = Color.Gray;
        lblAnswerTimeUnit.Location = new Point(228, 261);
        lblAnswerTimeUnit.AutoSize = true;

        btnSaveSettings.Text = "Save Settings";
        btnSaveSettings.Size = new Size(140, 40);
        btnSaveSettings.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnSaveSettings.BackColor = Color.FromArgb(0, 122, 204);
        btnSaveSettings.ForeColor = Color.White;
        btnSaveSettings.FlatStyle = FlatStyle.Flat;
        btnSaveSettings.FlatAppearance.BorderSize = 0;
        btnSaveSettings.Location = new Point(0, 305);
        btnSaveSettings.Cursor = Cursors.Hand;

        pnlSettingsView.Controls.Add(btnSaveSettings);
        pnlSettingsView.Controls.Add(lblAnswerTimeUnit);
        pnlSettingsView.Controls.Add(nudAnswerTime);
        pnlSettingsView.Controls.Add(lblAnswerTime);
        pnlSettingsView.Controls.Add(lblThinkingTimeUnit);
        pnlSettingsView.Controls.Add(nudThinkingTime);
        pnlSettingsView.Controls.Add(lblThinkingTime);
        pnlSettingsView.Controls.Add(chkSpeakQuestions);
        pnlSettingsView.Controls.Add(cboLanguage);
        pnlSettingsView.Controls.Add(lblSettingsLanguage);
        pnlSettingsView.Controls.Add(txtSettingsName);
        pnlSettingsView.Controls.Add(lblSettingsName);
        pnlSettingsView.Controls.Add(lblSettingsEmailValue);
        pnlSettingsView.Controls.Add(lblSettingsEmail);
        pnlSettingsView.Controls.Add(lblSettingsTitle);

        // ─── Right Panel ───────────────────────────────────────
        pnlRight.Dock = DockStyle.Fill;
        pnlRight.Padding = new Padding(5);

        lblRightTitle.Text = "  Session Progress";
        lblRightTitle.Dock = DockStyle.Top;
        lblRightTitle.Height = 30;
        lblRightTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblRightTitle.BackColor = Color.FromArgb(230, 230, 234);
        lblRightTitle.TextAlign = ContentAlignment.MiddleLeft;

        lvQuestionStatus.Dock = DockStyle.Fill;
        lvQuestionStatus.View = View.Details;
        lvQuestionStatus.FullRowSelect = true;
        lvQuestionStatus.GridLines = true;
        lvQuestionStatus.MultiSelect = false;
        lvQuestionStatus.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        lvQuestionStatus.Font = new Font("Segoe UI", 9F);
        lvQuestionStatus.BorderStyle = BorderStyle.None;
        lvQuestionStatus.Columns.AddRange(new ColumnHeader[] { colQNum, colQStatus, colQScore });

        colQNum.Text = "#";
        colQNum.Width = 35;
        colQStatus.Text = "Status";
        colQStatus.Width = 120;
        colQScore.Text = "Score";
        colQScore.Width = 65;

        pnlScoreSummary.Dock = DockStyle.Bottom;
        pnlScoreSummary.Height = 55;
        pnlScoreSummary.Padding = new Padding(5, 5, 5, 5);
        pnlScoreSummary.BackColor = Color.FromArgb(240, 240, 244);

        lblAnsweredCount.Text = "Answered: 0 / 0";
        lblAnsweredCount.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblAnsweredCount.Dock = DockStyle.Top;
        lblAnsweredCount.Height = 22;

        lblCurrentScore.Text = "Score: —";
        lblCurrentScore.Font = new Font("Segoe UI", 10F);
        lblCurrentScore.Dock = DockStyle.Top;
        lblCurrentScore.Height = 22;

        pnlScoreSummary.Controls.Add(lblCurrentScore);
        pnlScoreSummary.Controls.Add(lblAnsweredCount);

        pnlRight.Controls.Add(lvQuestionStatus);
        pnlRight.Controls.Add(pnlScoreSummary);
        pnlRight.Controls.Add(lblRightTitle);

        splitContent.Panel2.Controls.Add(pnlRight);

        // ─── Main Form ─────────────────────────────────────────
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1400, 800);
        MinimumSize = new Size(1100, 650);
        Controls.Add(splitMain);
        Controls.Add(pnlToolbar);
        WindowState = FormWindowState.Maximized;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Inquisitor AI";
        Name = "MainForm";

        // Resume layouts
        pnlScoreSummary.ResumeLayout(false);
        grpStrengths.ResumeLayout(false);
        grpStrengths.PerformLayout();
        grpImprovements.ResumeLayout(false);
        grpImprovements.PerformLayout();
        pnlRight.ResumeLayout(false);
        pnlSettingsView.ResumeLayout(false);
        pnlSettingsView.PerformLayout();
        pnlInterviewButtons.ResumeLayout(false);
        pnlQuestionMeta.ResumeLayout(false);
        pnlQuestionMeta.PerformLayout();
        splitInterview.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitInterview).EndInit();
        pnlInterview.ResumeLayout(false);
        pnlSessionOverview.ResumeLayout(false);
        pnlQuestionnaireView.ResumeLayout(false);
        pnlWelcome.ResumeLayout(false);
        pnlCenter.ResumeLayout(false);
        pnlLeft.ResumeLayout(false);
        splitContent.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContent).EndInit();
        splitMain.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
        pnlToolbar.ResumeLayout(false);
        pnlToolbar.PerformLayout();
        ResumeLayout(false);

        // Set initial splitter distance for interview split
        splitInterview.SplitterDistance = 200;
    }
}
