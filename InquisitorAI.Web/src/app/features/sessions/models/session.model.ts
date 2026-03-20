export interface InterviewSessionDto {
  readonly id: number;
  readonly userId: number;
  readonly questionnaireId: number;
  readonly questionnaireName: string;
  readonly startedAt: string;
  readonly endedAt: string | null;
  readonly durationSeconds: number | null;
  readonly finalScore: number | null;
  readonly classification: 'Approved' | 'ApprovedWithReservations' | 'Failed' | null;
  readonly reportContent: string | null;
  readonly answers: SessionAnswerDto[];
}

export interface SessionAnswerDto {
  readonly id: number;
  readonly sessionId: number;
  readonly questionId: number;
  readonly questionText: string;
  readonly idealAnswer: string;
  readonly transcript: string | null;
  readonly score: number | null;
  readonly aiFeedback: string | null;
  readonly strengths: string | null;
  readonly weaknesses: string | null;
  readonly improvementSuggestions: string | null;
}
