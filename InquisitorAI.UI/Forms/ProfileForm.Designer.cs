namespace InquisitorAI.UI.Forms;

partial class ProfileForm
{
    private System.ComponentModel.IContainer components = null;
    private PictureBox picAvatar;
    private Label lblEmail;
    private Label lblEmailValue;
    private Label lblProvider;
    private Label lblProviderValue;
    private Label lblDisplayName;
    private TextBox txtDisplayName;
    private Button btnSave;
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
        picAvatar = new PictureBox();
        lblEmail = new Label();
        lblEmailValue = new Label();
        lblProvider = new Label();
        lblProviderValue = new Label();
        lblDisplayName = new Label();
        txtDisplayName = new TextBox();
        btnSave = new Button();
        btnClose = new Button();
        ((System.ComponentModel.ISupportInitialize)picAvatar).BeginInit();
        SuspendLayout();

        // picAvatar
        picAvatar.Size = new Size(100, 100);
        picAvatar.Location = new Point(150, 20);
        picAvatar.SizeMode = PictureBoxSizeMode.Zoom;
        picAvatar.BorderStyle = BorderStyle.FixedSingle;

        // lblEmail
        lblEmail.Text = "Email:";
        lblEmail.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblEmail.Location = new Point(30, 140);
        lblEmail.AutoSize = true;

        // lblEmailValue
        lblEmailValue.Text = "";
        lblEmailValue.Font = new Font("Segoe UI", 10F);
        lblEmailValue.Location = new Point(160, 140);
        lblEmailValue.AutoSize = true;

        // lblProvider
        lblProvider.Text = "Provider:";
        lblProvider.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblProvider.Location = new Point(30, 175);
        lblProvider.AutoSize = true;

        // lblProviderValue
        lblProviderValue.Text = "";
        lblProviderValue.Font = new Font("Segoe UI", 10F);
        lblProviderValue.Location = new Point(160, 175);
        lblProviderValue.AutoSize = true;

        // lblDisplayName
        lblDisplayName.Text = "Display Name:";
        lblDisplayName.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblDisplayName.Location = new Point(30, 215);
        lblDisplayName.AutoSize = true;

        // txtDisplayName
        txtDisplayName.Font = new Font("Segoe UI", 10F);
        txtDisplayName.Location = new Point(160, 212);
        txtDisplayName.Size = new Size(200, 25);

        // btnSave
        btnSave.Text = "Save";
        btnSave.Size = new Size(120, 40);
        btnSave.Font = new Font("Segoe UI", 10F);
        btnSave.Location = new Point(100, 270);
        btnSave.Cursor = Cursors.Hand;

        // btnClose
        btnClose.Text = "Close";
        btnClose.Size = new Size(120, 40);
        btnClose.Font = new Font("Segoe UI", 10F);
        btnClose.Location = new Point(240, 270);
        btnClose.Cursor = Cursors.Hand;

        // ProfileForm
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(420, 340);
        Controls.Add(picAvatar);
        Controls.Add(lblEmail);
        Controls.Add(lblEmailValue);
        Controls.Add(lblProvider);
        Controls.Add(lblProviderValue);
        Controls.Add(lblDisplayName);
        Controls.Add(txtDisplayName);
        Controls.Add(btnSave);
        Controls.Add(btnClose);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Profile";
        Name = "ProfileForm";
        ((System.ComponentModel.ISupportInitialize)picAvatar).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
}
