namespace InquisitorAI.UI.Services.Local;

public static class UiStrings
{
    public static UiTranslations Get(string? language)
    {
        return language switch
        {
            "Español" or "Spanish" => Spanish,
            "Português" or "Portuguese" => Portuguese,
            _ => English
        };
    }

    private static readonly UiTranslations English = new()
    {
        // Toolbar
        AppTitle = "Inquisitor AI",
        ImportQuestionnaire = "Import Questionnaire",
        Settings = "Settings",
        Logout = "Logout",

        // Left panel
        Questionnaires = "  Questionnaires",

        // Welcome
        WelcomeTitle = "Welcome to Inquisitor AI",
        WelcomeSubtitle = "Select a questionnaire or session from the left panel to get started.\nUse 'Import Questionnaire' to add a new questionnaire.",

        // Questionnaire view
        StartNewSession = "Start New Session",
        DeleteQuestionnaire = "Delete Questionnaire",
        Questions = "questions",
        CreatedBy = "Created by",
        Public = "Public",
        Private = "Private",

        // Session overview
        SessionOverview = "Session Overview",
        ResumeSession = "Resume Session",
        ViewReport = "View Report",
        DeleteSession = "Delete Session",
        InProgress = "In Progress",
        Completed = "Completed",
        Score = "Score",
        Strengths = "Strengths",
        ImprovementAreas = "Improvement Areas",

        // Interview
        Question = "Question",
        Of = "of",
        Category = "Category",
        Difficulty = "Difficulty",
        Listen = "Listen",
        Record = "Record",
        Stop = "Stop",
        Previous = "Previous",
        Next = "Next",
        ShowAnswer = "Show Answer",
        Finish = "Finish",
        FeedbackRevealed = "Feedback was revealed — answer skipped.",
        IdealAnswer = "Ideal Answer",
        YourAnswer = "Your Answer",
        NoTranscript = "(no transcript)",
        OverallFeedback = "Overall Feedback",
        Weaknesses = "Weaknesses",
        HowToImprove = "How to Improve",
        InterviewComplete = "Interview Complete!",

        // Right panel
        SessionProgress = "  Session Progress",
        Status = "Status",
        Pending = "Pending",
        Answered = "Answered",
        Skipped = "Skipped",
        Current = "Current",

        // Settings
        UserSettings = "User Settings",
        Email = "Email:",
        DisplayName = "Display Name:",
        Language = "Language:",
        SpeakQuestions = "Speak questions aloud",
        ThinkingTime = "Thinking time:",
        AnswerTime = "Answer time:",
        TimeUnit = "seconds (0 = no limit)",
        SaveSettings = "Save Settings",

        // Timer
        ThinkingLabel = "Think",
        RecordingLabel = "Rec",
        TimeUp = "Time's up!",

        // Messages
        ConfirmDeleteQuestionnaire = "Are you sure you want to delete this questionnaire?",
        ConfirmDeleteSession = "Are you sure you want to delete this session?",
        ConfirmFinishEarly = "Are you sure you want to finish this session early? Unanswered questions will not be scored.",
        ConfirmLogout = "You have an active interview. Are you sure you want to logout?",
        FinishInterviewFirst = "Please finish or close the current interview first.",
        NoQuestions = "This questionnaire has no questions.",
        ImportSuccess = "Questionnaire imported successfully.",
        SettingsSaved = "Settings saved successfully.",
        DisplayNameRequired = "Display name cannot be empty.",
        FailedToLoadData = "Failed to load data",
        FailedToImport = "Failed to import questionnaire",
        FailedToStart = "Failed to start session",
        FailedToResume = "Failed to resume session",
        FailedToDelete = "Failed to delete",
        FailedToSave = "Failed to save settings",
        ErrorTitle = "Error",
        SuccessTitle = "Success",
        InfoTitle = "Info",
        ConfirmTitle = "Confirm",
        ValidationTitle = "Validation",
        FinishSessionTitle = "Finish Session",
        ConfirmDelete = "Confirm Delete",
        SelectFileTitle = "Select Questionnaire File",
        MarkdownFilter = "Markdown files (*.md)|*.md"
    };

