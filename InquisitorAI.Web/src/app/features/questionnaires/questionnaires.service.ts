import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ENVIRONMENT } from '../../core/environment.token';
import { QuestionnaireDto, QuestionnaireDetailDto } from './models/questionnaire.model';

@Injectable({ providedIn: 'root' })
export class QuestionnairesService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = inject(ENVIRONMENT).apiBaseUrl;

  getAll(): Observable<QuestionnaireDto[]> {
    return this.http.get<QuestionnaireDto[]>(`${this.apiUrl}/questionnaires`);
  }

  getById(id: number): Observable<QuestionnaireDetailDto> {
    return this.http.get<QuestionnaireDetailDto>(`${this.apiUrl}/questionnaires/${id}`);
  }

  import(file: File, isPublic: boolean): Observable<QuestionnaireDto> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('isPublic', String(isPublic));
    return this.http.post<QuestionnaireDto>(`${this.apiUrl}/questionnaires`, formData);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/questionnaires/${id}`);
  }
}
