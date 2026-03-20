import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ENVIRONMENT } from '../../core/environment.token';
import { UserDto } from '../../shared/models/user.model';

export interface UpdateProfileRequest {
  readonly displayName: string;
  readonly avatarUrl?: string | null;
}

@Injectable({ providedIn: 'root' })
export class ProfileService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = inject(ENVIRONMENT).apiBaseUrl;

  getMe(): Observable<UserDto> {
    return this.http.get<UserDto>(`${this.apiUrl}/users/me`);
  }

  update(req: UpdateProfileRequest): Observable<UserDto> {
    return this.http.put<UserDto>(`${this.apiUrl}/users/me`, req);
  }
}
