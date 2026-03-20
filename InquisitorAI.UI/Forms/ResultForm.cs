using System.Diagnostics;
using InquisitorAI.UI.Dtos;

namespace InquisitorAI.UI.Forms;

public partial class ResultForm : Form
{
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public FinalResultDto? Result { get; set; }

    public ResultForm()
    {
        InitializeComponent();

        btnViewReport.Click += BtnViewReport_Click;
        btnClose.Click += BtnClose_Click;
        Load += ResultForm_Load;
    }

    private void ResultForm_Load(object? sender, EventArgs e)
    {
        if (Result == null)
        {
            Close();
            return;
        }

        lblScore.Text = Result.FinalScore.ToString("F1");
        lblClassification.Text = Result.Classification;
        txtStrengths.Text = Result.Strengths;
        txtImprovements.Text = Result.ImprovementAreas;

        // Color-code classification
        lblClassification.ForeColor = Result.Classification.ToLower() switch
        {
            "approved" => Color.Green,
            "approved with reservations" => Color.Orange,
            "failed" => Color.Red,
            _ => Color.Black
        };
    }

    private void BtnViewReport_Click(object? sender, EventArgs e)
    {
        if (Result == null) return;

        try
        {
            var tempPath = Path.Combine(Path.GetTempPath(), $"InquisitorAI_Report_{DateTime.Now:yyyyMMdd_HHmmss}.md");
            File.WriteAllText(tempPath, Result.ReportContent);
            Process.Start(new ProcessStartInfo
            {
                FileName = tempPath,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to open report: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnClose_Click(object? sender, EventArgs e)
    {
        Close();
    }
}
