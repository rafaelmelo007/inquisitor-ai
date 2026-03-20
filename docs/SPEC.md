# SPEC — Inquisitor AI

C# · .NET 10 · Vertical Slice Architecture · PostgreSQL · JWT + OAuth

---

## Architecture Overview

```
┌─────────────────────┐     ┌──────────────────────┐
│  InquisitorAI.UI    │     │  InquisitorAI.Web     │
│  WinForms Desktop   │     │  Blazor WebAssembly   │
│  (thin HTTP client) │     │  (web portal)         │
└────────┬────────────┘     └──────────┬────────────┘
         │  JWT Bearer                 │  JWT Bearer
         ▼                             ▼
┌─────────────────────────────────────────────────────┐
│              InquisitorAI.Api                        │
│          .NET 10 Minimal API                         │
├─────────────────────────────────────────────────────┤
│              InquisitorAI.Features                   │
│     Auth · Users · Questionnaires · Sessions · Scores│
├─────────────────────────────────────────────────────┤
│           InquisitorAI.Infrastructure                │
│      EF Core · PostgreSQL · External Services        │
└─────────────────────────────────────────────────────┘
```

---

## Solution Structure

```
InquisitorAI/
├── InquisitorAI.Features/          # VSA business logic — all features
├── InquisitorAI.Api/               # Minimal API host
├── InquisitorAI.Infrastructure/    # EF Core + PostgreSQL + service implementations
├── InquisitorAI.UI/                # WinForms desktop (thin HTTP client)
├── InquisitorAI.Web/               # Angular 19 web portal (not in .sln)
├── InquisitorAI.Tests/             # xUnit tests
├── samples/
│   └── dotnet-interview.md
├── docker-compose.yml
├── .env
├── .dockerignore
└── InquisitorAI.sln
```

---

## Project: InquisitorAI.Features

Single class library. Shared by `InquisitorAI.Api`. No reference to infrastructure, UI, or Web projects. Contains all domain entities, EF configurations, CQRS handlers, DTOs, FluentValidation validators, and Minimal API endpoint definitions.

### Directory Layout

```
InquisitorAI.Features/
├── Setup.cs
├── Shared/
│   ├── IHandler.cs
│   ├── IDateTimeService.cs
│   ├── NotificationHandler.cs
│   └── AppDbContext.cs
├── Auth/
│   ├── Setup.cs
│   ├── IJwtService.cs
│   ├── Domain/
│   │   ├── RefreshToken.cs
│   │   ├── OAuthProvider.cs
│   │   └── RefreshTokenConfiguration.cs
│   ├── Dtos/
│   │   ├── TokenResponseDto.cs
│   │   └── RefreshTokenRequest.cs
│   ├── Commands/
│   │   ├── IssueTokens.cs
│   │   ├── RefreshAccessToken.cs
│   │   └── RevokeRefreshToken.cs
│   └── Endpoints/
│       ├── OAuthCallbackEndpoint.cs
│       ├── RefreshTokenEndpoint.cs
│       └── LogoutEndpoint.cs
├── Users/
│   ├── Setup.cs
│   ├── Domain/
│   │   ├── User.cs
│   │   └── UserConfiguration.cs
│   ├── Dtos/
│   │   ├── UserDto.cs
│   │   └── UpdateProfileRequest.cs
│   ├── Commands/
│   │   └── UpdateUserProfile.cs
│   ├── Queries/
│   │   └── GetCurrentUser.cs
│   ├── Endpoints/
│   │   ├── GetCurrentUserEndpoint.cs
│   │   └── UpdateUserProfileEndpoint.cs
│   └── Extensions/
│       └── UserMappingExtensions.cs
├── Questionnaires/
│   ├── Setup.cs
│   ├── IMarkdownParserService.cs
│   ├── Domain/
│   │   ├── Questionnaire.cs
│   │   ├── Question.cs
│   │   ├── Difficulty.cs
│   │   ├── QuestionnaireConfiguration.cs
│   │   └── QuestionConfiguration.cs
│   ├── Dtos/
│   │   ├── QuestionnaireDto.cs
│   │   ├── QuestionDto.cs
│   │   ├── ParsedQuestionnaireDto.cs
│   │   └── ParsedQuestionDto.cs
│   ├── Commands/
│   │   ├── ImportQuestionnaire.cs
│   │   └── DeleteQuestionnaire.cs
│   ├── Queries/
│   │   ├── GetQuestionnaires.cs
│   │   └── GetQuestionnaireById.cs
│   ├── Endpoints/
│   │   ├── ImportQuestionnaireEndpoint.cs
│   │   ├── GetQuestionnairesEndpoint.cs
│   │   ├── GetQuestionnaireByIdEndpoint.cs
│   │   └── DeleteQuestionnaireEndpoint.cs
│   └── Extensions/
│       └── QuestionnaireMappingExtensions.cs
├── InterviewSessions/
│   ├── Setup.cs
│   ├── IAiEvaluationService.cs
│   ├── IReportGeneratorService.cs
│   ├── Domain/
│   │   ├── InterviewSession.cs
│   │   ├── SessionAnswer.cs
│   │   ├── Classification.cs
│   │   ├── InterviewSessionConfiguration.cs
│   │   └── SessionAnswerConfiguration.cs
│   ├── Dtos/
│   │   ├── InterviewSessionDto.cs
│   │   ├── SessionAnswerDto.cs
│   │   ├── StartSessionRequest.cs
│   │   ├── SubmitAnswerRequest.cs
│   │   ├── EvaluationResultDto.cs
│   │   ├── EvaluateAnswerRequest.cs
│   │   └── FinalResultDto.cs
│   ├── Commands/
│   │   ├── StartInterviewSession.cs
│   │   ├── SubmitAnswer.cs
│   │   ├── FinishInterviewSession.cs
│   │   └── DeleteInterviewSession.cs
│   ├── Queries/
│   │   ├── GetInterviewSessions.cs
│   │   └── GetSessionDetails.cs
│   ├── Endpoints/
│   │   ├── StartSessionEndpoint.cs
│   │   ├── SubmitAnswerEndpoint.cs
│   │   ├── FinishSessionEndpoint.cs
│   │   ├── GetSessionsEndpoint.cs
│   │   ├── GetSessionDetailsEndpoint.cs
│   │   └── DeleteSessionEndpoint.cs
│   └── Extensions/
│       └── SessionMappingExtensions.cs
└── Scores/
    ├── Setup.cs
    ├── Dtos/
    │   ├── LeaderboardEntryDto.cs
    │   └── UserScoreSummaryDto.cs
    ├── Queries/
    │   ├── GetLeaderboard.cs
    │   └── GetUserScores.cs
    └── Endpoints/
        ├── GetLeaderboardEndpoint.cs
        └── GetUserScoresEndpoint.cs
```

---

### Shared/

| File | Description |
|---|---|
| `Shared/IHandler.cs` | `ICommand`, `ICommand<TResult>`, `IQuery<TResult>`, `ICommandHandler<TCommand>`, `ICommandHandler<TCommand, TResult>`, `IQueryHandler<TQuery, TResult>`. |
| `Shared/IDateTimeService.cs` | `DateTimeOffset UtcNow { get; }`. Injected into all command handlers. Never call `DateTimeOffset.UtcNow` directly. |
| `Shared/NotificationHandler.cs` | `List<string> _errors`, `IReadOnlyList<string> Errors`, `bool HasErrors`, `void AddError(string)`. Registered as **scoped**. |
| `Shared/AppDbContext.cs` | EF Core `DbContext`. `DbSet<User>`, `DbSet<RefreshToken>`, `DbSet<Questionnaire>`, `DbSet<Question>`, `DbSet<InterviewSession>`, `DbSet<SessionAnswer>`. `ApplyConfigurationsFromAssembly` in `OnModelCreating`. |

### Root Setup.cs

```csharp
public static class Setup
{
    public static IServiceCollection RegisterAllFeatures(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthFeature(config);
        services.AddUsersFeature();
        services.AddQuestionnairesFeature();
        services.AddInterviewSessionsFeature(config);
        services.AddScoresFeature();
        return services;
    }

    public static WebApplication MapAllEndpoints(this WebApplication app)
    {
        app.MapAuthEndpoints();
        app.MapUsersEndpoints();
        app.MapQuestionnairesEndpoints();
        app.MapInterviewSessionsEndpoints();
        app.MapScoresEndpoints();
        return app;
    }
}
```

---

### Feature: Auth/

