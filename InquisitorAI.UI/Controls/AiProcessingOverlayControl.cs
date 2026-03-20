namespace InquisitorAI.UI.Controls;

public class AiProcessingOverlayControl : UserControl
{
    private readonly ProgressBar _progressBar;
    private readonly Label _messageLabel;

    public AiProcessingOverlayControl()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(150, 0, 0, 0);
        Visible = false;

        _progressBar = new ProgressBar
        {
            Style = ProgressBarStyle.Marquee,
            MarqueeAnimationSpeed = 30,
            Size = new Size(300, 30),
            Anchor = AnchorStyles.None
        };

        _messageLabel = new Label
        {
            Text = "AI is analyzing...",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 14f, FontStyle.Bold),
            AutoSize = true,
            Anchor = AnchorStyles.None,
            BackColor = Color.Transparent
        };

        Controls.Add(_progressBar);
        Controls.Add(_messageLabel);
    }

    public void ShowOverlay()
    {
        Visible = true;
        BringToFront();
        CenterControls();
    }

    public void HideOverlay()
    {
        Visible = false;
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        CenterControls();
    }

    private void CenterControls()
    {
        if (Width == 0 || Height == 0) return;

        _messageLabel.Location = new Point(
            (Width - _messageLabel.Width) / 2,
            Height / 2 - 40);

        _progressBar.Location = new Point(
            (Width - _progressBar.Width) / 2,
            Height / 2);
    }

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
            return cp;
        }
    }
}
