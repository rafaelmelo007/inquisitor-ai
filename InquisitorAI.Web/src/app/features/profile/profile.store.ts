import { inject } from '@angular/core';
import { signalStore, withState, withMethods, patchState } from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { ProfileService, UpdateProfileRequest } from './profile.service';
import { AuthStore } from '../../core/auth/auth.store';
import { UserDto } from '../../shared/models/user.model';

interface ProfileState {
  profile: UserDto | null;
  saving: boolean;
  loading: boolean;
  error: string | null;
}

export const ProfileStore = signalStore(
  { providedIn: 'root' },
  withState<ProfileState>({
    profile: null,
    saving: false,
    loading: false,
    error: null,
  }),
  withMethods(
    (
      store,
      profileService = inject(ProfileService),
      authStore = inject(AuthStore),
    ) => ({
      load: rxMethod<void>(
        pipe(
          tap(() => patchState(store, { loading: true, error: null })),
          switchMap(() => profileService.getMe()),
          tapResponse({
            next: (profile) => patchState(store, { profile, loading: false }),
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
      save: rxMethod<UpdateProfileRequest>(
        pipe(
          tap(() => patchState(store, { saving: true, error: null })),
          switchMap((req) => profileService.update(req)),
          tapResponse({
            next: (profile) => {
              patchState(store, { profile, saving: false });
              authStore.restoreSession();
            },
            error: (err: unknown) => {
              const message =
                err instanceof HttpErrorResponse
                  ? (err.error?.errors?.join(', ') ?? err.message)
                  : String(err);
              patchState(store, { error: message, saving: false });
            },
          }),
        ),
      ),
    }),
  ),
);