| File | Description |
|---|---|
| `Auth/Setup.cs` | Registers `IJwtService`, command handlers. Configures `AddAuthentication` with JWT Bearer (primary) and OAuth schemes for Google, LinkedIn, GitHub. |
| `Auth/IJwtService.cs` | `string GenerateAccessToken(User user)`. `string GenerateRefreshToken()`. `ClaimsPrincipal? ValidateAccessToken(string token)`. |
| `Auth/Domain/OAuthProvider.cs` | Enum: `Google`, `LinkedIn`, `GitHub`. |
| `Auth/Domain/RefreshToken.cs` | Entity. Fields: `Id` (long), `UserId`, `Token` (string, hashed), `ExpiresAt`, `RevokedAt` (nullable), `CreatedAt`, `UpdatedAt`, `RowVersion`. Navigation: `User`. |
| `Auth/Domain/RefreshTokenConfiguration.cs` | Table `inq_refresh_tokens`. FK to `users`. Index on `token`. |
| `Auth/Dtos/TokenResponseDto.cs` | Record: `AccessToken`, `RefreshToken`, `ExpiresIn` (seconds). |
| `Auth/Dtos/RefreshTokenRequest.cs` | Record: `RefreshToken`. |
| `Auth/Commands/IssueTokens.cs` | `IssueTokensCommand(string Provider, string ExternalId, string Email, string DisplayName, string? AvatarUrl) : ICommand<TokenResponseDto?>`. Handler upserts `User` by `(provider, external_id)`, creates hashed `RefreshToken`, calls `IJwtService.GenerateAccessToken`. |
| `Auth/Commands/RefreshAccessToken.cs` | `RefreshAccessTokenCommand(string RefreshToken) : ICommand<TokenResponseDto?>`. Handler validates token against DB (not revoked, not expired), issues new access + refresh token, revokes old refresh token. |
| `Auth/Commands/RevokeRefreshToken.cs` | `RevokeRefreshTokenCommand(long UserId) : ICommand`. Sets `RevokedAt` on all active refresh tokens for the user. |
| `Auth/Endpoints/OAuthCallbackEndpoint.cs` | One endpoint per provider. On `OnTicketReceived` event from ASP.NET Core OAuth middleware: extracts claims, calls `IssueTokensCommand`. **Web flow**: sets `HttpOnly` cookie + redirects to `{WebPortalUrl}/auth/callback?token=...`. **Native flow**: if `state` param contains a loopback URI, redirects to `http://localhost:{PORT}/callback?access_token=...&refresh_token=...`. |
| `Auth/Endpoints/RefreshTokenEndpoint.cs` | `POST /auth/refresh`. No auth required. Calls `RefreshAccessTokenCommand`. |
| `Auth/Endpoints/LogoutEndpoint.cs` | `POST /auth/logout`. Requires auth. Calls `RevokeRefreshTokenCommand` for current user. |

### Feature: Users/

| File | Description |
|---|---|
| `Users/Setup.cs` | Registers command and query handlers. |
| `Users/Domain/User.cs` | Entity. Fields: `Id` (long), `Provider` (enum), `ExternalId`, `Email`, `DisplayName`, `AvatarUrl`, `CreatedAt`, `UpdatedAt`, `RowVersion`. Navigation: `ICollection<RefreshToken>`, `ICollection<Questionnaire>`, `ICollection<InterviewSession>`. |
| `Users/Domain/UserConfiguration.cs` | Table `inq_users`. Unique index on `(provider, external_id)`. Unique index on `email`. Stores `Provider` as string. |
| `Users/Dtos/UserDto.cs` | Record: `Id`, `Email`, `DisplayName`, `AvatarUrl`, `Provider`, `CreatedAt`. |
| `Users/Dtos/UpdateProfileRequest.cs` | Record: `DisplayName`, `AvatarUrl`. |
| `Users/Commands/UpdateUserProfile.cs` | `UpdateUserProfileCommand(long UserId, string DisplayName, string? AvatarUrl) : ICommand<UserDto?>`. Updates `DisplayName` and `AvatarUrl`. |
| `Users/Queries/GetCurrentUser.cs` | `GetCurrentUserQuery(long UserId) : IQuery<UserDto?>`. Dapper: `SELECT * FROM inq_users WHERE id = @UserId`. |
| `Users/Endpoints/GetCurrentUserEndpoint.cs` | `GET /users/me`. Requires auth. Extracts `UserId` from JWT claim, calls `GetCurrentUserQuery`. |
| `Users/Endpoints/UpdateUserProfileEndpoint.cs` | `PUT /users/me`. Requires auth. Calls `UpdateUserProfileCommand`. |
| `Users/Extensions/UserMappingExtensions.cs` | `ToDto(this User)`. |

### Feature: Questionnaires/

| File | Description |
|---|---|
| `Questionnaires/Setup.cs` | Registers `IMarkdownParserService`, command handlers, query handlers. |
| `Questionnaires/IMarkdownParserService.cs` | `ParsedQuestionnaireDto Parse(string content)` — parses raw Markdown string (not file path; file is uploaded to the API and read there). Throws `FormatException` on invalid structure. |
| `Questionnaires/Domain/Questionnaire.cs` | Entity. Fields: `Id`, `Name`, `CreatedByUserId`, `IsPublic`, `CreatedAt`, `UpdatedAt`, `RowVersion`. Navigation: `User`, `ICollection<Question>`. |
| `Questionnaires/Domain/Question.cs` | Entity. Fields: `Id`, `QuestionnaireId`, `OrderIndex`, `Category`, `Difficulty` (enum), `QuestionText`, `IdealAnswer`, `CreatedAt`, `UpdatedAt`, `RowVersion`. Navigation: `Questionnaire`. |
| `Questionnaires/Domain/Difficulty.cs` | Enum: `Easy`, `Medium`, `Hard`. |
| `Questionnaires/Domain/QuestionnaireConfiguration.cs` | Table `inq_questionnaires`. FK to `users`. |
| `Questionnaires/Domain/QuestionConfiguration.cs` | Table `inq_questions`. FK to `questionnaires`. Stores `Difficulty` as string. |
| `Questionnaires/Dtos/QuestionnaireDto.cs` | Record: `Id`, `Name`, `CreatedByUserId`, `CreatedByDisplayName`, `IsPublic`, `QuestionCount`, `CreatedAt`. |
| `Questionnaires/Dtos/QuestionDto.cs` | Record: `Id`, `QuestionnaireId`, `OrderIndex`, `Category`, `Difficulty`, `QuestionText`, `IdealAnswer`. |
| `Questionnaires/Dtos/ParsedQuestionnaireDto.cs` | Record: `Name`, `Questions` (`IReadOnlyList<ParsedQuestionDto>`). |
| `Questionnaires/Dtos/ParsedQuestionDto.cs` | Record: `OrderIndex`, `Category`, `Difficulty`, `QuestionText`, `IdealAnswer`. |
| `Questionnaires/Commands/ImportQuestionnaire.cs` | `ImportQuestionnaireCommand(long UserId, string MarkdownContent, bool IsPublic) : ICommand<QuestionnaireDto?>`. Calls `IMarkdownParserService.Parse`, persists `Questionnaire` + `Question` entities via EF Core. |
| `Questionnaires/Commands/DeleteQuestionnaire.cs` | `DeleteQuestionnaireCommand(long QuestionnaireId, long RequestingUserId) : ICommand`. Validates requester is the owner. Deletes with cascade. |
| `Questionnaires/Queries/GetQuestionnaires.cs` | `GetQuestionnairesQuery(long? UserId) : IQuery<IEnumerable<QuestionnaireDto>>`. Dapper. If `UserId` provided: returns user's own + all public. If null: returns public only. Includes question count via join. Ordered by `created_at DESC`. |
| `Questionnaires/Queries/GetQuestionnaireById.cs` | `GetQuestionnaireByIdQuery(long Id, long? RequestingUserId) : IQuery<QuestionnaireDto?>`. Returns questionnaire with questions. Validates visibility (public or owned by requester). |
| `Questionnaires/Endpoints/ImportQuestionnaireEndpoint.cs` | `POST /questionnaires`. Requires auth. Accepts `multipart/form-data` with `file` (`.md`) and `isPublic` (bool). Reads file content, calls `ImportQuestionnaireCommand`. |
| `Questionnaires/Endpoints/GetQuestionnairesEndpoint.cs` | `GET /questionnaires`. Optional auth. Passes `UserId` from JWT if present. |
| `Questionnaires/Endpoints/GetQuestionnaireByIdEndpoint.cs` | `GET /questionnaires/{id}`. Optional auth. |
| `Questionnaires/Endpoints/DeleteQuestionnaireEndpoint.cs` | `DELETE /questionnaires/{id}`. Requires auth. |

### Feature: InterviewSessions/

