import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { provideRouter } from '@angular/router';
import { QuestionnaireListComponent } from './questionnaire-list.component';
import { QuestionnairesStore } from '../questionnaires.store';
import { AuthStore } from '../../../core/auth/auth.store';
import { QuestionnaireDto } from '../models/questionnaire.model';

describe('QuestionnaireListComponent', () => {
  let fixture: ComponentFixture<QuestionnaireListComponent>;
  let component: QuestionnaireListComponent;
  let mockStore: {
    loadAll: jasmine.Spy;
    import: jasmine.Spy;
    remove: jasmine.Spy;
    questionnaires: ReturnType<typeof signal>;
    loading: ReturnType<typeof signal>;
    error: ReturnType<typeof signal>;
  };
  let mockAuthStore: {
    isAuthenticated: ReturnType<typeof signal>;
    currentUser: ReturnType<typeof signal>;
  };

  beforeEach(async () => {
    mockStore = {
      loadAll: jasmine.createSpy('loadAll'),
      import: jasmine.createSpy('import'),
      remove: jasmine.createSpy('remove'),
      questionnaires: signal<QuestionnaireDto[]>([]),
      loading: signal(false),
      error: signal<string | null>(null),
    };

    mockAuthStore = {
      isAuthenticated: signal(true),
      currentUser: signal({ id: 1, email: 'test@example.com', displayName: 'Test', avatarUrl: null, provider: 'google', createdAt: '2025-01-01' }),
    };

    await TestBed.configureTestingModule({
      imports: [QuestionnaireListComponent],
      providers: [
        provideRouter([]),
        { provide: QuestionnairesStore, useValue: mockStore },
        { provide: AuthStore, useValue: mockAuthStore },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(QuestionnaireListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call store.loadAll on init', () => {
    expect(mockStore.loadAll).toHaveBeenCalled();
  });

  it('should render questionnaire cards', () => {
    mockStore.questionnaires = signal<QuestionnaireDto[]>([
      {
        id: 1,
        name: 'Angular Basics',
        createdByUserId: 1,
        createdByDisplayName: 'User',
        isPublic: true,
        questionCount: 10,
        createdAt: '2025-01-01',
      },
      {
        id: 2,
        name: 'TypeScript Advanced',
        createdByUserId: 2,
        createdByDisplayName: 'Other',
        isPublic: false,
        questionCount: 5,
        createdAt: '2025-01-02',
      },
    ]);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const cards = compiled.querySelectorAll('h3');
    expect(cards.length).toBe(2);
    expect(cards[0].textContent).toContain('Angular Basics');
    expect(cards[1].textContent).toContain('TypeScript Advanced');
  });
});