    private static readonly UiTranslations Spanish = new()
    {
        AppTitle = "Inquisitor AI",
        ImportQuestionnaire = "Importar Cuestionario",
        Settings = "Ajustes",
        Logout = "Salir",

        Questionnaires = "  Cuestionarios",

        WelcomeTitle = "Bienvenido a Inquisitor AI",
        WelcomeSubtitle = "Seleccione un cuestionario o sesión del panel izquierdo para comenzar.\nUse 'Importar Cuestionario' para agregar uno nuevo.",

        StartNewSession = "Iniciar Nueva Sesión",
        DeleteQuestionnaire = "Eliminar Cuestionario",
        Questions = "preguntas",
        CreatedBy = "Creado por",
        Public = "Público",
        Private = "Privado",

        SessionOverview = "Resumen de Sesión",
        ResumeSession = "Reanudar Sesión",
        ViewReport = "Ver Informe",
        DeleteSession = "Eliminar Sesión",
        InProgress = "En Progreso",
        Completed = "Completado",
        Score = "Puntuación",
        Strengths = "Fortalezas",
        ImprovementAreas = "Áreas de Mejora",

        Question = "Pregunta",
        Of = "de",
        Category = "Categoría",
        Difficulty = "Dificultad",
        Listen = "Escuchar",
        Record = "Grabar",
        Stop = "Detener",
        Previous = "Anterior",
        Next = "Siguiente",
        ShowAnswer = "Ver Respuesta",
        Finish = "Finalizar",
        FeedbackRevealed = "La respuesta fue revelada — pregunta omitida.",
        IdealAnswer = "Respuesta Ideal",
        YourAnswer = "Tu Respuesta",
        NoTranscript = "(sin transcripción)",
        OverallFeedback = "Retroalimentación General",
        Weaknesses = "Debilidades",
        HowToImprove = "Cómo Mejorar",
        InterviewComplete = "¡Entrevista Completada!",

        SessionProgress = "  Progreso de Sesión",
        Status = "Estado",
        Pending = "Pendiente",
        Answered = "Respondida",
        Skipped = "Omitida",
        Current = "Actual",

        UserSettings = "Configuración de Usuario",
        Email = "Correo:",
        DisplayName = "Nombre:",
        Language = "Idioma:",
        SpeakQuestions = "Leer preguntas en voz alta",
        ThinkingTime = "Tiempo para pensar:",
        AnswerTime = "Tiempo para responder:",
        TimeUnit = "segundos (0 = sin límite)",
        SaveSettings = "Guardar",

        ThinkingLabel = "Pensar",
        RecordingLabel = "Grab",
        TimeUp = "¡Tiempo agotado!",

        ConfirmDeleteQuestionnaire = "¿Está seguro de que desea eliminar este cuestionario?",
        ConfirmDeleteSession = "¿Está seguro de que desea eliminar esta sesión?",
        ConfirmFinishEarly = "¿Está seguro de que desea finalizar esta sesión? Las preguntas sin responder no serán calificadas.",
        ConfirmLogout = "Tiene una entrevista activa. ¿Está seguro de que desea salir?",
        FinishInterviewFirst = "Por favor, termine o cierre la entrevista actual primero.",
        NoQuestions = "Este cuestionario no tiene preguntas.",
        ImportSuccess = "Cuestionario importado exitosamente.",
        SettingsSaved = "Configuración guardada exitosamente.",
        DisplayNameRequired = "El nombre no puede estar vacío.",
        FailedToLoadData = "Error al cargar datos",
        FailedToImport = "Error al importar cuestionario",
        FailedToStart = "Error al iniciar sesión",
        FailedToResume = "Error al reanudar sesión",
        FailedToDelete = "Error al eliminar",
        FailedToSave = "Error al guardar configuración",
        ErrorTitle = "Error",
        SuccessTitle = "Éxito",
        InfoTitle = "Info",
        ConfirmTitle = "Confirmar",
        ValidationTitle = "Validación",
        FinishSessionTitle = "Finalizar Sesión",
        ConfirmDelete = "Confirmar Eliminación",
        SelectFileTitle = "Seleccionar Archivo de Cuestionario",
        MarkdownFilter = "Archivos Markdown (*.md)|*.md"
    };

