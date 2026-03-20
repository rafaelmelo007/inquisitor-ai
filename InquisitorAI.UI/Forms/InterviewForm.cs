using InquisitorAI.UI.Dtos;
using InquisitorAI.UI.Services.Api;
using InquisitorAI.UI.Services.Local;
using Microsoft.Extensions.DependencyInjection;

namespace InquisitorAI.UI.Forms;

public partial class InterviewForm : Form
{
    private readonly IApiClient _apiClient;
    private readonly ISpeechSynthesisService _ttsService;
    private readonly IAudioRecordingService _recordingService;
    private readonly ISpeechToTextService _speechToTextService;
    private readonly IServiceProvider _serviceProvider;

    private InterviewSessionDto? _session;
    private QuestionnaireDetailDto? _questionnaire;
    private int _currentQuestionIndex;
    private string? _currentRecordingPath;

    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public long QuestionnaireId { get; set; }

    public InterviewForm(
        IApiClient apiClient,
        ISpeechSynthesisService ttsService,
        IAudioRecordingService recordingService,
        ISpeechToTextService speechToTextService,
        IServiceProvider serviceProvider)
    {
        _apiClient = apiClient;
        _ttsService = ttsService;
        _recordingService = recordingService;
        _speechToTextService = speechToTextService;
        _serviceProvider = serviceProvider;
        InitializeComponent();

        btnListen.Click += BtnListen_Click;
        btnRecord.Click += BtnRecord_Click;
        btnStop.Click += BtnStop_Click;
        btnNext.Click += BtnNext_Click;
        Load += InterviewForm_Load;
    }

    private async void InterviewForm_Load(object? sender, EventArgs e)
    {
        try
        {
            aiOverlay.ShowOverlay();
            _session = await _apiClient.StartSessionAsync(QuestionnaireId);
            _questionnaire = await _apiClient.GetQuestionnaireByIdAsync(QuestionnaireId);

            if (_questionnaire.Questions.Count == 0)
            {
                MessageBox.Show("This questionnaire has no questions.",
                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
                return;
            }

            progressBar.Maximum = _questionnaire.Questions.Count;
            _currentQuestionIndex = 0;
            DisplayCurrentQuestion();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to start interview: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
        }
        finally
        {
            aiOverlay.HideOverlay();
        }
    }

    private void DisplayCurrentQuestion()
    {
        if (_questionnaire == null) return;

        var question = _questionnaire.Questions[_currentQuestionIndex];
        progressBar.Value = _currentQuestionIndex + 1;
        lblProgress.Text = $"Question {_currentQuestionIndex + 1} of {_questionnaire.Questions.Count}";
        lblCategory.Text = $"Category: {question.Category ?? "N/A"}";
        lblDifficulty.Text = $"Difficulty: {question.Difficulty ?? "N/A"}";
        rtbQuestion.Text = question.QuestionText;

        // Reset answer panel
        lblTranscript.Text = "";
        lblScore.Text = "";
        lblFeedback.Text = "";

        // Reset button states
        btnRecord.Enabled = true;
        btnStop.Enabled = false;
        btnNext.Enabled = false;
        btnListen.Enabled = true;
    }

    private async void BtnListen_Click(object? sender, EventArgs e)
    {
        if (_questionnaire == null) return;

        try
        {
            btnListen.Enabled = false;
            var question = _questionnaire.Questions[_currentQuestionIndex];
            await _ttsService.SpeakAsync(question.QuestionText, CancellationToken.None);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"TTS error: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnListen.Enabled = true;
        }
    }

    private async void BtnRecord_Click(object? sender, EventArgs e)
    {
        try
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "InquisitorAI");
            Directory.CreateDirectory(tempDir);
            _currentRecordingPath = Path.Combine(tempDir, $"recording_{DateTime.Now:yyyyMMdd_HHmmss}.wav");

            await _recordingService.StartRecordingAsync(_currentRecordingPath, CancellationToken.None);

            btnRecord.Enabled = false;
            btnStop.Enabled = true;
            btnListen.Enabled = false;
            recordingIndicator.StartPulsing();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Recording error: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void BtnStop_Click(object? sender, EventArgs e)
    {
        if (_questionnaire == null || _session == null || _currentRecordingPath == null) return;

        try
        {
            await _recordingService.StopRecordingAsync();
            recordingIndicator.StopPulsing();
            btnStop.Enabled = false;

            aiOverlay.ShowOverlay();

            // Transcribe
            var transcript = await _speechToTextService.TranscribeAsync(_currentRecordingPath, CancellationToken.None);

            // Submit answer
            var question = _questionnaire.Questions[_currentQuestionIndex];
            var answer = await _apiClient.SubmitAnswerAsync(_session.Id, question.Id, transcript);

            // Display results
            lblTranscript.Text = $"Transcript: {answer.Transcript}";
            lblScore.Text = $"Score: {answer.Score:F1}";
            lblFeedback.Text = $"Feedback: {answer.Feedback}";

            btnNext.Enabled = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error processing answer: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            btnRecord.Enabled = true;
        }
        finally
        {
            aiOverlay.HideOverlay();
            CleanupRecording();
        }
    }

    private async void BtnNext_Click(object? sender, EventArgs e)
    {
        if (_questionnaire == null || _session == null) return;

        _currentQuestionIndex++;

        if (_currentQuestionIndex >= _questionnaire.Questions.Count)
        {
            // Last question - finish session
            try
            {
                aiOverlay.ShowOverlay();
                var result = await _apiClient.FinishSessionAsync(_session.Id);
                aiOverlay.HideOverlay();

                var resultForm = _serviceProvider.GetRequiredService<ResultForm>();
                resultForm.Result = result;
                resultForm.ShowDialog(this);
                Close();
            }
            catch (Exception ex)
            {
                aiOverlay.HideOverlay();
                MessageBox.Show($"Error finishing session: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        else
        {
            DisplayCurrentQuestion();
        }
    }

    private void CleanupRecording()
    {
        if (_currentRecordingPath != null && File.Exists(_currentRecordingPath))
        {
            try { File.Delete(_currentRecordingPath); } catch { }
        }
        _currentRecordingPath = null;
    }
}
