export interface QuestionnaireDto {
  readonly id: number;
  readonly name: string;
  readonly createdByUserId: number;
  readonly createdByDisplayName: string;
  readonly isPublic: boolean;
  readonly questionCount: number;
  readonly createdAt: string;
}

export interface QuestionDto {
  readonly id: number;
  readonly questionnaireId: number;
  readonly orderIndex: number;
  readonly category: string;
  readonly difficulty: string;
  readonly questionText: string;
  readonly idealAnswer: string;
}

export interface QuestionnaireDetailDto extends QuestionnaireDto {
  readonly questions: QuestionDto[];
}
