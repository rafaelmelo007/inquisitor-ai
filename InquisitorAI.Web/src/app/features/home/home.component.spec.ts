import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { HomeComponent } from './home.component';
import { AuthStore } from '../../core/auth/auth.store';
import { AuthService } from '../../core/auth/auth.service';
import { LeaderboardStore } from '../leaderboard/leaderboard.store';
import { ENVIRONMENT } from '../../core/environment.token';
import { LeaderboardEntryDto } from '../leaderboard/models/leaderboard.model';

describe('HomeComponent', () => {
  let fixture: ComponentFixture<HomeComponent>;
  let component: HomeComponent;
  let mockAuthStore: { isAuthenticated: ReturnType<typeof signal>; currentUser: ReturnType<typeof signal> };
  let mockLeaderboardStore: {
    loadTop: jasmine.Spy;
    entries: ReturnType<typeof signal>;
    loading: ReturnType<typeof signal>;
    error: ReturnType<typeof signal>;
  };
  let mockAuthService: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    mockAuthStore = {
      isAuthenticated: signal(false),
      currentUser: signal(null),
    };

    mockLeaderboardStore = {
      loadTop: jasmine.createSpy('loadTop'),
      entries: signal<LeaderboardEntryDto[]>([]),
      loading: signal(false),
      error: signal<string | null>(null),
    };

    mockAuthService = jasmine.createSpyObj('AuthService', ['login']);

    await TestBed.configureTestingModule({
      imports: [HomeComponent],
      providers: [
        { provide: AuthStore, useValue: mockAuthStore },
        { provide: LeaderboardStore, useValue: mockLeaderboardStore },
        { provide: AuthService, useValue: mockAuthService },
        { provide: ENVIRONMENT, useValue: { apiBaseUrl: 'http://test-api', downloadUrl: 'http://test-download' } },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(HomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call leaderboardStore.loadTop on init', () => {
    expect(mockLeaderboardStore.loadTop).toHaveBeenCalledWith(5);
  });

  it('should show sign-in buttons when not authenticated', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    const buttons = compiled.querySelectorAll('button');
    const signInButtons = Array.from(buttons).filter((b) => b.textContent?.includes('Sign in'));
    expect(signInButtons.length).toBe(3);
  });

  it('should hide sign-in buttons when authenticated', () => {
    mockAuthStore.isAuthenticated = signal(true);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const buttons = compiled.querySelectorAll('button');
    const signInButtons = Array.from(buttons).filter((b) => b.textContent?.includes('Sign in'));
    expect(signInButtons.length).toBe(0);
  });
});
