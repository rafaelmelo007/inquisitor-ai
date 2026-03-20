import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { DashboardComponent } from './dashboard.component';
import { AuthStore } from '../../core/auth/auth.store';
import { DashboardStore } from './dashboard.store';
import { InterviewSessionDto } from '../sessions/models/session.model';
import { UserScoreSummaryDto } from '../../shared/models/user-score-summary.model';
import { provideRouter } from '@angular/router';

describe('DashboardComponent', () => {
  let fixture: ComponentFixture<DashboardComponent>;
  let component: DashboardComponent;
  let mockAuthStore: {
    isAuthenticated: ReturnType<typeof signal>;
    currentUser: ReturnType<typeof signal>;
  };
  let mockDashboardStore: {
    loadDashboard: jasmine.Spy;
    recentSessions: ReturnType<typeof signal>;
    scoreSummary: ReturnType<typeof signal>;
    loading: ReturnType<typeof signal>;
    error: ReturnType<typeof signal>;
  };

  beforeEach(async () => {
    mockAuthStore = {
      isAuthenticated: signal(true),
      currentUser: signal({
        id: 1,
        email: 'test@example.com',
        displayName: 'Test User',
        avatarUrl: null,
        provider: 'google',
        createdAt: '2025-01-01',
      }),
    };

    mockDashboardStore = {
      loadDashboard: jasmine.createSpy('loadDashboard'),
      recentSessions: signal<InterviewSessionDto[]>([]),
      scoreSummary: signal<UserScoreSummaryDto | null>(null),
      loading: signal(false),
      error: signal<string | null>(null),
    };

    await TestBed.configureTestingModule({
      imports: [DashboardComponent],
      providers: [
        provideRouter([]),
        { provide: AuthStore, useValue: mockAuthStore },
        { provide: DashboardStore, useValue: mockDashboardStore },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call dashboardStore.loadDashboard on init', () => {
    expect(mockDashboardStore.loadDashboard).toHaveBeenCalled();
  });

  it('should display user welcome message', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    const heading = compiled.querySelector('h1');
    expect(heading?.textContent).toContain('Welcome back, Test User');
  });
});
