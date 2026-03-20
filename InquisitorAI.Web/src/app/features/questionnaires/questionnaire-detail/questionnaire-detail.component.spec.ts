import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { QuestionnaireDetailComponent } from './questionnaire-detail.component';
import { QuestionnairesStore } from '../questionnaires.store';
import { QuestionnaireDetailDto } from '../models/questionnaire.model';

describe('QuestionnaireDetailComponent', () => {
  let fixture: ComponentFixture<QuestionnaireDetailComponent>;
  let component: QuestionnaireDetailComponent;
  let mockStore: {
    loadById: jasmine.Spy;
    selected: ReturnType<typeof signal>;
    loading: ReturnType<typeof signal>;
    error: ReturnType<typeof signal>;
  };

  beforeEach(async () => {
    mockStore = {
      loadById: jasmine.createSpy('loadById'),
      selected: signal<QuestionnaireDetailDto | null>(null),
      loading: signal(false),
      error: signal<string | null>(null),
    };

    await TestBed.configureTestingModule({
      imports: [QuestionnaireDetailComponent],
      providers: [
        { provide: QuestionnairesStore, useValue: mockStore },
        {
          provide: ActivatedRoute,
          useValue: {
            params: of({ id: '42' }),
          },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(QuestionnaireDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call store.loadById with route param id', () => {
    expect(mockStore.loadById).toHaveBeenCalledWith(42);
  });
});
