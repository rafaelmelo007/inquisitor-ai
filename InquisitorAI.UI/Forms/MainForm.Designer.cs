namespace InquisitorAI.UI.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;
    private Panel pnlToolbar;
    private Button btnUpload;
    private Button btnStartInterview;
    private Button btnHistory;
    private Button btnProfile;
    private Button btnLogout;
    private ListView lvQuestionnaires;
    private ColumnHeader colName;
    private ColumnHeader colCreatedBy;
    private ColumnHeader colPublic;
    private ColumnHeader colQuestionCount;

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
        btnUpload = new Button();
        btnStartInterview = new Button();
        btnHistory = new Button();
        btnProfile = new Button();
        btnLogout = new Button();
        lvQuestionnaires = new ListView();
        colName = new ColumnHeader();
        colCreatedBy = new ColumnHeader();
        colPublic = new ColumnHeader();
        colQuestionCount = new ColumnHeader();
        pnlToolbar.SuspendLayout();
        SuspendLayout();

        // pnlToolbar
        pnlToolbar.Dock = DockStyle.Top;
        pnlToolbar.Height = 50;
        pnlToolbar.Padding = new Padding(5);
        pnlToolbar.Controls.Add(btnLogout);
        pnlToolbar.Controls.Add(btnProfile);
        pnlToolbar.Controls.Add(btnHistory);
        pnlToolbar.Controls.Add(btnStartInterview);
        pnlToolbar.Controls.Add(btnUpload);

        // btnUpload
        btnUpload.Text = "Upload Questionnaire";
        btnUpload.Dock = DockStyle.Left;
        btnUpload.Width = 160;
        btnUpload.Font = new Font("Segoe UI", 9F);
        btnUpload.Cursor = Cursors.Hand;

        // btnStartInterview
        btnStartInterview.Text = "Start Interview";
        btnStartInterview.Dock = DockStyle.Left;
        btnStartInterview.Width = 130;
        btnStartInterview.Font = new Font("Segoe UI", 9F);
        btnStartInterview.Cursor = Cursors.Hand;

        // btnHistory
        btnHistory.Text = "History";
        btnHistory.Dock = DockStyle.Left;
        btnHistory.Width = 100;
        btnHistory.Font = new Font("Segoe UI", 9F);
        btnHistory.Cursor = Cursors.Hand;

        // btnProfile
        btnProfile.Text = "Profile";
        btnProfile.Dock = DockStyle.Left;
        btnProfile.Width = 100;
        btnProfile.Font = new Font("Segoe UI", 9F);
        btnProfile.Cursor = Cursors.Hand;

        // btnLogout
        btnLogout.Text = "Logout";
        btnLogout.Dock = DockStyle.Right;
        btnLogout.Width = 100;
        btnLogout.Font = new Font("Segoe UI", 9F);
        btnLogout.Cursor = Cursors.Hand;

        // lvQuestionnaires
        lvQuestionnaires.Dock = DockStyle.Fill;
        lvQuestionnaires.View = View.Details;
        lvQuestionnaires.FullRowSelect = true;
        lvQuestionnaires.GridLines = true;
        lvQuestionnaires.MultiSelect = false;
        lvQuestionnaires.Columns.AddRange(new ColumnHeader[] { colName, colCreatedBy, colPublic, colQuestionCount });

        // Column Headers
        colName.Text = "Name";
        colName.Width = 300;
        colCreatedBy.Text = "Created By";
        colCreatedBy.Width = 200;
        colPublic.Text = "Public";
        colPublic.Width = 80;
        colQuestionCount.Text = "Questions";
        colQuestionCount.Width = 100;

        // MainForm
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(900, 600);
        Controls.Add(lvQuestionnaires);
        Controls.Add(pnlToolbar);
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Inquisitor AI";
        Name = "MainForm";
        pnlToolbar.ResumeLayout(false);
        ResumeLayout(false);
    }
}
