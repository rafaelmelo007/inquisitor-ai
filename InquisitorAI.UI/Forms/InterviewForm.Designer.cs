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
    private SplitContainer splitContainer;
    private RichTextBox rtbFeedback;
    private Panel pnlButtons;
    private Button btnListen;
    private Button btnRecord;
    private Button btnStop;
    private Button btnNext;
    private Button btnPrevious;
    private Button btnShowFeedback;
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
        splitContainer = new SplitContainer();
        rtbFeedback = new RichTextBox();
        pnlButtons = new Panel();
        btnListen = new Button();
        btnRecord = new Button();
        btnStop = new Button();
        btnNext = new Button();
        btnPrevious = new Button();
        btnShowFeedback = new Button();
        recordingIndicator = new RecordingIndicatorControl();
        aiOverlay = new AiProcessingOverlayControl();
        ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
        splitContainer.SuspendLayout();
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

        // splitContainer
        splitContainer.Dock = DockStyle.Fill;
        splitContainer.Orientation = Orientation.Horizontal;
        splitContainer.SplitterWidth = 6;

        // rtbQuestion (top panel)
        rtbQuestion.Dock = DockStyle.Fill;
        rtbQuestion.ReadOnly = true;
        rtbQuestion.Font = new Font("Segoe UI", 12F);
        rtbQuestion.BackColor = Color.White;
        rtbQuestion.BorderStyle = BorderStyle.FixedSingle;
        rtbQuestion.Padding = new Padding(10);
        splitContainer.Panel1.Controls.Add(rtbQuestion);

        // rtbFeedback (bottom panel)
        rtbFeedback.Dock = DockStyle.Fill;
        rtbFeedback.ReadOnly = true;
        rtbFeedback.Font = new Font("Segoe UI", 11F);
        rtbFeedback.BackColor = Color.FromArgb(250, 250, 250);
        rtbFeedback.BorderStyle = BorderStyle.None;
        rtbFeedback.ScrollBars = RichTextBoxScrollBars.Vertical;
        rtbFeedback.Text = "";

        splitContainer.Panel2.Controls.Add(rtbFeedback);

        // pnlButtons
        pnlButtons.Dock = DockStyle.Bottom;
        pnlButtons.Height = 50;
        pnlButtons.Padding = new Padding(10, 5, 10, 5);
        pnlButtons.Controls.Add(btnNext);
        pnlButtons.Controls.Add(btnShowFeedback);
        pnlButtons.Controls.Add(btnStop);
        pnlButtons.Controls.Add(recordingIndicator);
        pnlButtons.Controls.Add(btnRecord);
        pnlButtons.Controls.Add(btnListen);
        pnlButtons.Controls.Add(btnPrevious);

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

        // btnPrevious
        btnPrevious.Text = "Previous";
        btnPrevious.Dock = DockStyle.Left;
        btnPrevious.Width = 100;
        btnPrevious.Font = new Font("Segoe UI", 10F);
        btnPrevious.Enabled = false;
        btnPrevious.Cursor = Cursors.Hand;

        // btnNext
        btnNext.Text = "Next Question";
        btnNext.Dock = DockStyle.Right;
        btnNext.Width = 130;
        btnNext.Font = new Font("Segoe UI", 10F);
        btnNext.Enabled = false;
        btnNext.Cursor = Cursors.Hand;

        // btnShowFeedback
        btnShowFeedback.Text = "Show Feedback";
        btnShowFeedback.Dock = DockStyle.Right;
        btnShowFeedback.Width = 130;
        btnShowFeedback.Font = new Font("Segoe UI", 10F);
        btnShowFeedback.Cursor = Cursors.Hand;

        // aiOverlay
        aiOverlay.Dock = DockStyle.Fill;
        aiOverlay.Visible = false;

        // InterviewForm
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 750);
        Controls.Add(aiOverlay);
        Controls.Add(splitContainer);
        Controls.Add(lblDifficulty);
        Controls.Add(lblCategory);
        Controls.Add(lblProgress);
        Controls.Add(progressBar);
        Controls.Add(pnlButtons);
        StartPosition = FormStartPosition.CenterParent;
        Text = "Interview";
        Name = "InterviewForm";
        ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
        splitContainer.ResumeLayout(false);
        pnlButtons.ResumeLayout(false);
        ResumeLayout(false);

        Load += (_, _) =>
        {
            var available = splitContainer.Height - splitContainer.SplitterWidth;
            splitContainer.SplitterDistance = available / 2;
        };
    }
}
