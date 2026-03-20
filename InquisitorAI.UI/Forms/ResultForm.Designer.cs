namespace InquisitorAI.UI.Forms;

partial class ResultForm
{
    private System.ComponentModel.IContainer components = null;
    private Label lblScoreTitle;
    private Label lblScore;
    private Label lblClassification;
    private GroupBox grpStrengths;
    private TextBox txtStrengths;
    private GroupBox grpImprovements;
    private TextBox txtImprovements;
    private Button btnViewReport;
    private Button btnClose;

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
        lblScoreTitle = new Label();
        lblScore = new Label();
        lblClassification = new Label();
        grpStrengths = new GroupBox();
        txtStrengths = new TextBox();
        grpImprovements = new GroupBox();
        txtImprovements = new TextBox();
        btnViewReport = new Button();
        btnClose = new Button();
        grpStrengths.SuspendLayout();
        grpImprovements.SuspendLayout();
        SuspendLayout();

        // lblScoreTitle
        lblScoreTitle.Text = "Final Score";
        lblScoreTitle.Font = new Font("Segoe UI", 14F);
        lblScoreTitle.TextAlign = ContentAlignment.MiddleCenter;
        lblScoreTitle.Dock = DockStyle.Top;
        lblScoreTitle.Height = 35;

        // lblScore
        lblScore.Text = "0";
        lblScore.Font = new Font("Segoe UI", 36F, FontStyle.Bold);
        lblScore.TextAlign = ContentAlignment.MiddleCenter;
        lblScore.Dock = DockStyle.Top;
        lblScore.Height = 70;

        // lblClassification
        lblClassification.Text = "";
        lblClassification.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblClassification.TextAlign = ContentAlignment.MiddleCenter;
        lblClassification.Dock = DockStyle.Top;
        lblClassification.Height = 40;

        // grpStrengths
        grpStrengths.Text = "Strengths";
        grpStrengths.Dock = DockStyle.Top;
        grpStrengths.Height = 140;
        grpStrengths.Padding = new Padding(10);
        grpStrengths.Font = new Font("Segoe UI", 10F);
        grpStrengths.Controls.Add(txtStrengths);

        // txtStrengths
        txtStrengths.Dock = DockStyle.Fill;
        txtStrengths.Multiline = true;
        txtStrengths.ReadOnly = true;
        txtStrengths.ScrollBars = ScrollBars.Vertical;
        txtStrengths.Font = new Font("Segoe UI", 9F);

        // grpImprovements
        grpImprovements.Text = "Improvement Areas";
        grpImprovements.Dock = DockStyle.Top;
        grpImprovements.Height = 140;
        grpImprovements.Padding = new Padding(10);
        grpImprovements.Font = new Font("Segoe UI", 10F);
        grpImprovements.Controls.Add(txtImprovements);

        // txtImprovements
        txtImprovements.Dock = DockStyle.Fill;
        txtImprovements.Multiline = true;
        txtImprovements.ReadOnly = true;
        txtImprovements.ScrollBars = ScrollBars.Vertical;
        txtImprovements.Font = new Font("Segoe UI", 9F);

        // btnViewReport
        btnViewReport.Text = "View Report";
        btnViewReport.Size = new Size(150, 40);
        btnViewReport.Font = new Font("Segoe UI", 10F);
        btnViewReport.Location = new Point(120, 470);
        btnViewReport.Anchor = AnchorStyles.Bottom;
        btnViewReport.Cursor = Cursors.Hand;

        // btnClose
        btnClose.Text = "Close";
        btnClose.Size = new Size(150, 40);
        btnClose.Font = new Font("Segoe UI", 10F);
        btnClose.Location = new Point(300, 470);
        btnClose.Anchor = AnchorStyles.Bottom;
        btnClose.Cursor = Cursors.Hand;

        // ResultForm
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(550, 530);
        Controls.Add(btnClose);
        Controls.Add(btnViewReport);
        Controls.Add(grpImprovements);
        Controls.Add(grpStrengths);
        Controls.Add(lblClassification);
        Controls.Add(lblScore);
        Controls.Add(lblScoreTitle);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Interview Results";
        Name = "ResultForm";
        grpStrengths.ResumeLayout(false);
        grpStrengths.PerformLayout();
        grpImprovements.ResumeLayout(false);
        grpImprovements.PerformLayout();
        ResumeLayout(false);
    }
}
