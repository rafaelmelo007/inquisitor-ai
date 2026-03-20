using InquisitorAI.UI.Controls;

namespace InquisitorAI.UI.Forms;

partial class InterviewForm
{
    private System.ComponentModel.IContainer components = null;
    private ProgressBar progressBar;
    private Label lblProgress;
    private Label lblCategory;
    private Label lblDifficulty;
    private RichTextBox rtbQuestion;
    private Panel pnlAnswer;
    private Label lblTranscript;
    private Label lblScore;
    private Label lblFeedback;
    private Panel pnlButtons;
    private Button btnListen;
    private Button btnRecord;
    private Button btnStop;
    private Button btnNext;
    private RecordingIndicatorControl recordingIndicator;
    private AiProcessingOverlayControl aiOverlay;

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
        progressBar = new ProgressBar();
        lblProgress = new Label();
        lblCategory = new Label();
        lblDifficulty = new Label();
        rtbQuestion = new RichTextBox();
        pnlAnswer = new Panel();
        lblTranscript = new Label();
        lblScore = new Label();
        lblFeedback = new Label();
        pnlButtons = new Panel();
        btnListen = new Button();
        btnRecord = new Button();
        btnStop = new Button();
        btnNext = new Button();
        recordingIndicator = new RecordingIndicatorControl();
        aiOverlay = new AiProcessingOverlayControl();
        pnlAnswer.SuspendLayout();
        pnlButtons.SuspendLayout();
        SuspendLayout();

        // progressBar
        progressBar.Dock = DockStyle.Top;
        progressBar.Height = 25;
        progressBar.Minimum = 0;

        // lblProgress
        lblProgress.Dock = DockStyle.Top;
        lblProgress.Height = 25;
        lblProgress.Font = new Font("Segoe UI", 10F);
        lblProgress.TextAlign = ContentAlignment.MiddleCenter;
        lblProgress.Text = "Question 0 of 0";

        // lblCategory
        lblCategory.Dock = DockStyle.Top;
        lblCategory.Height = 25;
        lblCategory.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblCategory.Text = "Category: ";
        lblCategory.Padding = new Padding(10, 0, 0, 0);

        // lblDifficulty
        lblDifficulty.Dock = DockStyle.Top;
        lblDifficulty.Height = 25;
        lblDifficulty.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblDifficulty.Text = "Difficulty: ";
        lblDifficulty.Padding = new Padding(10, 0, 0, 0);

        // rtbQuestion
        rtbQuestion.Dock = DockStyle.Fill;
        rtbQuestion.ReadOnly = true;
        rtbQuestion.Font = new Font("Segoe UI", 12F);
        rtbQuestion.BackColor = Color.White;
        rtbQuestion.BorderStyle = BorderStyle.FixedSingle;
        rtbQuestion.Padding = new Padding(10);

        // pnlAnswer
        pnlAnswer.Dock = DockStyle.Bottom;
        pnlAnswer.Height = 150;
        pnlAnswer.AutoScroll = true;
        pnlAnswer.Padding = new Padding(10);
        pnlAnswer.Controls.Add(lblFeedback);
        pnlAnswer.Controls.Add(lblScore);
        pnlAnswer.Controls.Add(lblTranscript);

        // lblTranscript
        lblTranscript.Dock = DockStyle.Top;
        lblTranscript.Height = 40;
        lblTranscript.Font = new Font("Segoe UI", 9F);
        lblTranscript.Text = "";

        // lblScore
        lblScore.Dock = DockStyle.Top;
        lblScore.Height = 25;
        lblScore.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblScore.Text = "";

        // lblFeedback
        lblFeedback.Dock = DockStyle.Top;
        lblFeedback.Height = 60;
        lblFeedback.Font = new Font("Segoe UI", 9F);
        lblFeedback.Text = "";

        // pnlButtons
        pnlButtons.Dock = DockStyle.Bottom;
        pnlButtons.Height = 50;
        pnlButtons.Padding = new Padding(10, 5, 10, 5);
        pnlButtons.Controls.Add(btnNext);
        pnlButtons.Controls.Add(btnStop);
        pnlButtons.Controls.Add(recordingIndicator);
        pnlButtons.Controls.Add(btnRecord);
        pnlButtons.Controls.Add(btnListen);

        // btnListen
        btnListen.Text = "Listen";
        btnListen.Dock = DockStyle.Left;
        btnListen.Width = 100;
        btnListen.Font = new Font("Segoe UI", 10F);
        btnListen.Cursor = Cursors.Hand;

        // btnRecord
        btnRecord.Text = "Record";
        btnRecord.Dock = DockStyle.Left;
        btnRecord.Width = 100;
        btnRecord.Font = new Font("Segoe UI", 10F);
        btnRecord.Cursor = Cursors.Hand;

        // recordingIndicator
        recordingIndicator.Dock = DockStyle.Left;
        recordingIndicator.Size = new Size(30, 24);
        recordingIndicator.Visible = false;

        // btnStop
        btnStop.Text = "Stop";
        btnStop.Dock = DockStyle.Left;
        btnStop.Width = 100;
        btnStop.Font = new Font("Segoe UI", 10F);
        btnStop.Enabled = false;
        btnStop.Cursor = Cursors.Hand;

        // btnNext
        btnNext.Text = "Next Question";
        btnNext.Dock = DockStyle.Right;
        btnNext.Width = 130;
        btnNext.Font = new Font("Segoe UI", 10F);
        btnNext.Enabled = false;
        btnNext.Cursor = Cursors.Hand;

        // aiOverlay
        aiOverlay.Dock = DockStyle.Fill;
        aiOverlay.Visible = false;

        // InterviewForm
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 600);
        Controls.Add(aiOverlay);
        Controls.Add(rtbQuestion);
        Controls.Add(lblDifficulty);
        Controls.Add(lblCategory);
        Controls.Add(lblProgress);
        Controls.Add(progressBar);
        Controls.Add(pnlAnswer);
        Controls.Add(pnlButtons);
        StartPosition = FormStartPosition.CenterParent;
        Text = "Interview";
        Name = "InterviewForm";
        pnlAnswer.ResumeLayout(false);
        pnlButtons.ResumeLayout(false);
        ResumeLayout(false);
    }
}
