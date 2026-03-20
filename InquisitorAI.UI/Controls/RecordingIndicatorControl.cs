namespace InquisitorAI.UI.Controls;

public class RecordingIndicatorControl : UserControl
{
    private readonly System.Windows.Forms.Timer _pulseTimer;
    private float _opacity = 1.0f;
    private bool _fading;

    public RecordingIndicatorControl()
    {
        DoubleBuffered = true;
        Size = new Size(24, 24);
        BackColor = Color.Transparent;

        _pulseTimer = new System.Windows.Forms.Timer
        {
            Interval = 500
        };
        _pulseTimer.Tick += OnPulseTimerTick;
    }

    public void StartPulsing()
    {
        _opacity = 1.0f;
        _fading = true;
        Visible = true;
        _pulseTimer.Start();
    }

    public void StopPulsing()
    {
        _pulseTimer.Stop();
        Visible = false;
    }

    private void OnPulseTimerTick(object? sender, EventArgs e)
    {
        _opacity = _fading ? 0.3f : 1.0f;
        _fading = !_fading;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        var alpha = (int)(_opacity * 255);
        using var brush = new SolidBrush(Color.FromArgb(alpha, Color.Red));
        var padding = 2;
        e.Graphics.FillEllipse(brush, padding, padding, Width - padding * 2, Height - padding * 2);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _pulseTimer.Stop();
            _pulseTimer.Dispose();
        }
        base.Dispose(disposing);
    }
}