| File | Description |
|---|---|
| `InterviewSessions/Setup.cs` | Registers `IAiEvaluationService`, `IReportGeneratorService`, all command and query handlers. |
| `InterviewSessions/IAiEvaluationService.cs` | `Task<EvaluationResultDto> EvaluateAsync(EvaluateAnswerRequest request, CancellationToken ct)`. Called server-side on answer submission. |
| `InterviewSessions/IReportGeneratorService.cs` | `string Generate(InterviewSessionDto session)`. Returns a Markdown string. Synchronous — no I/O; report is stored in the DB column `report_content`. |
| `InterviewSessions/Domain/InterviewSession.cs` | Entity. Fields: `Id`, `UserId`, `QuestionnaireId`, `StartedAt`, `EndedAt`, `DurationSeconds`, `FinalScore`, `Classification` (enum), `ReportContent` (Markdown text), `CreatedAt`, `UpdatedAt`, `RowVersion`. Navigation: `User`, `Questionnaire`, `ICollection<SessionAnswer>`. |
| `InterviewSessions/Domain/SessionAnswer.cs` | Entity. Fields: `Id`, `SessionId`, `QuestionId`, `Transcript`, `Score`, `AiFeedback`, `Strengths`, `Weaknesses`, `ImprovementSuggestions`, `CreatedAt`, `UpdatedAt`, `RowVersion`. |
| `InterviewSessions/Domain/Classification.cs` | Enum: `Approved`, `ApprovedWithReservations`, `Failed`. |
| `InterviewSessions/Domain/InterviewSessionConfiguration.cs` | Table `inq_interview_sessions`. FK to `users` and `questionnaires`. Stores `Classification` as string. `report_content` is `HasColumnType("text")`. |
| `InterviewSessions/Domain/SessionAnswerConfiguration.cs` | Table `inq_session_answers`. FK to `interview_sessions` (cascade delete) and `questions`. |
| `InterviewSessions/Dtos/StartSessionRequest.cs` | Record: `QuestionnaireId`. |
| `InterviewSessions/Dtos/SubmitAnswerRequest.cs` | Record: `QuestionId`, `Transcript`. |
| `InterviewSessions/Dtos/InterviewSessionDto.cs` | Record: `Id`, `UserId`, `QuestionnaireId`, `QuestionnaireName`, `StartedAt`, `EndedAt`, `DurationSeconds`, `FinalScore`, `Classification`, `ReportContent`, `Answers` (`IReadOnlyList<SessionAnswerDto>`). |
| `InterviewSessions/Dtos/SessionAnswerDto.cs` | Record: `Id`, `SessionId`, `QuestionId`, `QuestionText`, `IdealAnswer`, `Transcript`, `Score`, `AiFeedback`, `Strengths`, `Weaknesses`, `ImprovementSuggestions`. |
| `InterviewSessions/Dtos/EvaluationResultDto.cs` | Record: `Score`, `Summary`, `Strengths`, `Weaknesses`, `ImprovementSuggestions`. |
| `InterviewSessions/Dtos/EvaluateAnswerRequest.cs` | Record: `QuestionText`, `IdealAnswer`, `Transcript`. |
| `InterviewSessions/Dtos/FinalResultDto.cs` | Record: `FinalScore`, `Classification`, `Strengths`, `ImprovementAreas`, `ReportContent`. |
| `InterviewSessions/Commands/StartInterviewSession.cs` | `StartInterviewSessionCommand(long UserId, long QuestionnaireId) : ICommand<InterviewSessionDto?>`. Creates session with `StartedAt = clock.UtcNow`. Validates questionnaire exists and is accessible by user. |
| `InterviewSessions/Commands/SubmitAnswer.cs` | `SubmitAnswerCommand(long SessionId, long UserId, long QuestionId, string Transcript) : ICommand<SessionAnswerDto?>`. Validates session belongs to user. Calls `IAiEvaluationService.EvaluateAsync`. Persists `SessionAnswer`. |
| `InterviewSessions/Commands/FinishInterviewSession.cs` | `FinishInterviewSessionCommand(long SessionId, long UserId) : ICommand<FinalResultDto?>`. Validates session belongs to user. Averages scores → `FinalScore`. Determines `Classification` (thresholds: 8.0 / 6.5, hardcoded — configurable in future). Sets `EndedAt`, `DurationSeconds`. Calls `IReportGeneratorService.Generate` → stores `ReportContent`. |
| `InterviewSessions/Commands/DeleteInterviewSession.cs` | `DeleteInterviewSessionCommand(long SessionId, long UserId) : ICommand`. Validates ownership. Deletes with cascade. |
| `InterviewSessions/Queries/GetInterviewSessions.cs` | `GetInterviewSessionsQuery(long UserId) : IQuery<IEnumerable<InterviewSessionDto>>`. Dapper. Returns user's sessions with questionnaire name. No answers in list view. Ordered by `started_at DESC`. |
| `InterviewSessions/Queries/GetSessionDetails.cs` | `GetSessionDetailsQuery(long SessionId, long UserId) : IQuery<InterviewSessionDto?>`. Dapper. Full session with all answers, question texts, ideal answers joined. Validates ownership. |
| `InterviewSessions/Endpoints/StartSessionEndpoint.cs` | `POST /sessions`. Requires auth. |
| `InterviewSessions/Endpoints/SubmitAnswerEndpoint.cs` | `POST /sessions/{id}/answers`. Requires auth. |
| `InterviewSessions/Endpoints/FinishSessionEndpoint.cs` | `POST /sessions/{id}/finish`. Requires auth. |
| `InterviewSessions/Endpoints/GetSessionsEndpoint.cs` | `GET /sessions`. Requires auth. |
| `InterviewSessions/Endpoints/GetSessionDetailsEndpoint.cs` | `GET /sessions/{id}`. Requires auth. Validates ownership. |
| `InterviewSessions/Endpoints/DeleteSessionEndpoint.cs` | `DELETE /sessions/{id}`. Requires auth. |

### Feature: Scores/

| File | Description |
|---|---|
| `Scores/Setup.cs` | Registers query handlers. |
| `Scores/Dtos/LeaderboardEntryDto.cs` | Record: `Rank`, `UserId`, `DisplayName`, `AvatarUrl`, `BestScore`, `SessionCount`, `AverageScore`. |
| `Scores/Dtos/UserScoreSummaryDto.cs` | Record: `UserId`, `DisplayName`, `TotalSessions`, `AverageScore`, `BestScore`, `LastSessionAt`. |
| `Scores/Queries/GetLeaderboard.cs` | `GetLeaderboardQuery(int Top = 50) : IQuery<IEnumerable<LeaderboardEntryDto>>`. Dapper. Aggregates `final_score` from `inq_interview_sessions` joined with `inq_users`. Orders by `best_score DESC`. |
| `Scores/Queries/GetUserScores.cs` | `GetUserScoresQuery(long UserId) : IQuery<UserScoreSummaryDto?>`. Dapper. Returns aggregated stats for a single user. |
| `Scores/Endpoints/GetLeaderboardEndpoint.cs` | `GET /scores/leaderboard`. No auth required. |
| `Scores/Endpoints/GetUserScoresEndpoint.cs` | `GET /scores/me`. Requires auth. |

---

## Project: InquisitorAI.Api

.NET 10 Minimal API host. References `InquisitorAI.Features` and `InquisitorAI.Infrastructure`.

| File | Description |
|---|---|
| `InquisitorAI.Api.csproj` | `net10.0` console app. References Features and Infrastructure. |
| `Program.cs` | Configures the host. Loads `.env` via `DotNetEnv`. Calls `services.AddInfrastructure(config)`, `services.RegisterAllFeatures(config)`. Configures JWT Bearer auth, CORS, rate limiting, Swagger, global exception handler, health checks. Calls `app.MapAllEndpoints()`. Applies migrations at startup. |
| `Dockerfile` | Multi-stage build targeting `mcr.microsoft.com/dotnet/sdk:10.0` → `mcr.microsoft.com/dotnet/aspnet:10.0`. Exposes port 8080. |

### Program.cs — key registrations

```csharp
DotNetEnv.Env.Load();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddInfrastructure(config);
builder.Services.RegisterAllFeatures(config);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* validate with secret from .env */ })
    .AddGoogle(options => { /* ClientId, ClientSecret from .env */ })
    .AddGitHub(options => { /* ClientId, ClientSecret from .env */ })
    .AddLinkedIn(options => { /* ClientId, ClientSecret from .env */ });

builder.Services.AddAuthorization();
builder.Services.AddOpenApi();
builder.Services.AddRateLimiter(...);
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler(...);
app.MapAllEndpoints();
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live");
```

---

## Project: InquisitorAI.Infrastructure

Implements all service interfaces declared in Features. References `InquisitorAI.Features` only.

| File | Description |
|---|---|
| `InquisitorAI.Infrastructure.csproj` | Class library. References `InquisitorAI.Features`. |
| `Services/DateTimeService.cs` | Implements `IDateTimeService`. Returns `DateTimeOffset.UtcNow`. Singleton. |
| `Services/JwtService.cs` | Implements `IJwtService`. Uses `System.IdentityModel.Tokens.Jwt`. Signs with symmetric key from config. Access token lifetime: 15 minutes. Refresh token: `Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))` — stored as SHA-256 hash in DB. |
| `Services/MarkdownParserService.cs` | Implements `IMarkdownParserService`. Parses raw Markdown string content (not file path). Same rules as before — H1 = name, `## H2` = question blocks, bold-key fields. |
| `Services/ClaudeAiEvaluationService.cs` | Implements `IAiEvaluationService`. Sends structured prompt to Anthropic Claude API (`POST /v1/messages`) via `HttpClient`. API key from config. Parses JSON response into `EvaluationResultDto`. |
| `Services/MarkdownReportGeneratorService.cs` | Implements `IReportGeneratorService`. Builds Markdown string from `InterviewSessionDto`. Returns string — no file I/O (stored in DB). Synchronous. |
| `Migrations/` | EF Core migrations. Initial migration: `InitialCreate`. Named descriptively. Applied at startup. |
| `Setup/InfrastructureSetup.cs` | `AddInfrastructure(this IServiceCollection services, IConfiguration config)`. Registers `AppDbContext` with `UseNpgsql`, `IDbConnection` (Npgsql), all service implementations, `IDateTimeService` (singleton), `HttpClient` for external APIs. |

---

## Project: InquisitorAI.UI

WinForms desktop client. Thin HTTP client — no EF Core, no local database. Handles TTS, audio recording, and transcription locally. Sends the transcript to the API.

