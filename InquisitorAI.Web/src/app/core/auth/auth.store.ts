import { computed, inject } from '@angular/core';
import { Router } from '@angular/router';
import { signalStore, withState, withComputed, withMethods, patchState } from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { tapResponse } from '@ngrx/operators';
import { pipe, switchMap } from 'rxjs';
import { jwtDecode } from 'jwt-decode';
import { UserDto, JwtPayload } from '../../shared/models/user.model';
import { TokenStorageService } from './token-storage.service';
import { AuthService } from './auth.service';

interface AuthState {
  currentUser: UserDto | null;
  loading: boolean;
}

export const AuthStore = signalStore(
  { providedIn: 'root' },
  withState<AuthState>({ currentUser: null, loading: false }),
  withComputed(({ currentUser }) => ({
    isAuthenticated: computed(() => currentUser() !== null),
  })),
  withMethods(
    (
      store,
      tokenStorage = inject(TokenStorageService),
      authService = inject(AuthService),
      router = inject(Router),
    ) => ({
      restoreSession(): void {
        const token = tokenStorage.getAccessToken();
        if (!token) {
          patchState(store, { currentUser: null });
          return;
        }
        try {
          const decoded = jwtDecode<JwtPayload>(token);
          patchState(store, {
            currentUser: {
              id: Number(decoded.sub),
              email: decoded.email,
              displayName: decoded.name,
              avatarUrl: decoded.avatar_url ?? null,
              provider: decoded.provider,
              createdAt: decoded.created_at ?? '',
            },
          });
        } catch {
          tokenStorage.clear();
          patchState(store, { currentUser: null });
        }
      },
      clearSession(): void {
        tokenStorage.clear();
        patchState(store, { currentUser: null });
      },
      logout: rxMethod<void>(
        pipe(
          switchMap(() => authService.logout()),
          tapResponse({
            next: () => {
              tokenStorage.clear();
              patchState(store, { currentUser: null });
              router.navigate(['/']);
            },
            error: (err: unknown) => {
              tokenStorage.clear();
              patchState(store, { currentUser: null });
              router.navigate(['/']);
            },
          }),
        ),
      ),
    }),
  ),
);
