import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { SessionsService } from './sessions.service';
import { ENVIRONMENT } from '../../core/environment.token';
import { InterviewSessionDto } from './models/session.model';

describe('SessionsService', () => {
  let service: SessionsService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        SessionsService,
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: ENVIRONMENT, useValue: { apiBaseUrl: 'http://test-api' } },
      ],
    });
    service = TestBed.inject(SessionsService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('should GET all sessions', () => {
    const expected: InterviewSessionDto[] = [
      {
        id: 1,
        userId: 10,
        questionnaireId: 5,
        questionnaireName: 'Test Quiz',
        startedAt: '2026-01-01T00:00:00Z',
        endedAt: null,
        durationSeconds: null,
        finalScore: null,
        classification: null,
        reportContent: null,
        answers: [],
      },
    ];

    service.getAll().subscribe((result) => {
      expect(result).toEqual(expected);
    });

    const req = httpTesting.expectOne('http://test-api/sessions');
    expect(req.request.method).toBe('GET');
    req.flush(expected);
  });

  it('should GET session by id', () => {
    const expected: InterviewSessionDto = {
      id: 42,
      userId: 10,
      questionnaireId: 5,
      questionnaireName: 'Test Quiz',
      startedAt: '2026-01-01T00:00:00Z',
      endedAt: '2026-01-01T01:00:00Z',
      durationSeconds: 3600,
      finalScore: 85,
      classification: 'Approved',
      reportContent: 'Good job',
      answers: [],
    };

    service.getById(42).subscribe((result) => {
      expect(result).toEqual(expected);
    });

    const req = httpTesting.expectOne('http://test-api/sessions/42');
    expect(req.request.method).toBe('GET');
    req.flush(expected);
  });

  it('should DELETE session', () => {
    service.delete(42).subscribe((result) => {
      expect(result).toBeUndefined();
    });

    const req = httpTesting.expectOne('http://test-api/sessions/42');
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});
