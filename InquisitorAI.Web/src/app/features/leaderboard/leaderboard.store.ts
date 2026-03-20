import { inject } from '@angular/core';
import { signalStore, withState, withMethods, patchState } from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { LeaderboardService } from './leaderboard.service';
import { LeaderboardEntryDto } from './models/leaderboard.model';

interface LeaderboardState {
  entries: LeaderboardEntryDto[];
  loading: boolean;
  error: string | null;
}

export const LeaderboardStore = signalStore(
  { providedIn: 'root' },
  withState<LeaderboardState>({
    entries: [],
    loading: false,
    error: null,
  }),
  withMethods((store, service = inject(LeaderboardService)) => ({
    loadTop: rxMethod<number>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap((n) => service.getTop(n)),
        tapResponse({
          next: (entries) => patchState(store, { entries, loading: false }),
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
  })),
);
