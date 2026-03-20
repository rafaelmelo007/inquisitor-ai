import { inject } from '@angular/core';
import { signalStore, withState, withMethods, patchState } from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap, map } from 'rxjs';
import { tapResponse } from '@ngrx/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { QuestionnairesService } from './questionnaires.service';
import { QuestionnaireDto, QuestionnaireDetailDto } from './models/questionnaire.model';

interface QuestionnairesState {
  questionnaires: QuestionnaireDto[];
  selected: QuestionnaireDetailDto | null;
  loading: boolean;
  error: string | null;
}

export const QuestionnairesStore = signalStore(
  { providedIn: 'root' },
  withState<QuestionnairesState>({
    questionnaires: [],
    selected: null,
    loading: false,
    error: null,
  }),
  withMethods((store, service = inject(QuestionnairesService)) => ({
    loadAll: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap(() => service.getAll()),
        tapResponse({
          next: (questionnaires) => patchState(store, { questionnaires, loading: false }),
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
          next: (selected) => patchState(store, { selected, loading: false }),
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
    import: rxMethod<{ file: File; isPublic: boolean }>(
      pipe(
        switchMap(({ file, isPublic }) => service.import(file, isPublic)),
        tapResponse({
          next: (q) =>
            patchState(store, (s) => ({
              questionnaires: [...s.questionnaires, q],
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
    remove: rxMethod<number>(
      pipe(
        switchMap((id) => service.delete(id).pipe(map(() => id))),
        tapResponse({
          next: (id) =>
            patchState(store, (s) => ({
              questionnaires: s.questionnaires.filter((q) => q.id !== id),
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
