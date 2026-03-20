export interface LeaderboardEntryDto {
  readonly rank: number;
  readonly userId: number;
  readonly displayName: string;
  readonly avatarUrl: string | null;
  readonly bestScore: number;
  readonly sessionCount: number;
  readonly averageScore: number;
}
