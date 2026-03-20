PROMPT — Inquisitor AI



Create a desktop application in C# using Windows Forms and .NET 10

called: “Inquisitor AI”.



The application is an AI-powered technical interview simulator. The goal

is to allow developers to practice technical interviews by answering

questions out loud and receiving automatic evaluation.



The experience must be simple, fast, and pleasant, similar to a real

technical interview.



OBJECTIVE The user imports a Markdown (.md) file containing technical

questions and ideal answers.



The application then conducts a simulated interview:



1\.  Reads the question aloud

2\.  The user answers using the microphone

3\.  AI transcribes the answer

4\.  AI analyzes the answer compared with the ideal answer

5\.  The response receives a score from 0 to 10

6\.  At the end of the interview the system calculates a final score



The system then classifies the result as: - Approved - Approved with

reservations - Failed



Every interview generates a final Markdown report. The application must

also maintain a history of past interviews.



USER EXPERIENCE FLOW



1 — Import questions The user imports a .md file containing questions.



2 — Start interview The application automatically conducts the

interview.



3 — Answer by voice The user answers using the microphone.



4 — Receive evaluation The AI analyzes the response and shows a score.



5 — Final result At the end the user receives an overall score.



6 — Report The application generates a report automatically.



7 — History All interviews are stored for later consultation.



MARKDOWN FORMAT



Example structure:



Interview Name



Question 1



Category: C# Difficulty: Medium



Question: What is eventual consistency?



Ideal answer: Eventual consistency is a consistency model used in

distributed systems where data replicas may temporarily diverge but

eventually converge to the same state.



MAIN FEATURES



HOME SCREEN The home screen must contain: - Import interview (.md) -

List of available interviews - Start interview - History - Settings



INTERVIEW SCREEN



Display: - Current question - Category - Difficulty - Interview progress



Controls: - Listen to question - Start recording - Stop recording - Next

question



After answering the system must show: - Transcription - Score - AI

feedback



ANSWER EVALUATION



AI should evaluate answers based on: - technical alignment with ideal

answer - coverage of main concepts - clarity - technical accuracy -

objectivity



Each answer receives a score from 0 to 10.



FINAL RESULT



At the end show: - final score - classification



Classification rules:



Approved score >= 8.0



Approved with reservations score >= 6.5 and < 8



Failed score < 6.5



Also show: - strengths - improvement areas



FINAL REPORT



Generate a Markdown report containing:



\-   interview name

\-   date

\-   duration

\-   final score

\-   classification



For each question: - question text - user answer transcript - ideal

answer - score - AI feedback



INTERVIEW HISTORY



Store locally: - interview name - date - duration - final score -

status - report path



User must be able to: - open reports - delete history entries



SETTINGS



Simple settings screen with: - TTS voice - language - AI API key -

reports directory - approval thresholds



TECHNICAL REQUIREMENTS



Language: C# Framework: .NET 10 UI: Windows Forms



Architecture layers: - UI - Application - Domain - Infrastructure



Persistence: SQLite



ORM: Entity Framework Core



SERVICES REQUIRED



Create services for: - Markdown parsing - speech synthesis (TTS) - audio

recording - speech-to-text transcription - AI evaluation - report

generation - interview history management



All services must use interfaces.



UX REQUIREMENTS



The application must be: - fast - intuitive - clean



Include: - interview progress bar - recording indicator - AI processing

indicator



FUTURE EXPANSION



Architecture should allow: - multiple interview templates -

multi-language support - PDF reports - performance analytics



EXPECTED DELIVERY



\-   complete .NET solution

\-   organized project structure

\-   WinForms screens

\-   Markdown parser

\-   SQLite database

\-   automatic report generation

\-   example interview file

\-   README with execution instructions



CODE QUALITY



Code must be: - clean - maintainable - loosely coupled - ready for

expansion



AI EVALUATION BEHAVIOR



The AI receives: - question - ideal answer - user transcript



And returns: - score (0–10) - summary evaluation - strengths -

weaknesses - improvement suggestions



Evaluation must prioritize real technical understanding rather than

textual similarity.



