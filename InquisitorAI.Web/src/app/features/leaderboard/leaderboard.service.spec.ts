import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { LeaderboardService } from './leaderboard.service';
import { ENVIRONMENT } from '../../core/environment.token';
import { LeaderboardEntryDto } from './models/leaderboard.model';
import { UserScoreSummaryDto } from '../../shared/models/user-score-summary.model';

describe('LeaderboardService', () => {
  let service: LeaderboardService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        LeaderboardService,
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: ENVIRONMENT, useValue: { apiBaseUrl: 'http://test-api' } },
      ],
    });
    service = TestBed.inject(LeaderboardService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('should GET leaderboard with top param', () => {
    const expected: LeaderboardEntryDto[] = [
      {
        rank: 1,
        userId: 10,
        displayName: 'Alice',
        avatarUrl: null,
        bestScore: 95,
        sessionCount: 5,
        averageScore: 88.5,
      },
    ];

    service.getTop(10).subscribe((result) => {
      expect(result).toEqual(expected);
    });

    const req = httpTesting.expectOne('http://test-api/scores/leaderboard?top=10');
    expect(req.request.method).toBe('GET');
    req.flush(expected);
  });

  it('should GET user scores', () => {
    const expected: UserScoreSummaryDto = {
      userId: 10,
      displayName: 'Alice',
      totalSessions: 5,
      averageScore: 88.5,
      bestScore: 95,
      lastSessionAt: '2026-01-15T10:00:00Z',
    };

    service.getMyScores().subscribe((result) => {
      expect(result).toEqual(expected);
    });

    const req = httpTesting.expectOne('http://test-api/scores/me');
    expect(req.request.method).toBe('GET');
    req.flush(expected);
  });
});
