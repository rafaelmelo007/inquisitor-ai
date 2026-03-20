export interface UserScoreSummaryDto {
  readonly userId: number;
  readonly displayName: string;
  readonly totalSessions: number;
  readonly averageScore: number;
  readonly bestScore: number;
  readonly lastSessionAt: string | null;
}