    private static readonly UiTranslations Portuguese = new()
    {
        AppTitle = "Inquisitor AI",
        ImportQuestionnaire = "Importar Questionário",
        Settings = "Configurações",
        Logout = "Sair",

        Questionnaires = "  Questionários",

        WelcomeTitle = "Bem-vindo ao Inquisitor AI",
        WelcomeSubtitle = "Selecione um questionário ou sessão no painel esquerdo para começar.\nUse 'Importar Questionário' para adicionar um novo.",

        StartNewSession = "Iniciar Nova Sessão",
        DeleteQuestionnaire = "Excluir Questionário",
        Questions = "perguntas",
        CreatedBy = "Criado por",
        Public = "Público",
        Private = "Privado",

        SessionOverview = "Resumo da Sessão",
        ResumeSession = "Retomar Sessão",
        ViewReport = "Ver Relatório",
        DeleteSession = "Excluir Sessão",
        InProgress = "Em Progresso",
        Completed = "Concluído",
        Score = "Pontuação",
        Strengths = "Pontos Fortes",
        ImprovementAreas = "Áreas de Melhoria",

        Question = "Pergunta",
        Of = "de",
        Category = "Categoria",
        Difficulty = "Dificuldade",
        Listen = "Ouvir",
        Record = "Gravar",
        Stop = "Parar",
        Previous = "Anterior",
        Next = "Próxima",
        ShowAnswer = "Ver Resposta",
        Finish = "Finalizar",
        FeedbackRevealed = "A resposta foi revelada — pergunta ignorada.",
        IdealAnswer = "Resposta Ideal",
        YourAnswer = "Sua Resposta",
        NoTranscript = "(sem transcrição)",
        OverallFeedback = "Feedback Geral",
        Weaknesses = "Pontos Fracos",
        HowToImprove = "Como Melhorar",
        InterviewComplete = "Entrevista Concluída!",

        SessionProgress = "  Progresso da Sessão",
        Status = "Estado",
        Pending = "Pendente",
        Answered = "Respondida",
        Skipped = "Ignorada",
        Current = "Atual",

        UserSettings = "Configurações do Usuário",
        Email = "E-mail:",
        DisplayName = "Nome:",
        Language = "Idioma:",
        SpeakQuestions = "Ler perguntas em voz alta",
        ThinkingTime = "Tempo para pensar:",
        AnswerTime = "Tempo para responder:",
        TimeUnit = "segundos (0 = sem limite)",
        SaveSettings = "Salvar",

        ThinkingLabel = "Pensar",
        RecordingLabel = "Grav",
        TimeUp = "Tempo esgotado!",

        ConfirmDeleteQuestionnaire = "Tem certeza de que deseja excluir este questionário?",
        ConfirmDeleteSession = "Tem certeza de que deseja excluir esta sessão?",
        ConfirmFinishEarly = "Tem certeza de que deseja finalizar esta sessão? Perguntas não respondidas não serão pontuadas.",
        ConfirmLogout = "Você tem uma entrevista ativa. Tem certeza de que deseja sair?",
        FinishInterviewFirst = "Por favor, termine ou feche a entrevista atual primeiro.",
        NoQuestions = "Este questionário não tem perguntas.",
        ImportSuccess = "Questionário importado com sucesso.",
        SettingsSaved = "Configurações salvas com sucesso.",
        DisplayNameRequired = "O nome não pode estar vazio.",
        FailedToLoadData = "Falha ao carregar dados",
        FailedToImport = "Falha ao importar questionário",
        FailedToStart = "Falha ao iniciar sessão",
        FailedToResume = "Falha ao retomar sessão",
        FailedToDelete = "Falha ao excluir",
        FailedToSave = "Falha ao salvar configurações",
        ErrorTitle = "Erro",
        SuccessTitle = "Sucesso",
        InfoTitle = "Info",
        ConfirmTitle = "Confirmar",
        ValidationTitle = "Validação",
        FinishSessionTitle = "Finalizar Sessão",
        ConfirmDelete = "Confirmar Exclusão",
        SelectFileTitle = "Selecionar Arquivo de Questionário",
        MarkdownFilter = "Arquivos Markdown (*.md)|*.md"
    };
}

