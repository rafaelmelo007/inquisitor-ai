import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { LeaderboardComponent } from './leaderboard.component';
import { LeaderboardStore } from './leaderboard.store';
import { LeaderboardEntryDto } from './models/leaderboard.model';

describe('LeaderboardComponent', () => {
  let fixture: ComponentFixture<LeaderboardComponent>;
  let component: LeaderboardComponent;
  let mockStore: jasmine.SpyObj<InstanceType<typeof LeaderboardStore>>;

  beforeEach(async () => {
    mockStore = jasmine.createSpyObj('LeaderboardStore', ['loadTop'], {
      entries: signal<LeaderboardEntryDto[]>([]),
      loading: signal(false),
      error: signal<string | null>(null),
    });

    await TestBed.configureTestingModule({
      imports: [LeaderboardComponent],
      providers: [{ provide: LeaderboardStore, useValue: mockStore }],
    }).compileComponents();

    fixture = TestBed.createComponent(LeaderboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call store.loadTop on init', () => {
    expect(mockStore.loadTop).toHaveBeenCalledWith(50);
  });

  it('should render ranked entries', () => {
    const entries: LeaderboardEntryDto[] = [
      {
        rank: 1,
        userId: 10,
        displayName: 'Alice',
        avatarUrl: null,
        bestScore: 95,
        sessionCount: 5,
        averageScore: 88.5,
      },
      {
        rank: 2,
        userId: 20,
        displayName: 'Bob',
        avatarUrl: 'https://example.com/bob.png',
        bestScore: 90,
        sessionCount: 3,
        averageScore: 82.0,
      },
    ];

    (mockStore.entries as unknown as ReturnType<typeof signal>).set(entries);
    fixture.detectChanges();

    const rows = fixture.nativeElement.querySelectorAll('tbody tr');
    expect(rows.length).toBe(2);
    expect(rows[0].textContent).toContain('Alice');
    expect(rows[1].textContent).toContain('Bob');
  });
});