### Local Concerns (UI-project only — no interface in Features)

| File | Description |
|---|---|
| `Services/Local/ISpeechSynthesisService.cs` | `Task SpeakAsync(string text, CancellationToken ct)`. `IEnumerable<string> GetAvailableVoices()`. |
| `Services/Local/IAudioRecordingService.cs` | `Task StartRecordingAsync(string outputFilePath, CancellationToken ct)`. `Task StopRecordingAsync()`. `bool IsRecording { get; }`. |
| `Services/Local/ISpeechToTextService.cs` | `Task<string> TranscribeAsync(string audioFilePath, CancellationToken ct)`. Calls OpenAI Whisper API directly from WinForms. |
| `Services/Local/WindowsTtsService.cs` | Implements `ISpeechSynthesisService` using `System.Speech.Synthesis.SpeechSynthesizer`. |
| `Services/Local/NAudioRecordingService.cs` | Implements `IAudioRecordingService` using `NAudio.Wave.WaveInEvent`. Writes to `%TEMP%/InquisitorAI/`. |
| `Services/Local/OpenAiWhisperService.cs` | Implements `ISpeechToTextService`. Sends WAV file to `POST /v1/audio/transcriptions` via `HttpClient`. API key stored in Windows Credential Manager. |

### API Client Layer

| File | Description |
|---|---|
| `Services/Api/IApiClient.cs` | Interface grouping all API call methods (see below). |
| `Services/Api/ApiClient.cs` | Implements `IApiClient`. `HttpClient` with base address from config. Attaches `Authorization: Bearer {token}` to every request. Handles 401 by calling token refresh and retrying once. |
| `Services/Api/ITokenStore.cs` | `string? GetAccessToken()`. `void SaveTokens(string accessToken, string refreshToken)`. `void Clear()`. |
| `Services/Api/WindowsCredentialTokenStore.cs` | Implements `ITokenStore`. Stores tokens in Windows Credential Manager via `CredentialManagement` NuGet package. |

### Authentication Flow (WinForms)

| File | Description |
|---|---|
| `Auth/OAuthHandler.cs` | Orchestrates the native OAuth flow. `Task<TokenResponseDto?> AuthenticateAsync(OAuthProvider provider)`. Opens the system browser to `GET {ApiBaseUrl}/auth/{provider}?redirect_uri=http://localhost:{port}/callback`. Starts a local `HttpListener` on a random available port. Waits for the callback request containing `access_token` and `refresh_token` as query params. Saves tokens via `ITokenStore`. Stops listener. |

### Forms & Controls

| File | Description |
|---|---|
| `Program.cs` | Entry point. Builds `ServiceProvider`. Resolves `MainForm`. |
| `Setup/ApplicationSetup.cs` | Registers all local services, `IApiClient`, `ITokenStore`, `OAuthHandler`, forms (transient). Loads `appsettings.json`. |
| `appsettings.json` | `ApiBaseUrl`, `OpenAiApiKey` (Whisper only; Claude key is on server). |
| `Forms/LoginForm.cs` + `Designer.cs` | Login screen. Three buttons: **Sign in with Google**, **Sign in with GitHub**, **Sign in with LinkedIn**. Each calls `OAuthHandler.AuthenticateAsync`. On success, closes and opens `MainForm`. |
| `Forms/MainForm.cs` + `Designer.cs` | Home screen. Calls `GET /questionnaires` on load. `ListView` of available questionnaires. Buttons: **Upload Questionnaire** (file dialog → `POST /questionnaires`), **Start Interview** (opens `InterviewForm`), **History** (opens `HistoryForm`), **Profile** (opens `ProfileForm`), **Logout**. |
| `Forms/InterviewForm.cs` + `Designer.cs` | Interview screen. On open: calls `POST /sessions` to start. Displays question progress bar, category, difficulty, question text. Buttons: **Listen** (local TTS), **Record** (local NAudio), **Stop** (local Whisper transcription → `POST /sessions/{id}/answers` → shows transcript + score + feedback from API), **Next Question**. On last question: calls `POST /sessions/{id}/finish`, opens `ResultForm`. Shows `RecordingIndicatorControl` while recording. Shows `AiProcessingOverlayControl` during API calls. |
| `Forms/ResultForm.cs` + `Designer.cs` | Final result. Shows `FinalScore`, colour-coded classification, strengths, improvement areas. **View Report** copies `ReportContent` to a temp `.md` file and opens it with the default OS app. |
| `Forms/HistoryForm.cs` + `Designer.cs` | History. `DataGridView` from `GET /sessions`. Columns: Questionnaire Name, Date, Duration, Score, Classification. **Open Report** and **Delete** (`DELETE /sessions/{id}`) buttons. |
| `Forms/ProfileForm.cs` + `Designer.cs` | User profile. Shows data from `GET /users/me`. Editable `DisplayName`. **Save** calls `PUT /users/me`. |
| `Controls/RecordingIndicatorControl.cs` | Red pulsing circle via `System.Windows.Forms.Timer`. |
| `Controls/AiProcessingOverlayControl.cs` | Semi-transparent overlay with marquee `ProgressBar` and "AI is analyzing…" label. |

---

## Project: InquisitorAI.Web

Angular (latest) SPA web portal. Standalone project — **not part of the `.sln`**. Lives in the repo root alongside the .NET projects. Built with `ng build`; served from its own Nginx container in Docker.

### Design Principles

| Principle | Implementation |
|---|---|
| **Vertical Slice Architecture** | Each feature is self-contained: routes, service, store, components, and models in one folder. No cross-feature imports. |
| **Standalone components** | All components use `standalone: true` (Angular default). No `NgModule`. Imports declared per-component. |
| **Tailwind CSS** | Utility classes in all templates. No per-component `.scss` files. Only `styles.scss` imports Tailwind directives. |
| **Signals** | Store state is exposed as signals. Components read `store.property()` directly in templates using Angular control flow (`@if`, `@for`). No `async` pipe. |
| **Observables** | Services return `Observable<T>` via `HttpClient`. Services never call `.subscribe()`. |
| **Stores (`@ngrx/signals`)** | `signalStore()` bridges Observables to signals via `rxMethod` + `tapResponse`. Components inject the store; the store injects the service. |
| **Pattern** | Component → injects Store → `rxMethod` calls Service → Service calls `HttpClient` |

### Directory Layout

```
InquisitorAI.Web/
├── src/
│   ├── app/
│   │   ├── app.component.ts
│   │   ├── app.component.html
│   │   ├── app.routes.ts
│   │   ├── app.config.ts
│   │   ├── core/                         # Cross-cutting: auth state, HTTP, guards
│   │   │   └── auth/
│   │   │       ├── auth.service.ts
│   │   │       ├── auth.store.ts
│   │   │       ├── token-storage.service.ts
│   │   │       ├── auth.interceptor.ts
│   │   │       ├── token-refresh.interceptor.ts
│   │   │       └── auth.guard.ts
│   │   ├── features/
│   │   │   ├── home/
│   │   │   │   ├── home.routes.ts
│   │   │   │   ├── home.component.ts
│   │   │   │   └── home.component.html
│   │   │   ├── login/
│   │   │   │   ├── login.routes.ts
│   │   │   │   ├── login.component.ts
│   │   │   │   └── login.component.html
│   │   │   ├── auth-callback/
│   │   │   │   ├── auth-callback.routes.ts
│   │   │   │   └── auth-callback.component.ts
│   │   │   ├── dashboard/
│   │   │   │   ├── dashboard.routes.ts
│   │   │   │   ├── dashboard.store.ts
│   │   │   │   ├── dashboard.component.ts
│   │   │   │   └── dashboard.component.html
│   │   │   ├── questionnaires/
│   │   │   │   ├── questionnaires.routes.ts
│   │   │   │   ├── questionnaires.service.ts
│   │   │   │   ├── questionnaires.store.ts
│   │   │   │   ├── models/
│   │   │   │   │   └── questionnaire.model.ts
│   │   │   │   ├── questionnaire-list/
│   │   │   │   │   ├── questionnaire-list.component.ts
│   │   │   │   │   └── questionnaire-list.component.html
│   │   │   │   └── questionnaire-detail/
│   │   │   │       ├── questionnaire-detail.component.ts
│   │   │   │       └── questionnaire-detail.component.html
│   │   │   ├── sessions/
│   │   │   │   ├── sessions.routes.ts
│   │   │   │   ├── sessions.service.ts
│   │   │   │   ├── sessions.store.ts
│   │   │   │   ├── models/
│   │   │   │   │   └── session.model.ts
│   │   │   │   ├── session-list/
│   │   │   │   │   ├── session-list.component.ts
│   │   │   │   │   └── session-list.component.html
│   │   │   │   └── session-detail/
│   │   │   │       ├── session-detail.component.ts
│   │   │   │       └── session-detail.component.html
│   │   │   ├── leaderboard/
│   │   │   │   ├── leaderboard.routes.ts
│   │   │   │   ├── leaderboard.service.ts
│   │   │   │   ├── leaderboard.store.ts
│   │   │   │   ├── models/
│   │   │   │   │   └── leaderboard.model.ts
│   │   │   │   ├── leaderboard.component.ts
│   │   │   │   └── leaderboard.component.html
│   │   │   └── profile/
│   │   │       ├── profile.routes.ts
│   │   │       ├── profile.service.ts
│   │   │       ├── profile.store.ts
│   │   │       ├── profile.component.ts
│   │   │       └── profile.component.html
│   │   └── shared/
│   │       ├── models/
│   │       │   └── user.model.ts
│   │       └── components/
│   │           ├── loading-spinner/
│   │           │   └── loading-spinner.component.ts
│   │           ├── score-badge/
│   │           │   └── score-badge.component.ts
│   │           └── markdown-viewer/
│   │               └── markdown-viewer.component.ts
│   ├── environments/
│   │   ├── environment.ts
│   │   └── environment.prod.ts
│   ├── index.html
│   ├── main.ts
│   └── styles.scss
├── angular.json
├── package.json
├── tsconfig.json
├── tailwind.config.js
├── postcss.config.js
├── nginx.conf
└── Dockerfile
```

