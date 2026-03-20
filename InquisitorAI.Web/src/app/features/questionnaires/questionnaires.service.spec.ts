import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { QuestionnairesService } from './questionnaires.service';
import { ENVIRONMENT } from '../../core/environment.token';
import { QuestionnaireDto, QuestionnaireDetailDto } from './models/questionnaire.model';

describe('QuestionnairesService', () => {
  let service: QuestionnairesService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        QuestionnairesService,
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: ENVIRONMENT, useValue: { apiBaseUrl: 'http://test-api' } },
      ],
    });
    service = TestBed.inject(QuestionnairesService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('should GET all questionnaires', () => {
    const expected: QuestionnaireDto[] = [
      {
        id: 1,
        name: 'Test Questionnaire',
        createdByUserId: 1,
        createdByDisplayName: 'User',
        isPublic: true,
        questionCount: 5,
        createdAt: '2025-01-01',
      },
    ];

    service.getAll().subscribe((result) => {
      expect(result).toEqual(expected);
    });

    const req = httpTesting.expectOne('http://test-api/questionnaires');
    expect(req.request.method).toBe('GET');
    req.flush(expected);
  });

  it('should GET questionnaire by id', () => {
    const expected: QuestionnaireDetailDto = {
      id: 1,
      name: 'Test',
      createdByUserId: 1,
      createdByDisplayName: 'User',
      isPublic: true,
      questionCount: 2,
      createdAt: '2025-01-01',
      questions: [
        {
          id: 1,
          questionnaireId: 1,
          orderIndex: 0,
          category: 'General',
          difficulty: 'Easy',
          questionText: 'What is Angular?',
          idealAnswer: 'A framework',
        },
      ],
    };

    service.getById(1).subscribe((result) => {
      expect(result).toEqual(expected);
    });

    const req = httpTesting.expectOne('http://test-api/questionnaires/1');
    expect(req.request.method).toBe('GET');
    req.flush(expected);
  });

  it('should POST import with FormData', () => {
    const file = new File(['content'], 'test.md', { type: 'text/markdown' });
    const expected: QuestionnaireDto = {
      id: 2,
      name: 'Imported',
      createdByUserId: 1,
      createdByDisplayName: 'User',
      isPublic: false,
      questionCount: 3,
      createdAt: '2025-01-01',
    };

    service.import(file, false).subscribe((result) => {
      expect(result).toEqual(expected);
    });

    const req = httpTesting.expectOne('http://test-api/questionnaires');
    expect(req.request.method).toBe('POST');
    expect(req.request.body instanceof FormData).toBe(true);
    req.flush(expected);
  });

  it('should DELETE questionnaire', () => {
    service.delete(1).subscribe();

    const req = httpTesting.expectOne('http://test-api/questionnaires/1');
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});
