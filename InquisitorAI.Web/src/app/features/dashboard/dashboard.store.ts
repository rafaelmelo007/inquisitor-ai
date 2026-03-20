import { inject } from '@angular/core';
import { signalStore, withState, withMethods, patchState } from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap, forkJoin } from 'rxjs';
import { tapResponse } from '@ngrx/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { SessionsService } from '../sessions/sessions.service';
import { LeaderboardService } from '../leaderboard/leaderboard.service';
import { InterviewSessionDto } from '../sessions/models/session.model';
import { UserScoreSummaryDto } from '../../shared/models/user-score-summary.model';

interface DashboardState {
  recentSessions: InterviewSessionDto[];
  scoreSummary: UserScoreSummaryDto | null;
  loading: boolean;
  error: string | null;
}

export const DashboardStore = signalStore(
  { providedIn: 'root' },
  withState<DashboardState>({
    recentSessions: [],
    scoreSummary: null,
    loading: false,
    error: null,
  }),
  withMethods(
    (
      store,
      sessionsService = inject(SessionsService),
      leaderboardService = inject(LeaderboardService),
    ) => ({
      loadDashboard: rxMethod<void>(
        pipe(
          tap(() => patchState(store, { loading: true, error: null })),
          switchMap(() =>
            forkJoin({
              sessions: sessionsService.getAll(),
              scores: leaderboardService.getMyScores(),
            }),
          ),
          tapResponse({
            next: ({ sessions, scores }) =>
              patchState(store, {
                recentSessions: sessions,
                scoreSummary: scores,
                loading: false,
              }),
            error: (err: unknown) => {
              const message =
                err instanceof HttpErrorResponse
                  ? (err.error?.errors?.join(', ') ?? err.message)
                  : String(err);
              patchState(store, { error: message, loading: false });
            },
          }),
        ),
      ),
    }),
  ),
);