---

### Core: Auth

The auth feature lives in `core/` because its state (`AuthStore`) is consumed by the `app.routes.ts` guard and the HTTP interceptors — it is a true cross-cutting concern.

| File | Description |
|---|---|
| `core/auth/token-storage.service.ts` | `@Injectable({ providedIn: 'root' })`. Wraps `localStorage`. `getAccessToken(): string \| null`. `getRefreshToken(): string \| null`. `saveTokens(access: string, refresh: string): void`. `clear(): void`. |
| `core/auth/auth.service.ts` | `@Injectable({ providedIn: 'root' })`. Raw HTTP calls only. `login(provider): void` — `window.location.href = '{apiBaseUrl}/auth/{provider}'`. `logout(): Observable<void>` — `POST /auth/logout`. `refresh(token: string): Observable<TokenResponse>` — `POST /auth/refresh`. |
| `core/auth/auth.store.ts` | `signalStore({ providedIn: 'root' })`. State: `currentUser: UserDto \| null`, `loading: boolean`. Computed: `isAuthenticated = computed(() => !!currentUser())`. Methods: `restoreSession()` — decodes stored JWT with `jwt-decode`, sets `currentUser`; `logout: rxMethod` — calls `authService.logout()`, clears `TokenStorageService`, sets `currentUser` to null, navigates to `/`. |
| `core/auth/auth.interceptor.ts` | `HttpInterceptorFn`. Reads token from `TokenStorageService`. Clones request with `Authorization: Bearer {token}` header. Skips `/auth/refresh` and `/auth/logout`. |
| `core/auth/token-refresh.interceptor.ts` | `HttpInterceptorFn`. On 401: calls `authService.refresh(refreshToken)` as Observable, saves new tokens, retries the original request once via `switchMap`. On second failure or missing refresh token: calls `authStore.logout()`. Prevents concurrent refresh calls with a `BehaviorSubject` lock. |
| `core/auth/auth.guard.ts` | `CanActivateFn`. Injects `AuthStore`. Returns `true` if `authStore.isAuthenticated()`. Otherwise navigates to `/login` and returns `false`. |

---

### Store Pattern (applied to every feature store)

```typescript
// Example: questionnaires.store.ts
interface QuestionnairesState {
  questionnaires: QuestionnaireDto[];
  selected: QuestionnaireDto | null;
  loading: boolean;
  error: string | null;
}

export const QuestionnairesStore = signalStore(
  { providedIn: 'root' },
  withState<QuestionnairesState>({
    questionnaires: [], selected: null, loading: false, error: null,
  }),
  withComputed(({ questionnaires }) => ({
    count: computed(() => questionnaires().length),
  })),
  withMethods((store, service = inject(QuestionnairesService)) => ({
    loadAll: rxMethod<void>(pipe(
      tap(() => patchState(store, { loading: true, error: null })),
      switchMap(() => service.getAll()),
      tapResponse({
        next: (questionnaires) => patchState(store, { questionnaires, loading: false }),
        error: (err) => patchState(store, { error: String(err), loading: false }),
      }),
    )),
    import: rxMethod<{ file: File; isPublic: boolean }>(pipe(
      switchMap(({ file, isPublic }) => service.import(file, isPublic)),
      tapResponse({
        next: (q) => patchState(store, (s) => ({ questionnaires: [...s.questionnaires, q] })),
        error: (err) => patchState(store, { error: String(err) }),
      }),
    )),
    remove: rxMethod<number>(pipe(
      switchMap((id) => service.delete(id).pipe(map(() => id))),
      tapResponse({
        next: (id) => patchState(store, (s) => ({ questionnaires: s.questionnaires.filter(q => q.id !== id) })),
        error: (err) => patchState(store, { error: String(err) }),
      }),
    )),
  })),
);
```

### Component Pattern (applied to every feature component)

```typescript
// Example: questionnaire-list.component.ts
@Component({
  standalone: true,
  selector: 'app-questionnaire-list',
  imports: [RouterModule, LoadingSpinnerComponent, ScoreBadgeComponent],
  templateUrl: './questionnaire-list.component.html',
})
export class QuestionnaireListComponent {
  readonly store = inject(QuestionnairesStore);

  ngOnInit(): void {
    this.store.loadAll();
  }

  onFileSelected(event: Event, isPublic: boolean): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (file) this.store.import({ file, isPublic });
  }
}
```

```html
<!-- questionnaire-list.component.html — Tailwind classes, Angular control flow -->
@if (store.loading()) {
  <app-loading-spinner />
} @else {
  <div class="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
    @for (q of store.questionnaires(); track q.id) {
      <div class="rounded-xl border border-gray-200 bg-white p-4 shadow-sm">
        <h3 class="text-lg font-semibold">{{ q.name }}</h3>
        <p class="text-sm text-gray-500">{{ q.questionCount }} questions</p>
        <a [routerLink]="['/questionnaires', q.id]"
           class="mt-2 inline-block text-sm text-indigo-600 hover:underline">
          View
        </a>
      </div>
    }
  </div>
}
```

---

### Feature: Home

| File | Description |
|---|---|
| `features/home/home.routes.ts` | `{ path: '', component: HomeComponent }` |
| `features/home/home.component.ts` | Standalone. Injects `AuthStore`, `LeaderboardStore`. On init: `leaderboardStore.loadTop5()`. Template reads `authStore.isAuthenticated()` signal to show/hide sign-in buttons. |
| `features/home/home.component.html` | Hero section with app description and **Download for Windows** button (URL from `environment.downloadUrl`). OAuth sign-in buttons (Google, GitHub, LinkedIn). Top-5 leaderboard preview table. |

### Feature: Login

| File | Description |
|---|---|
| `features/login/login.routes.ts` | `{ path: 'login', component: LoginComponent }` |
| `features/login/login.component.ts` | Standalone. Injects `AuthStore`. If already authenticated, redirects to `/dashboard`. Three buttons call `authStore.login(provider)`. |
| `features/login/login.component.html` | Centered card with Tailwind. Three OAuth buttons with provider icons. |

### Feature: Auth Callback

| File | Description |
|---|---|
| `features/auth-callback/auth-callback.routes.ts` | `{ path: 'auth/callback', component: AuthCallbackComponent }` |
| `features/auth-callback/auth-callback.component.ts` | Standalone. Injects `ActivatedRoute`, `TokenStorageService`, `AuthStore`, `Router`. On init: reads `access_token` + `refresh_token` from `queryParams` signal. Calls `tokenStorage.saveTokens(...)`, then `authStore.restoreSession()`. Navigates to `/dashboard`. Shows loading spinner while processing. |

### Feature: Dashboard

| File | Description |
|---|---|
| `features/dashboard/dashboard.routes.ts` | `{ path: 'dashboard', canActivate: [authGuard], component: DashboardComponent }` |
| `features/dashboard/dashboard.store.ts` | `signalStore`. State: `recentSessions: InterviewSessionDto[]`, `scoreSummary: UserScoreSummaryDto \| null`, `loading: boolean`. Injects `SessionsService` and `LeaderboardService`. Method `loadDashboard: rxMethod<void>` — calls both services in parallel with `forkJoin`, populates state. |
| `features/dashboard/dashboard.component.ts` | Standalone. Injects `AuthStore`, `DashboardStore`. On init: `dashboardStore.loadDashboard()`. |
| `features/dashboard/dashboard.component.html` | Welcome header with avatar + display name from `authStore.currentUser()`. Score summary card. Recent sessions list with `ScoreBadge` per row. Quick-link to `/questionnaires`. |

### Feature: Questionnaires

