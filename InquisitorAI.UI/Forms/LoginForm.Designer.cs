namespace InquisitorAI.UI.Forms;

partial class LoginForm
{
    private System.ComponentModel.IContainer components = null;
    private Label lblTitle;
    private Button btnGoogle;
    private Button btnGitHub;
    private Button btnLinkedIn;

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
        lblTitle = new Label();
        btnGoogle = new Button();
        btnGitHub = new Button();
        btnLinkedIn = new Button();
        SuspendLayout();

        // lblTitle
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
        lblTitle.Text = "Inquisitor AI";
        lblTitle.Anchor = AnchorStyles.None;
        lblTitle.Location = new Point(130, 40);

        // btnGoogle
        btnGoogle.Size = new Size(250, 50);
        btnGoogle.Text = "Sign in with Google";
        btnGoogle.Font = new Font("Segoe UI", 12F);
        btnGoogle.Anchor = AnchorStyles.None;
        btnGoogle.Location = new Point(100, 130);
        btnGoogle.FlatStyle = FlatStyle.Flat;
        btnGoogle.BackColor = Color.White;
        btnGoogle.Cursor = Cursors.Hand;

        // btnGitHub
        btnGitHub.Size = new Size(250, 50);
        btnGitHub.Text = "Sign in with GitHub";
        btnGitHub.Font = new Font("Segoe UI", 12F);
        btnGitHub.Anchor = AnchorStyles.None;
        btnGitHub.Location = new Point(100, 200);
        btnGitHub.FlatStyle = FlatStyle.Flat;
        btnGitHub.BackColor = Color.White;
        btnGitHub.Cursor = Cursors.Hand;

        // btnLinkedIn
        btnLinkedIn.Size = new Size(250, 50);
        btnLinkedIn.Text = "Sign in with LinkedIn";
        btnLinkedIn.Font = new Font("Segoe UI", 12F);
        btnLinkedIn.Anchor = AnchorStyles.None;
        btnLinkedIn.Location = new Point(100, 270);
        btnLinkedIn.FlatStyle = FlatStyle.Flat;
        btnLinkedIn.BackColor = Color.White;
        btnLinkedIn.Cursor = Cursors.Hand;

        // LoginForm
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(450, 380);
        Controls.Add(lblTitle);
        Controls.Add(btnGoogle);
        Controls.Add(btnGitHub);
        Controls.Add(btnLinkedIn);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Inquisitor AI - Login";
        Name = "LoginForm";
        ResumeLayout(false);
        PerformLayout();
    }
}
