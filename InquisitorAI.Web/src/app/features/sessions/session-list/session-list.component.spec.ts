import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { SessionListComponent } from './session-list.component';
import { SessionsStore } from '../sessions.store';
import { InterviewSessionDto } from '../models/session.model';

describe('SessionListComponent', () => {
  let fixture: ComponentFixture<SessionListComponent>;
  let component: SessionListComponent;
  let mockStore: jasmine.SpyObj<InstanceType<typeof SessionsStore>>;

  beforeEach(async () => {
    mockStore = jasmine.createSpyObj('SessionsStore', ['loadAll', 'remove'], {
      sessions: signal<InterviewSessionDto[]>([]),
      loading: signal(false),
      error: signal<string | null>(null),
    });

    await TestBed.configureTestingModule({
      imports: [SessionListComponent],
      providers: [{ provide: SessionsStore, useValue: mockStore }],
    }).compileComponents();

    fixture = TestBed.createComponent(SessionListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call store.loadAll on init', () => {
    expect(mockStore.loadAll).toHaveBeenCalled();
  });

  it('should render session rows', () => {
    const sessions: InterviewSessionDto[] = [
      {
        id: 1,
        userId: 10,
        questionnaireId: 5,
        questionnaireName: 'Angular Basics',
        startedAt: '2026-01-15T10:00:00Z',
        endedAt: '2026-01-15T10:30:00Z',
        durationSeconds: 1800,
        finalScore: 90,
        classification: 'Approved',
        reportContent: null,
        answers: [],
      },
      {
        id: 2,
        userId: 10,
        questionnaireId: 6,
        questionnaireName: 'TypeScript Advanced',
        startedAt: '2026-01-16T10:00:00Z',
        endedAt: null,
        durationSeconds: null,
        finalScore: null,
        classification: null,
        reportContent: null,
        answers: [],
      },
    ];

    (mockStore.sessions as unknown as ReturnType<typeof signal>).set(sessions);
    fixture.detectChanges();

    const rows = fixture.nativeElement.querySelectorAll('tbody tr');
    expect(rows.length).toBe(2);
    expect(rows[0].textContent).toContain('Angular Basics');
    expect(rows[1].textContent).toContain('TypeScript Advanced');
  });
});