| File | Description |
|---|---|
| `features/questionnaires/questionnaires.routes.ts` | Parent route `questionnaires` with two children: `''` → `QuestionnaireListComponent`, `':id'` → `QuestionnaireDetailComponent`. Both guarded. |
| `features/questionnaires/questionnaires.service.ts` | `@Injectable({ providedIn: 'root' })`. `getAll(): Observable<QuestionnaireDto[]>` — `GET /questionnaires`. `getById(id: number): Observable<QuestionnaireDto>` — `GET /questionnaires/{id}`. `import(file: File, isPublic: boolean): Observable<QuestionnaireDto>` — `POST /questionnaires` multipart. `delete(id: number): Observable<void>` — `DELETE /questionnaires/{id}`. |
| `features/questionnaires/questionnaires.store.ts` | `signalStore({ providedIn: 'root' })`. State: `questionnaires`, `selected`, `loading`, `error`. Methods: `loadAll`, `loadById`, `import`, `remove` — all `rxMethod`. |
| `features/questionnaires/models/questionnaire.model.ts` | TypeScript interfaces: `QuestionnaireDto`, `QuestionDto`. Mirrors API response shapes. |
| `questionnaire-list/questionnaire-list.component.ts` | Standalone. Injects `QuestionnairesStore`, `AuthStore`. On init: `store.loadAll()`. `onImport(file, isPublic)` calls `store.import(...)`. `onDelete(id)` calls `store.remove(id)` (only shown for owned questionnaires: `q.createdByUserId === authStore.currentUser()?.id`). |
| `questionnaire-list/questionnaire-list.component.html` | Grid of questionnaire cards. File input + public/private toggle for upload. `@for` loop with `track q.id`. |
| `questionnaire-detail/questionnaire-detail.component.ts` | Standalone. Injects `QuestionnairesStore`, `ActivatedRoute`. On init: reads `id` from `route.params` signal, calls `store.loadById(id)`. |
| `questionnaire-detail/questionnaire-detail.component.html` | Questionnaire header. Expandable accordion for each question showing `questionText` and `idealAnswer`. `@for` over `store.selected()?.questions`. |

### Feature: Sessions

| File | Description |
|---|---|
| `features/sessions/sessions.routes.ts` | Parent route `sessions`: `''` → `SessionListComponent`, `':id'` → `SessionDetailComponent`. Both guarded. |
| `features/sessions/sessions.service.ts` | `@Injectable({ providedIn: 'root' })`. `getAll(): Observable<InterviewSessionDto[]>` — `GET /sessions`. `getById(id: number): Observable<InterviewSessionDto>` — `GET /sessions/{id}`. `delete(id: number): Observable<void>` — `DELETE /sessions/{id}`. |
| `features/sessions/sessions.store.ts` | `signalStore({ providedIn: 'root' })`. State: `sessions`, `selectedSession`, `loading`, `error`. Methods: `loadAll`, `loadById`, `remove` — all `rxMethod`. |
| `features/sessions/models/session.model.ts` | Interfaces: `InterviewSessionDto`, `SessionAnswerDto`, `FinalResultDto`. |
| `session-list/session-list.component.ts` | Standalone. Injects `SessionsStore`. On init: `store.loadAll()`. `onDelete(id)`: `store.remove(id)`. |
| `session-list/session-list.component.html` | Table with columns: Questionnaire, Date, Duration, Score, Classification (`ScoreBadge`). Row links to `/sessions/:id`. Delete button per row. |
| `session-detail/session-detail.component.ts` | Standalone. Injects `SessionsStore`, `ActivatedRoute`. On init: reads `id` from route params signal, calls `store.loadById(id)`. |
| `session-detail/session-detail.component.html` | Session header: questionnaire name, date, duration, final score, `ScoreBadge`. `MarkdownViewer` component renders `store.selectedSession()?.reportContent`. Per-question accordion with transcript, score, AI feedback, strengths, weaknesses. |

### Feature: Leaderboard

| File | Description |
|---|---|
| `features/leaderboard/leaderboard.routes.ts` | `{ path: 'leaderboard', component: LeaderboardComponent }` — public, no guard. |
| `features/leaderboard/leaderboard.service.ts` | `getTop(n = 50): Observable<LeaderboardEntryDto[]>` — `GET /scores/leaderboard?top={n}`. |
| `features/leaderboard/leaderboard.store.ts` | State: `entries`, `loading`, `error`. Method: `loadTop: rxMethod<number>`. |
| `features/leaderboard/models/leaderboard.model.ts` | Interfaces: `LeaderboardEntryDto`. |
| `features/leaderboard/leaderboard.component.ts` | Standalone. Injects `LeaderboardStore`. On init: `store.loadTop(50)`. |
| `features/leaderboard/leaderboard.component.html` | Ranked table: Rank, Avatar, Name, Best Score, Sessions, Average. Top 3 rows highlighted with gold/silver/bronze Tailwind classes. |

### Feature: Profile

| File | Description |
|---|---|
| `features/profile/profile.routes.ts` | `{ path: 'profile', canActivate: [authGuard], component: ProfileComponent }` |
| `features/profile/profile.service.ts` | `getMe(): Observable<UserDto>` — `GET /users/me`. `update(req: UpdateProfileRequest): Observable<UserDto>` — `PUT /users/me`. |
| `features/profile/profile.store.ts` | State: `profile: UserDto \| null`, `saving: boolean`, `error: string \| null`. Methods: `load: rxMethod<void>`, `save: rxMethod<UpdateProfileRequest>` — on success also calls `AuthStore.restoreSession()` to refresh name in nav. |
| `features/profile/profile.component.ts` | Standalone. Injects `ProfileStore`. Uses Angular reactive forms (`FormGroup`). On init: `store.load()`; patches form when `store.profile()` signal changes using `effect()`. On submit: `store.save(formValue)`. |
| `features/profile/profile.component.html` | Avatar image. `displayName` input. Save button disabled while `store.saving()`. Tailwind card layout. |

---

### Shared

| File | Description |
|---|---|
| `shared/models/user.model.ts` | `UserDto` interface shared by `AuthStore` and `ProfileStore`. |
| `shared/components/loading-spinner/loading-spinner.component.ts` | Standalone. Animated Tailwind spinner. Accepts optional `[message]` input signal. |
| `shared/components/score-badge/score-badge.component.ts` | Standalone. Input signal `[classification]: 'Approved' \| 'ApprovedWithReservations' \| 'Failed'`. Renders Tailwind pill: green / orange / red. |
| `shared/components/markdown-viewer/markdown-viewer.component.ts` | Standalone. Input signal `[content]: string`. Uses `ngx-markdown` to render Markdown to HTML safely. |

---

### Root Configuration

| File | Description |
|---|---|
| `app.routes.ts` | Lazy-loads each feature's routes: `{ path: '', loadChildren: () => import('./features/home/home.routes') }`, etc. |
| `app.config.ts` | `provideRouter(routes)`, `provideHttpClient(withInterceptors([authInterceptor, tokenRefreshInterceptor]))`, `provideAnimationsAsync()`. |
| `app.component.ts` | Standalone shell. Injects `AuthStore`. Calls `authStore.restoreSession()` on init. Renders `<router-outlet>` with Tailwind nav bar. |
| `styles.scss` | `@tailwind base; @tailwind components; @tailwind utilities;` — nothing else. |
| `tailwind.config.js` | `content: ['./src/**/*.{html,ts}']`. Theme: extends default with brand colours. |
| `postcss.config.js` | `plugins: { tailwindcss: {}, autoprefixer: {} }`. |
| `environments/environment.ts` | `{ apiBaseUrl: 'http://localhost:8080', downloadUrl: 'https://github.com/...' }` |
| `environments/environment.prod.ts` | `{ apiBaseUrl: 'https://api.inquisitorai.com', downloadUrl: '...' }` |
| `nginx.conf` | Serves `dist/inquisitor-ai/browser/` on port 80. `try_files $uri $uri/ /index.html` for client-side routing. |
| `Dockerfile` | Stage 1: `node:22-alpine`, runs `npm ci && ng build --configuration production`. Stage 2: `nginx:alpine`, copies `dist/` + `nginx.conf`. Exposes port 80. |

---

## Project: InquisitorAI.Tests

xUnit tests mirroring the Features structure. References `InquisitorAI.Features` and `InquisitorAI.Infrastructure`.

```
InquisitorAI.Tests/
├── Auth/
│   └── Commands/
│       ├── IssueTokensHandlerTests.cs
│       └── RefreshAccessTokenHandlerTests.cs
├── Users/
│   ├── Commands/
│   │   └── UpdateUserProfileHandlerTests.cs
│   └── Queries/
│       └── GetCurrentUserHandlerTests.cs
├── Questionnaires/
│   ├── Commands/
│   │   ├── ImportQuestionnaireHandlerTests.cs
│   │   └── DeleteQuestionnaireHandlerTests.cs
│   ├── Queries/
│   │   └── GetQuestionnairesHandlerTests.cs
│   └── Services/
│       └── MarkdownParserServiceTests.cs
├── InterviewSessions/
│   ├── Commands/
│   │   ├── StartInterviewSessionHandlerTests.cs
│   │   ├── SubmitAnswerHandlerTests.cs
│   │   └── FinishInterviewSessionHandlerTests.cs
│   └── Queries/
│       └── GetInterviewSessionsHandlerTests.cs
└── Scores/
    └── Queries/
        └── GetLeaderboardHandlerTests.cs
```

