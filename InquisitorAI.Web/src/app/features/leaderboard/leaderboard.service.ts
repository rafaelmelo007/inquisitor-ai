import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ENVIRONMENT } from '../../core/environment.token';
import { LeaderboardEntryDto } from './models/leaderboard.model';
import { UserScoreSummaryDto } from '../../shared/models/user-score-summary.model';

@Injectable({ providedIn: 'root' })
export class LeaderboardService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = inject(ENVIRONMENT).apiBaseUrl;

  getTop(n: number = 50): Observable<LeaderboardEntryDto[]> {
    return this.http.get<LeaderboardEntryDto[]>(`${this.apiUrl}/scores/leaderboard?top=${n}`);
  }

  getMyScores(): Observable<UserScoreSummaryDto> {
    return this.http.get<UserScoreSummaryDto>(`${this.apiUrl}/scores/me`);
  }
}
