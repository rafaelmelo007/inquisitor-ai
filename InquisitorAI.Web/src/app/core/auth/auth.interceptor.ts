import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { TokenStorageService } from './token-storage.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenStorage = inject(TokenStorageService);
  const token = tokenStorage.getAccessToken();

  if (req.url.includes('/auth/refresh') || req.url.includes('/auth/logout')) {
    return next(req);
  }

  if (token) {
    req = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` },
    });
  }

  return next(req);
};
