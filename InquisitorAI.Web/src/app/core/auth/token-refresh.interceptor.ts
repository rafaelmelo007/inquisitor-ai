import { inject } from '@angular/core';
import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { TokenStorageService } from './token-storage.service';
import { AuthService } from './auth.service';
import { AuthStore } from './auth.store';

export const tokenRefreshInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenStorage = inject(TokenStorageService);
  const authService = inject(AuthService);
  const authStore = inject(AuthStore);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status !== 401 || req.url.includes('/auth/refresh')) {
        return throwError(() => error);
      }

      const refreshToken = tokenStorage.getRefreshToken();
      if (!refreshToken) {
        authStore.clearSession();
        router.navigate(['/login']);
        return throwError(() => error);
      }

      return authService.refresh(refreshToken).pipe(
        switchMap((tokens) => {
          tokenStorage.saveTokens(tokens.accessToken, tokens.refreshToken);
          return next(
            req.clone({
              setHeaders: { Authorization: `Bearer ${tokens.accessToken}` },
            }),
          );
        }),
        catchError((refreshError) => {
          authStore.clearSession();
          router.navigate(['/login']);
          return throwError(() => refreshError);
        }),
      );
    }),
  );
};