public class UiTranslations
{
    // Toolbar
    public string AppTitle { get; set; } = "";
    public string ImportQuestionnaire { get; set; } = "";
    public string Settings { get; set; } = "";
    public string Logout { get; set; } = "";

    // Left panel
    public string Questionnaires { get; set; } = "";

    // Welcome
    public string WelcomeTitle { get; set; } = "";
    public string WelcomeSubtitle { get; set; } = "";

    // Questionnaire view
    public string StartNewSession { get; set; } = "";
    public string DeleteQuestionnaire { get; set; } = "";
    public string Questions { get; set; } = "";
    public string CreatedBy { get; set; } = "";
    public string Public { get; set; } = "";
    public string Private { get; set; } = "";

    // Session overview
    public string SessionOverview { get; set; } = "";
    public string ResumeSession { get; set; } = "";
    public string ViewReport { get; set; } = "";
    public string DeleteSession { get; set; } = "";
    public string InProgress { get; set; } = "";
    public string Completed { get; set; } = "";
    public string Score { get; set; } = "";
    public string Strengths { get; set; } = "";
    public string ImprovementAreas { get; set; } = "";

    // Interview
    public string Question { get; set; } = "";
    public string Of { get; set; } = "";
    public string Category { get; set; } = "";
    public string Difficulty { get; set; } = "";
    public string Listen { get; set; } = "";
    public string Record { get; set; } = "";
    public string Stop { get; set; } = "";
    public string Previous { get; set; } = "";
    public string Next { get; set; } = "";
    public string ShowAnswer { get; set; } = "";
    public string Finish { get; set; } = "";
    public string FeedbackRevealed { get; set; } = "";
    public string IdealAnswer { get; set; } = "";
    public string YourAnswer { get; set; } = "";
    public string NoTranscript { get; set; } = "";
    public string OverallFeedback { get; set; } = "";
    public string Weaknesses { get; set; } = "";
    public string HowToImprove { get; set; } = "";
    public string InterviewComplete { get; set; } = "";

    // Right panel
    public string SessionProgress { get; set; } = "";
    public string Status { get; set; } = "";
    public string Pending { get; set; } = "";
    public string Answered { get; set; } = "";
    public string Skipped { get; set; } = "";
    public string Current { get; set; } = "";

    // Settings
    public string UserSettings { get; set; } = "";
    public string Email { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Language { get; set; } = "";
    public string SpeakQuestions { get; set; } = "";
    public string ThinkingTime { get; set; } = "";
    public string AnswerTime { get; set; } = "";
    public string TimeUnit { get; set; } = "";
    public string SaveSettings { get; set; } = "";

    // Timer
    public string ThinkingLabel { get; set; } = "";
    public string RecordingLabel { get; set; } = "";
    public string TimeUp { get; set; } = "";

    // Messages
    public string ConfirmDeleteQuestionnaire { get; set; } = "";
    public string ConfirmDeleteSession { get; set; } = "";
    public string ConfirmFinishEarly { get; set; } = "";
    public string ConfirmLogout { get; set; } = "";
    public string FinishInterviewFirst { get; set; } = "";
    public string NoQuestions { get; set; } = "";
    public string ImportSuccess { get; set; } = "";
    public string SettingsSaved { get; set; } = "";
    public string DisplayNameRequired { get; set; } = "";
    public string FailedToLoadData { get; set; } = "";
    public string FailedToImport { get; set; } = "";
    public string FailedToStart { get; set; } = "";
    public string FailedToResume { get; set; } = "";
    public string FailedToDelete { get; set; } = "";
    public string FailedToSave { get; set; } = "";
    public string ErrorTitle { get; set; } = "";
    public string SuccessTitle { get; set; } = "";
    public string InfoTitle { get; set; } = "";
    public string ConfirmTitle { get; set; } = "";
    public string ValidationTitle { get; set; } = "";
    public string FinishSessionTitle { get; set; } = "";
    public string ConfirmDelete { get; set; } = "";
    public string SelectFileTitle { get; set; } = "";
    public string MarkdownFilter { get; set; } = "";
}
