namespace InquisitorAI.UI.Forms;

partial class HistoryForm
{
    private System.ComponentModel.IContainer components = null;
    private DataGridView dgvSessions;
    private Panel pnlButtons;
    private Button btnOpenReport;
    private Button btnDelete;
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
        dgvSessions = new DataGridView();
        pnlButtons = new Panel();
        btnOpenReport = new Button();
        btnDelete = new Button();
        btnClose = new Button();
        ((System.ComponentModel.ISupportInitialize)dgvSessions).BeginInit();
        pnlButtons.SuspendLayout();
        SuspendLayout();

        // dgvSessions
        dgvSessions.Dock = DockStyle.Fill;
        dgvSessions.AllowUserToAddRows = false;
        dgvSessions.AllowUserToDeleteRows = false;
        dgvSessions.ReadOnly = true;
        dgvSessions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvSessions.MultiSelect = false;
        dgvSessions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvSessions.RowHeadersVisible = false;
        dgvSessions.BackgroundColor = Color.White;

        // pnlButtons
        pnlButtons.Dock = DockStyle.Bottom;
        pnlButtons.Height = 50;
        pnlButtons.Padding = new Padding(10, 5, 10, 5);
        pnlButtons.Controls.Add(btnClose);
        pnlButtons.Controls.Add(btnDelete);
        pnlButtons.Controls.Add(btnOpenReport);

        // btnOpenReport
        btnOpenReport.Text = "Open Report";
        btnOpenReport.Dock = DockStyle.Left;
        btnOpenReport.Width = 120;
        btnOpenReport.Font = new Font("Segoe UI", 9F);
        btnOpenReport.Cursor = Cursors.Hand;

        // btnDelete
        btnDelete.Text = "Delete";
        btnDelete.Dock = DockStyle.Left;
        btnDelete.Width = 100;
        btnDelete.Font = new Font("Segoe UI", 9F);
        btnDelete.Cursor = Cursors.Hand;

        // btnClose
        btnClose.Text = "Close";
        btnClose.Dock = DockStyle.Right;
        btnClose.Width = 100;
        btnClose.Font = new Font("Segoe UI", 9F);
        btnClose.Cursor = Cursors.Hand;

        // HistoryForm
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 500);
        Controls.Add(dgvSessions);
        Controls.Add(pnlButtons);
        StartPosition = FormStartPosition.CenterParent;
        Text = "Interview History";
        Name = "HistoryForm";
        ((System.ComponentModel.ISupportInitialize)dgvSessions).EndInit();
        pnlButtons.ResumeLayout(false);
        ResumeLayout(false);
    }
}
