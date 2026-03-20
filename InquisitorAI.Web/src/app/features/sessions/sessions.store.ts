import { inject } from '@angular/core';
import { signalStore, withState, withMethods, patchState } from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap, map } from 'rxjs';
import { tapResponse } from '@ngrx/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { SessionsService } from './sessions.service';
import { InterviewSessionDto } from './models/session.model';

interface SessionsState {
  sessions: InterviewSessionDto[];
  selectedSession: InterviewSessionDto | null;
  loading: boolean;
  error: string | null;
}

export const SessionsStore = signalStore(
  { providedIn: 'root' },
  withState<SessionsState>({
    sessions: [],
    selectedSession: null,
    loading: false,
    error: null,
  }),
  withMethods((store, service = inject(SessionsService)) => ({
    loadAll: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap(() => service.getAll()),
        tapResponse({
          next: (sessions) => patchState(store, { sessions, loading: false }),
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
    loadById: rxMethod<number>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap((id) => service.getById(id)),
        tapResponse({
          next: (selectedSession) => patchState(store, { selectedSession, loading: false }),
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
    remove: rxMethod<number>(
      pipe(
        switchMap((id) => service.delete(id).pipe(map(() => id))),
        tapResponse({
          next: (id) =>
            patchState(store, (s) => ({
              sessions: s.sessions.filter((session) => session.id !== id),
            })),
          error: (err: unknown) => {
            const message =
              err instanceof HttpErrorResponse
                ? (err.error?.errors?.join(', ') ?? err.message)
                : String(err);
            patchState(store, { error: message });
          },
        }),
      ),
    ),
  })),
);