| Test File | What It Tests |
|---|---|
| `Auth/Commands/IssueTokensHandlerTests.cs` | New user created on first login. Existing user updated on subsequent login. Refresh token stored hashed. |
| `Auth/Commands/RefreshAccessTokenHandlerTests.cs` | Valid token returns new token pair. Expired token adds error. Revoked token adds error. |
| `Questionnaires/Commands/ImportQuestionnaireHandlerTests.cs` | Mocks `IMarkdownParserService`. Verifies questionnaire + questions persisted. Tests empty question list adds error. |
| `Questionnaires/Services/MarkdownParserServiceTests.cs` | Parses valid Markdown string. Tests all field types. Tests multi-line ideal answer. Tests missing H1 and missing required fields throw `FormatException`. |
| `InterviewSessions/Commands/SubmitAnswerHandlerTests.cs` | Mocks `IAiEvaluationService`. Verifies `SessionAnswer` persisted with correct score and feedback. Validates session ownership check. |
| `InterviewSessions/Commands/FinishInterviewSessionHandlerTests.cs` | Seeds session with answers. Verifies `FinalScore` average (2dp). Tests all three classification boundary conditions. Mocks `IReportGeneratorService`. |
| `Scores/Queries/GetLeaderboardHandlerTests.cs` | Seeds multiple users with sessions. Verifies ranking order and aggregation correctness. |

---

## PostgreSQL Database Schema

All tables use snake_case with prefix `inq_` to avoid conflicts with existing tables. `DateTimeOffset` values stored as `timestamptz`. `RowVersion` as `xid` or `bytea` (EF Core handles via `IsRowVersion()` on PostgreSQL with `UseXminAsConcurrencyToken()`).

### Table: `inq_users`

| Column | Type | Constraints |
|---|---|---|
| `id` | BIGSERIAL | PRIMARY KEY |
| `provider` | VARCHAR(20) | NOT NULL · 'Google' \| 'LinkedIn' \| 'GitHub' |
| `external_id` | VARCHAR(255) | NOT NULL |
| `email` | VARCHAR(255) | NOT NULL |
| `display_name` | VARCHAR(200) | NOT NULL |
| `avatar_url` | TEXT | NULL |
| `created_at` | TIMESTAMPTZ | NOT NULL |
| `updated_at` | TIMESTAMPTZ | NOT NULL |
| `xmin` | xid | concurrency token (built-in, no column needed) |

**Indexes:** UNIQUE(`provider`, `external_id`), UNIQUE(`email`)

### Table: `inq_refresh_tokens`

| Column | Type | Constraints |
|---|---|---|
| `id` | BIGSERIAL | PRIMARY KEY |
| `user_id` | BIGINT | NOT NULL · FK → inq_users(id) ON DELETE CASCADE |
| `token_hash` | VARCHAR(128) | NOT NULL |
| `expires_at` | TIMESTAMPTZ | NOT NULL |
| `revoked_at` | TIMESTAMPTZ | NULL |
| `created_at` | TIMESTAMPTZ | NOT NULL |
| `updated_at` | TIMESTAMPTZ | NOT NULL |

**Indexes:** INDEX(`token_hash`), INDEX(`user_id`)

### Table: `inq_questionnaires`

| Column | Type | Constraints |
|---|---|---|
| `id` | BIGSERIAL | PRIMARY KEY |
| `name` | VARCHAR(300) | NOT NULL |
| `created_by_user_id` | BIGINT | NOT NULL · FK → inq_users(id) |
| `is_public` | BOOLEAN | NOT NULL · DEFAULT FALSE |
| `created_at` | TIMESTAMPTZ | NOT NULL |
| `updated_at` | TIMESTAMPTZ | NOT NULL |

**Indexes:** INDEX(`created_by_user_id`), INDEX(`is_public`)

### Table: `inq_questions`

| Column | Type | Constraints |
|---|---|---|
| `id` | BIGSERIAL | PRIMARY KEY |
| `questionnaire_id` | BIGINT | NOT NULL · FK → inq_questionnaires(id) ON DELETE CASCADE |
| `order_index` | INTEGER | NOT NULL |
| `category` | VARCHAR(100) | NULL |
| `difficulty` | VARCHAR(10) | NULL · 'Easy' \| 'Medium' \| 'Hard' |
| `question_text` | TEXT | NOT NULL |
| `ideal_answer` | TEXT | NOT NULL |
| `created_at` | TIMESTAMPTZ | NOT NULL |
| `updated_at` | TIMESTAMPTZ | NOT NULL |

**Indexes:** INDEX(`questionnaire_id`)

### Table: `inq_interview_sessions`

| Column | Type | Constraints |
|---|---|---|
| `id` | BIGSERIAL | PRIMARY KEY |
| `user_id` | BIGINT | NOT NULL · FK → inq_users(id) |
| `questionnaire_id` | BIGINT | NOT NULL · FK → inq_questionnaires(id) |
| `started_at` | TIMESTAMPTZ | NOT NULL |
| `ended_at` | TIMESTAMPTZ | NULL |
| `duration_seconds` | INTEGER | NULL |
| `final_score` | NUMERIC(4,2) | NULL |
| `classification` | VARCHAR(30) | NULL · 'Approved' \| 'ApprovedWithReservations' \| 'Failed' |
| `report_content` | TEXT | NULL |
| `created_at` | TIMESTAMPTZ | NOT NULL |
| `updated_at` | TIMESTAMPTZ | NOT NULL |

**Indexes:** INDEX(`user_id`), INDEX(`questionnaire_id`), INDEX(`final_score`)

### Table: `inq_session_answers`

| Column | Type | Constraints |
|---|---|---|
| `id` | BIGSERIAL | PRIMARY KEY |
| `session_id` | BIGINT | NOT NULL · FK → inq_interview_sessions(id) ON DELETE CASCADE |
| `question_id` | BIGINT | NOT NULL · FK → inq_questions(id) |
| `transcript` | TEXT | NULL |
| `score` | NUMERIC(4,2) | NULL · 0.00–10.00 |
| `ai_feedback` | TEXT | NULL |
| `strengths` | TEXT | NULL |
| `weaknesses` | TEXT | NULL |
| `improvement_suggestions` | TEXT | NULL |
| `created_at` | TIMESTAMPTZ | NOT NULL |
| `updated_at` | TIMESTAMPTZ | NOT NULL |

**Indexes:** INDEX(`session_id`)

---

## API Endpoint Contracts

### Auth

| Method | Path | Auth | Request | Response |
|---|---|---|---|---|
| `GET` | `/auth/google` | None | `?redirect_uri={loopback}` (optional, native flow) | 302 → Google |
| `GET` | `/auth/google/callback` | None | OAuth code | 302 → portal or loopback |
| `GET` | `/auth/github` | None | same | 302 → GitHub |
| `GET` | `/auth/github/callback` | None | OAuth code | 302 |
| `GET` | `/auth/linkedin` | None | same | 302 → LinkedIn |
| `GET` | `/auth/linkedin/callback` | None | OAuth code | 302 |
| `POST` | `/auth/refresh` | None | `{ refreshToken }` | `TokenResponseDto` |
| `POST` | `/auth/logout` | JWT | — | 204 |

### Users

| Method | Path | Auth | Request | Response |
|---|---|---|---|---|
| `GET` | `/users/me` | JWT | — | `UserDto` |
| `PUT` | `/users/me` | JWT | `UpdateProfileRequest` | `UserDto` |

### Questionnaires

| Method | Path | Auth | Request | Response |
|---|---|---|---|---|
| `GET` | `/questionnaires` | Optional | — | `QuestionnaireDto[]` |
| `POST` | `/questionnaires` | JWT | `multipart/form-data`: `file` (.md), `isPublic` (bool) | `QuestionnaireDto` 201 |
| `GET` | `/questionnaires/{id}` | Optional | — | `QuestionnaireDto` with `Questions[]` |
| `DELETE` | `/questionnaires/{id}` | JWT | — | 204 |

### Interview Sessions

| Method | Path | Auth | Request | Response |
|---|---|---|---|---|
| `POST` | `/sessions` | JWT | `StartSessionRequest` | `InterviewSessionDto` 201 |
| `POST` | `/sessions/{id}/answers` | JWT | `SubmitAnswerRequest` | `SessionAnswerDto` 201 |
| `POST` | `/sessions/{id}/finish` | JWT | — | `FinalResultDto` |
| `GET` | `/sessions` | JWT | — | `InterviewSessionDto[]` |
| `GET` | `/sessions/{id}` | JWT | — | `InterviewSessionDto` |
| `DELETE` | `/sessions/{id}` | JWT | — | 204 |

### Scores

| Method | Path | Auth | Request | Response |
|---|---|---|---|---|
| `GET` | `/scores/leaderboard` | None | `?top=50` | `LeaderboardEntryDto[]` |
| `GET` | `/scores/me` | JWT | — | `UserScoreSummaryDto` |

---

## Authentication Flows

### Web Portal Flow (Blazor WASM)

```
1. User clicks "Sign in with Google" on /login
2. Browser navigates to GET /auth/google  (no redirect_uri param)
3. API redirects to Google OAuth consent screen
4. Google redirects to GET /auth/google/callback?code=...
5. ASP.NET Core OAuth middleware exchanges code for profile
6. OAuthCallbackEndpoint calls IssueTokensCommand → creates/updates User + RefreshToken
7. API redirects to {WebPortalUrl}/auth/callback?access_token=...&refresh_token=...
8. AuthCallback.razor saves tokens to localStorage
9. User is redirected to /dashboard
```

### Native App Flow (WinForms)

