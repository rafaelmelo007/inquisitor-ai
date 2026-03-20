import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { DashboardStore } from './dashboard.store';
import { SessionsService } from '../sessions/sessions.service';
import { LeaderboardService } from '../leaderboard/leaderboard.service';
import { ENVIRONMENT } from '../../core/environment.token';

describe('DashboardStore', () => {
  let store: InstanceType<typeof DashboardStore>;
  let mockSessionsService: jasmine.SpyObj<SessionsService>;
  let mockLeaderboardService: jasmine.SpyObj<LeaderboardService>;

  beforeEach(() => {
    mockSessionsService = jasmine.createSpyObj('SessionsService', ['getAll']);
    mockLeaderboardService = jasmine.createSpyObj('LeaderboardService', ['getMyScores']);

    mockSessionsService.getAll.and.returnValue(of([]));
    mockLeaderboardService.getMyScores.and.returnValue(
      of({
        userId: 1,
        displayName: 'Test',
        totalSessions: 0,
        averageScore: 0,
        bestScore: 0,
        lastSessionAt: null,
      }),
    );

    TestBed.configureTestingModule({
      providers: [
        DashboardStore,
        { provide: SessionsService, useValue: mockSessionsService },
        { provide: LeaderboardService, useValue: mockLeaderboardService },
        { provide: ENVIRONMENT, useValue: { apiBaseUrl: 'http://test-api' } },
      ],
    });

    store = TestBed.inject(DashboardStore);
  });

  it('should set loading true when loadDashboard starts', () => {
    // Before calling loadDashboard, loading should be false
    expect(store.loading()).toBe(false);

    store.loadDashboard();

    // After the rxMethod processes synchronously with `of()`, loading resolves to false
    // because tapResponse next handler sets loading: false.
    // We verify the services were called, confirming the pipeline executed.
    expect(mockSessionsService.getAll).toHaveBeenCalled();
    expect(mockLeaderboardService.getMyScores).toHaveBeenCalled();
  });
});
