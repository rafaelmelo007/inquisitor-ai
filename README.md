# Inquisitor AI

Inquisitor AI is an AI-powered technical interview simulator that asks questions out loud, listens to your answers, evaluates them with Claude AI, and generates a detailed performance report.

## Architecture

```
┌─────────────────────┐     ┌──────────────────────┐
│  InquisitorAI.UI    │     │  InquisitorAI.Web     │
│  WinForms Desktop   │     │  Angular 21 SPA       │
│  (thin HTTP client) │     │  (web portal)         │
└────────┬────────────┘     └──────────┬────────────┘
         │  JWT Bearer                 │  JWT Bearer
         ▼                             ▼
┌─────────────────────────────────────────────────────┐
│              InquisitorAI.Api                        │
│          .NET 10 Minimal API                         │
├─────────────────────────────────────────────────────┤
│              InquisitorAI.Features                   │
│   Auth · Users · Questionnaires · Sessions · Scores  │
├─────────────────────────────────────────────────────┤
│           InquisitorAI.Infrastructure                │
│      EF Core · PostgreSQL · External Services        │
└─────────────────────────────────────────────────────┘
```

## Projects

| Project | Description |
|---------|-------------|
| `InquisitorAI.Features` | Vertical Slice business logic — domain, CQRS handlers, DTOs, validators, endpoints |
| `InquisitorAI.Api` | .NET 10 Minimal API host |
| `InquisitorAI.Infrastructure` | EF Core, PostgreSQL, JWT, Claude AI, Markdown parser implementations |
| `InquisitorAI.UI` | WinForms desktop client with TTS, audio recording, and Whisper transcription |
| `InquisitorAI.Web` | Angular 21 SPA with Tailwind CSS, NgRx SignalStore |
| `InquisitorAI.Tests` | xUnit tests with FluentAssertions, Moq, Bogus |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 22+](https://nodejs.org/) (for the Angular web portal)
- [Docker & Docker Compose](https://docs.docker.com/get-docker/) (for containerized setup)
- PostgreSQL 17 (if running locally without Docker)

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/your-org/inquisitor-ai.git
cd inquisitor-ai
```

### 2. Configure environment variables

Copy the `.env` file and fill in your values:

```bash
cp .env .env.local   # optional — .env is used directly by the app
```

Edit `.env` with your actual credentials:

```env
# Database
ConnectionStrings__Default=Host=localhost;Port=5432;Database=inquisitor_ai;Username=inquisitor;Password=YOUR_SECURE_PASSWORD
DB_PASSWORD=YOUR_SECURE_PASSWORD

# JWT — use a long random string (64+ characters)
Jwt__Secret=YOUR_LONG_RANDOM_SECRET_KEY_HERE_AT_LEAST_64_CHARACTERS
Jwt__Issuer=InquisitorAI
Jwt__Audience=InquisitorAI

# OAuth — Google (https://console.cloud.google.com/apis/credentials)
Google__ClientId=your-google-client-id.apps.googleusercontent.com
Google__ClientSecret=your-google-client-secret

# OAuth — GitHub (https://github.com/settings/developers)
GitHub__ClientId=your-github-client-id
GitHub__ClientSecret=your-github-client-secret

# OAuth — LinkedIn (https://www.linkedin.com/developers/apps)
LinkedIn__ClientId=your-linkedin-client-id
LinkedIn__ClientSecret=your-linkedin-client-secret

# AI Providers
Anthropic__ApiKey=sk-ant-your-anthropic-api-key
OpenAi__ApiKey=sk-your-openai-api-key

# CORS
Cors__AllowedOrigins=http://localhost:4200

# Web Portal URL (used for OAuth callback redirects)
WebPortalUrl=http://localhost:4200
```

#### Required API keys

| Key | Purpose | Where to get it |
|-----|---------|-----------------|
| `Anthropic__ApiKey` | AI evaluation of interview answers (Claude) | [console.anthropic.com](https://console.anthropic.com/) |
| `OpenAi__ApiKey` | Speech-to-text transcription (Whisper) — desktop app only | [platform.openai.com](https://platform.openai.com/) |
| `Google__ClientId/Secret` | Google OAuth sign-in | [Google Cloud Console](https://console.cloud.google.com/apis/credentials) |
| `GitHub__ClientId/Secret` | GitHub OAuth sign-in | [GitHub Developer Settings](https://github.com/settings/developers) |
| `LinkedIn__ClientId/Secret` | LinkedIn OAuth sign-in | [LinkedIn Developer Portal](https://www.linkedin.com/developers/apps) |

#### OAuth callback URLs

When configuring OAuth providers, set these callback URLs:

| Provider | Callback URL |
|----------|-------------|
| Google | `http://localhost:8080/auth/google/callback` |
| GitHub | `http://localhost:8080/auth/github/callback` |
| LinkedIn | `http://localhost:8080/auth/linkedin/callback` |

---

## Running with Docker (recommended)

The easiest way to run the full stack:

```bash
# Make sure .env is configured, then:
docker compose up --build
```

This starts:
- **API** at `http://localhost:8080`
- **Web Portal** at `http://localhost:4200`
- **PostgreSQL** at `localhost:5432`

EF Core migrations run automatically on API startup.

To stop:

```bash
docker compose down
```

To reset the database:

```bash
docker compose down -v   # removes volumes including pgdata
docker compose up --build
```

---

## Running Locally (without Docker)

### Database

Start a PostgreSQL 17 instance and create the database:

```sql
CREATE DATABASE inquisitor_ai;
CREATE USER inquisitor WITH PASSWORD 'your-password';
GRANT ALL PRIVILEGES ON DATABASE inquisitor_ai TO inquisitor;
```

### API

```bash
cd InquisitorAI.Api
dotnet run
```

The API starts at `http://localhost:8080`. Migrations are applied automatically on startup.

#### Health checks

- Readiness: `http://localhost:8080/health/ready`
- Liveness: `http://localhost:8080/health/live`
- Swagger: `http://localhost:8080/swagger` (development only)

### Web Portal

```bash
cd InquisitorAI.Web
npm install
npx ng serve
```

The web portal starts at `http://localhost:4200`.

### Desktop App (Windows only)

```bash
cd InquisitorAI.UI
dotnet run
```

The desktop app connects to the API at the URL configured in `InquisitorAI.UI/appsettings.json`:

```json
{
  "ApiBaseUrl": "http://localhost:8080"
}
```

The desktop app requires:
- A working microphone (for recording answers)
- Windows TTS voices installed (for reading questions aloud)
- An OpenAI API key in the `.env` or stored in Windows Credential Manager (for Whisper transcription)

---

## Running Tests

```bash
dotnet test
```

Tests use:
- **xUnit** as the test framework
- **FluentAssertions** for assertions
- **Moq** for mocking
- **Bogus** for fake data generation
- **EF Core InMemory** for command handler tests

---

## Project Structure

```
inquisitor-ai/
├── InquisitorAI.Features/          # Business logic (Vertical Slice Architecture)
│   ├── Shared/                     # IHandler, AppDbContext, NotificationHandler
│   ├── Auth/                       # OAuth + JWT token management
│   ├── Users/                      # User profiles
│   ├── Questionnaires/             # Import/manage markdown questionnaires
│   ├── InterviewSessions/          # Interview flow + AI evaluation
│   └── Scores/                     # Leaderboard + user score summaries
├── InquisitorAI.Api/               # Minimal API host
├── InquisitorAI.Infrastructure/    # Service implementations
├── InquisitorAI.UI/                # WinForms desktop client
├── InquisitorAI.Web/               # Angular 19 web portal
├── InquisitorAI.Tests/             # Unit tests
├── samples/                        # Sample questionnaire files
├── docker-compose.yml
├── .env
└── InquisitorAI.sln
```

---

## Sample Questionnaire

A sample `.NET interview` questionnaire is included at `samples/dotnet-interview.md`. Upload it through the web portal or desktop app to get started.

Questionnaire markdown format:

```markdown
# My Questionnaire Name

## 1

**Category:** Topic Name
**Difficulty:** Easy | Medium | Hard
**Question:** Your question text here?
**Ideal Answer:** The expected answer that the AI will compare against.

## 2

**Category:** Another Topic
**Difficulty:** Medium
**Question:** Another question?
**Ideal Answer:** Another ideal answer.
```

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend API | .NET 10, Minimal API, EF Core, Dapper, PostgreSQL |
| Authentication | JWT Bearer + OAuth (Google, GitHub, LinkedIn) |
| AI Evaluation | Anthropic Claude API |
| Speech-to-Text | OpenAI Whisper API (desktop only) |
| Text-to-Speech | Windows TTS (desktop only) |
| Web Frontend | Angular 21, Tailwind CSS, NgRx SignalStore |
| Desktop Client | WinForms, NAudio |
| Testing | xUnit, FluentAssertions, Moq, Bogus |
| Containerization | Docker, Docker Compose, Nginx |

## License

See [LICENSE](LICENSE) for details.
