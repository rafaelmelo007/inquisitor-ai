# UI Screens — Inquisitor AI

---

## WinForms Desktop (InquisitorAI.UI)

### 1. Login Screen

A simple window with three large buttons stacked vertically: **Sign in with Google**, **Sign in with GitHub**, **Sign in with LinkedIn**. Clicking any button opens the system browser for OAuth, while the app waits for a loopback callback with tokens. On success, this form closes and the main form opens.

### 2. Main Screen (Home)

The primary hub after login. Contains:

- A **ListView** showing all available questionnaires (the user's own + public ones), loaded from the API on open.
- Buttons along the top or side: **Upload Questionnaire** (opens a file dialog to select a `.md` file), **Start Interview** (launches the interview form for the selected questionnaire), **History**, **Profile**, and **Logout**.

### 3. Interview Screen

The most interactive screen. On open it calls the API to start a new session, then walks through questions one at a time:

- **Progress bar** at the top showing question X of Y.
- Current question's **category** and **difficulty** labels.
- The **question text** displayed prominently.
- Action buttons: **Listen** (reads the question aloud via local TTS), **Record** (starts microphone recording — a **red pulsing circle indicator** appears), **Stop** (stops recording, sends audio to Whisper for transcription, then submits the transcript to the API).
- After submission, the screen shows the **transcript**, **score**, and **AI feedback** for that answer.
- A **Next Question** button advances to the next question.
- On the last question, finishing calls the API and automatically opens the Result screen.
- A semi-transparent **"AI is analyzing..." overlay** with a marquee progress bar appears during API calls.

### 4. Result Screen

Shown after completing an interview:

- Large **final score** number.
- **Classification badge** colour-coded: green for Approved, orange for Approved with Reservations, red for Failed.
- Lists of **strengths** and **improvement areas**.
- A **View Report** button that exports the Markdown report to a temp file and opens it in the OS default app.

### 5. History Screen

A **DataGridView** (table) listing all past interview sessions with columns: Questionnaire Name, Date, Duration, Score, Classification. Each row has **Open Report** and **Delete** buttons.

### 6. Profile Screen

Shows the current user's info from the API (avatar, email, display name, provider). The **Display Name** field is editable, with a **Save** button that calls the update API.

---

## Angular Web Portal (InquisitorAI.Web)

All screens use **Tailwind CSS** utility classes with a consistent nav bar shell. The nav bar shows the user's avatar + name when authenticated, with links to Dashboard, Questionnaires, Sessions, Leaderboard, and Profile.

### 1. Home Page (`/`)

- **Hero section** with app description and a prominent **"Download for Windows"** button.
- Three **OAuth sign-in buttons** (Google, GitHub, LinkedIn) — hidden if already logged in.
- A **Top-5 leaderboard preview table** showing the highest-scoring users.

### 2. Login Page (`/login`)

- A **centered Tailwind card** with three OAuth buttons (with provider icons).
- If already authenticated, auto-redirects to `/dashboard`.

### 3. Auth Callback (`/auth/callback`)

- Not a visible screen — just a **loading spinner** while it extracts tokens from the URL query params, saves them, restores the session, and redirects to `/dashboard`.

### 4. Dashboard (`/dashboard`) — requires auth

- **Welcome header** with the user's avatar and display name.
- **Score summary card** showing total sessions, average score, best score, last session date.
- **Recent sessions list** with a `ScoreBadge` (coloured pill) per row.
- **Quick-link** to the questionnaires page.

### 5. Questionnaire List (`/questionnaires`) — requires auth

- A **responsive grid** (1 col on mobile, 2 on tablet, 3 on desktop) of **questionnaire cards**. Each card shows the name, question count, and a "View" link.
- **Upload section**: a file input for `.md` files with a public/private toggle.
- **Delete button** shown only on questionnaires the user owns.

### 6. Questionnaire Detail (`/questionnaires/:id`) — requires auth

- **Header** with the questionnaire name and metadata.
- An **expandable accordion** for each question, showing the question text and ideal answer.

### 7. Session List (`/sessions`) — requires auth

- A **table** with columns: Questionnaire, Date, Duration, Score, Classification (rendered as a coloured `ScoreBadge` pill — green/orange/red).
- Each row links to the detail view. Each row has a **Delete** button.

### 8. Session Detail (`/sessions/:id`) — requires auth

- **Session header**: questionnaire name, date, duration, final score, classification badge.
- **Markdown report** rendered via `ngx-markdown` in a `MarkdownViewer` component.
- **Per-question accordion**: each panel shows the transcript, score, AI feedback, strengths, weaknesses, and improvement suggestions.

### 9. Leaderboard (`/leaderboard`) — public, no auth

- A **ranked table**: Rank, Avatar, Name, Best Score, Sessions, Average Score.
- **Top 3 rows highlighted** with gold, silver, and bronze styling.

### 10. Profile (`/profile`) — requires auth

- **Avatar image** display.
- Editable **Display Name** input (reactive form).
- **Save** button, disabled while saving is in progress.
- Clean Tailwind card layout.

---

## Shared Components (Angular)

- **LoadingSpinner**: animated Tailwind spinner with optional message text.
- **ScoreBadge**: coloured pill — green (Approved), orange (Approved with Reservations), red (Failed).
- **MarkdownViewer**: renders Markdown content to HTML safely using `ngx-markdown`.
