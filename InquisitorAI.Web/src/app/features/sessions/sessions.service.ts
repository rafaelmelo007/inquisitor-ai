import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ENVIRONMENT } from '../../core/environment.token';
import { InterviewSessionDto } from './models/session.model';

@Injectable({ providedIn: 'root' })
export class SessionsService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = inject(ENVIRONMENT).apiBaseUrl;

  getAll(): Observable<InterviewSessionDto[]> {
    return this.http.get<InterviewSessionDto[]>(`${this.apiUrl}/sessions`);
  }

  getById(id: number): Observable<InterviewSessionDto> {
    return this.http.get<InterviewSessionDto>(`${this.apiUrl}/sessions/${id}`);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/sessions/${id}`);
  }
}