```
1. User clicks "Sign in with Google" in LoginForm
2. OAuthHandler picks a random available port (e.g. 54321)
3. OAuthHandler starts HttpListener on http://localhost:54321/callback
4. OAuthHandler opens system browser to:
   GET /auth/google?redirect_uri=http://localhost:54321/callback
5. API validates redirect_uri is a loopback address
6. Google OAuth flow proceeds normally
7. OAuthCallbackEndpoint detects redirect_uri param → native flow
8. API redirects to http://localhost:54321/callback?access_token=...&refresh_token=...
9. HttpListener captures the request, extracts tokens
10. OAuthHandler saves tokens via WindowsCredentialTokenStore
11. HttpListener stops, browser can be closed
12. LoginForm closes, MainForm opens
```

### Token Refresh

- Access token: 15 minutes, JWT signed with HMAC-SHA256 secret from `.env`
- Refresh token: 30 days, stored as SHA-256 hash in `refresh_tokens` table
- On 401: `ApiClient` (both WinForms and Blazor) automatically calls `POST /auth/refresh` and retries once
- `RefreshAccessToken` handler revokes the old refresh token and issues a new pair (rotation)

---

## JWT Configuration

Claims included in access token:
- `sub` — user `Id` (long, as string)
- `email` — user email
- `name` — display name
- `provider` — OAuth provider string

Configuration keys in `.env`:
```
JWT_SECRET=<min 32 chars>
JWT_ISSUER=https://api.inquisitorai.com
JWT_AUDIENCE=inquisitorai-clients
JWT_EXPIRY_MINUTES=15
REFRESH_TOKEN_EXPIRY_DAYS=30
WEB_PORTAL_URL=https://inquisitorai.com
```

---

## OAuth Provider Configuration (`.env`)

```
GOOGLE_CLIENT_ID=...
GOOGLE_CLIENT_SECRET=...
GITHUB_CLIENT_ID=...
GITHUB_CLIENT_SECRET=...
LINKEDIN_CLIENT_ID=...
LINKEDIN_CLIENT_SECRET=...

CONNECTION_STRING=Host=db;Database=inquisitor;Username=postgres;Password=${DB_PASSWORD}
DB_PASSWORD=...
ANTHROPIC_API_KEY=...
CORS_ORIGINS=https://inquisitorai.com,http://localhost:5173
```

---

## Interview Markdown File Format

Uploaded via `POST /questionnaires` as a `.md` file. Parsed server-side by `MarkdownParserService`.

```markdown
# .NET Senior Developer Interview

## Question 1

**Category:** C#
**Difficulty:** Medium
**Question:** What is the difference between `class` and `struct` in C#?
**Ideal Answer:** Classes are reference types allocated on the heap; structs are value types
allocated on the stack. Classes support inheritance; structs do not. Structs are preferable
for small, immutable data with value semantics.

## Question 2

**Category:** Architecture
**Difficulty:** Hard
**Question:** What is eventual consistency and when would you accept it?
**Ideal Answer:** Eventual consistency is a consistency model in distributed systems where
replicas may temporarily diverge but converge given no new updates. Acceptable when high
availability is prioritised over strict consistency — e.g. shopping carts, activity feeds.
```

### Parsing Rules

- `# H1` line → interview/questionnaire `Name` (required)
- `## H2` line → question block delimiter (content ignored)
- `**Question:**` → required field
- `**Ideal Answer:**` → required field; may span multiple lines until next `**Key:**` or `##`
- `**Category:**` → optional free text
- `**Difficulty:**` → optional; `Easy` | `Medium` | `Hard` (case-insensitive, defaults `Medium`)
- At least one question required; otherwise `FormatException`

---

## Classification Rules

Applied in `FinishInterviewSessionHandler`.

| Classification | Condition |
|---|---|
| `Approved` | `FinalScore >= 8.0` |
| `ApprovedWithReservations` | `FinalScore >= 6.5 AND < 8.0` |
| `Failed` | `FinalScore < 6.5` |

`FinalScore` = arithmetic mean of all `inq_session_answers.score` values, rounded to 2 decimal places.

---

## Final Report Format

Generated by `MarkdownReportGeneratorService`. Stored in `interview_sessions.report_content`.

```markdown
# Interview Report — .NET Senior Developer Interview

**Date:** 2026-03-20
**Duration:** 00:18:42
**Final Score:** 7.33 / 10
**Classification:** Approved with Reservations

---

## Overall Summary

**Strengths:** Strong C# fundamentals and SQL knowledge.
**Improvement Areas:** Distributed systems concepts need deepening.

---

## Question 1

**Category:** C# · **Difficulty:** Medium
**Question:** What is the difference between `class` and `struct` in C#?

**Your Answer:** Classes are on the heap, structs on the stack. Structs are value types.

**Ideal Answer:** Classes are reference types on the heap; structs are value types on the stack.
Classes support inheritance; structs do not.

**Score:** 7.5 / 10
**AI Feedback:** Good foundational answer. Missing immutability preference and equality implications.
**Strengths:** Identified heap/stack allocation correctly.
**Weaknesses:** Missed IEquatable<T> and value semantics discussion.
**Improvement Suggestions:** Review .NET guidance on choosing struct vs class.
```

---

## AI Evaluation Prompt Contract

Sent by `ClaudeAiEvaluationService` to Anthropic Claude API. Returns only JSON.

```
You are a senior technical interviewer evaluating a candidate's verbal answer.

Question: {QuestionText}
Ideal Answer: {IdealAnswer}
Candidate's Answer (transcribed from speech): {Transcript}

Evaluate on: technical alignment, concept coverage, clarity, accuracy, depth.

Return ONLY this JSON:
{
  "score": <0.0–10.0, one decimal>,
  "summary": "<2-3 sentence evaluation>",
  "strengths": "<bullet points>",
  "weaknesses": "<bullet points>",
  "improvement_suggestions": "<actionable advice>"
}

Prioritise real technical understanding over textual similarity. No text outside the JSON.
```

---

## NuGet Package Reference

| Project | Package | Purpose |
|---|---|---|
| Features | `FluentValidation` | Command validators |
| Features | `Dapper` | Read query handlers |
| Api | `Microsoft.AspNetCore.Authentication.JwtBearer` | JWT validation |
| Api | `Microsoft.AspNetCore.Authentication.Google` | Google OAuth |
| Api | `AspNet.Security.OAuth.GitHub` | GitHub OAuth |
| Api | `AspNet.Security.OAuth.LinkedIn` | LinkedIn OAuth |
| Api | `Swashbuckle.AspNetCore` | OpenAPI / Swagger UI |
| Api | `DotNetEnv` | `.env` file loading |
| Api | `Microsoft.AspNetCore.RateLimiting` | Rate limiting middleware |
| Api | `Microsoft.Extensions.Diagnostics.HealthChecks` | Health check endpoints |
| Infrastructure | `Microsoft.EntityFrameworkCore` | ORM base |
| Infrastructure | `Npgsql.EntityFrameworkCore.PostgreSQL` | PostgreSQL EF provider |
| Infrastructure | `Microsoft.EntityFrameworkCore.Design` | EF CLI tooling |
| Infrastructure | `Npgsql` | `IDbConnection` for Dapper |
| Infrastructure | `Serilog` + `Serilog.Sinks.File` | Structured file logging |
| Infrastructure | `System.IdentityModel.Tokens.Jwt` | JWT generation / validation |
| UI | `Microsoft.Extensions.DependencyInjection` | DI container |
| UI | `Microsoft.Extensions.Configuration.Json` | `appsettings.json` |
| UI | `System.Speech` _(Windows built-in)_ | TTS |
| UI | `NAudio` | Microphone recording |
| UI | `CredentialManagement` | Windows Credential Manager (token storage) |
| Web (npm) | `@angular/core` + `@angular/common` + `@angular/router` + `@angular/forms` + `@angular/animations` | Angular (latest) framework |
| Web (npm) | `@ngrx/signals` | `signalStore()`, `withState`, `withMethods`, `withComputed`, `rxMethod`, `tapResponse`, `patchState` |
| Web (npm) | `tailwindcss` + `postcss` + `autoprefixer` | Utility-first CSS |
| Web (npm) | `ngx-markdown` | Render Markdown report content to HTML |
| Web (npm) | `jwt-decode` | Decode JWT claims client-side to restore session |
| Tests | `xunit` | Test framework |
| Tests | `FluentAssertions` | Assertions |
| Tests | `Moq` | Mocking |
| Tests | `Bogus` | Fake data |
| Tests | `Microsoft.EntityFrameworkCore.InMemory` | In-memory DB for command tests |

---

## Docker Compose

```yaml
services:
  api:
    build:
      context: .
      dockerfile: InquisitorAI.Api/Dockerfile
    ports:
      - "8080:8080"
    env_file: .env
    depends_on:
      db:
        condition: service_healthy
    volumes:
      - logs:/logs

  web:
    build:
      context: ./InquisitorAI.Web
      dockerfile: Dockerfile
    ports:
      - "4200:80"
    depends_on:
      - api

  db:
    image: postgres:16
    environment:
      POSTGRES_DB: inquisitor
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: ${DB_PASSWORD}
    ports:
      - "5432:5432"
    volumes:
      - pgdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 10

volumes:
  pgdata:
  logs:
```

---

## Example Questionnaire File

Located at: `samples/dotnet-interview.md`

Ships with the solution. Contains 5 questions covering C# fundamentals, architecture, SQL, async/await, and distributed systems. Used for import demo and manual testing.
