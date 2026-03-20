import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ENVIRONMENT } from '../environment.token';
import { TokenResponse } from '../../shared/models/user.model';

export const OAUTH_PROVIDERS = ['google', 'github', 'linkedin'] as const;
export type OAuthProvider = (typeof OAUTH_PROVIDERS)[number];

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = inject(ENVIRONMENT).apiBaseUrl;

  login(provider: OAuthProvider): void {
    window.location.href = `${this.apiUrl}/auth/${provider}`;
  }

  logout(): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/auth/logout`, {});
  }

  refresh(refreshToken: string): Observable<TokenResponse> {
    return this.http.post<TokenResponse>(`${this.apiUrl}/auth/refresh`, { refreshToken });
  }
}
