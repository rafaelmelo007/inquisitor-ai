import { TestBed } from '@angular/core/testing';
import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { authInterceptor } from './auth.interceptor';
import { TokenStorageService } from './token-storage.service';

describe('authInterceptor', () => {
  let http: HttpClient;
  let httpTesting: HttpTestingController;

  function setup(tokenValue: string | null): void {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([authInterceptor])),
        provideHttpClientTesting(),
        {
          provide: TokenStorageService,
          useValue: { getAccessToken: () => tokenValue },
        },
      ],
    });
    http = TestBed.inject(HttpClient);
    httpTesting = TestBed.inject(HttpTestingController);
  }

  afterEach(() => httpTesting.verify());

  it('should add Authorization header when token exists', () => {
    setup('test-token');

    http.get('/api/data').subscribe();

    const req = httpTesting.expectOne('/api/data');
    expect(req.request.headers.get('Authorization')).toBe('Bearer test-token');
    req.flush({});
  });

  it('should not add header when no token', () => {
    setup(null);

    http.get('/api/data').subscribe();

    const req = httpTesting.expectOne('/api/data');
    expect(req.request.headers.has('Authorization')).toBeFalse();
    req.flush({});
  });

  it('should skip header for /auth/refresh requests', () => {
    setup('test-token');

    http.post('/auth/refresh', {}).subscribe();

    const req = httpTesting.expectOne('/auth/refresh');
    expect(req.request.headers.has('Authorization')).toBeFalse();
    req.flush({});
  });
});
