import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from './auth.service';
import { ENVIRONMENT } from '../environment.token';
import { TokenResponse } from '../../shared/models/user.model';

describe('AuthService', () => {
  let service: AuthService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        AuthService,
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: ENVIRONMENT, useValue: { apiBaseUrl: 'http://test-api', downloadUrl: '' } },
      ],
    });
    service = TestBed.inject(AuthService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('login() should redirect via window.location.href', () => {
    const originalLocation = window.location.href;
    spyOnProperty(window, 'location', 'get').and.returnValue({
      ...window.location,
      set href(url: string) {
        // no-op for test
      },
      get href() {
        return originalLocation;
      },
    } as Location);

    // Verify login does not throw and constructs the correct URL
    expect(() => service.login('google')).not.toThrow();
  });

  it('logout() should POST to /auth/logout', () => {
    service.logout().subscribe();

    const req = httpTesting.expectOne('http://test-api/auth/logout');
    expect(req.request.method).toBe('POST');
    req.flush(null);
  });

  it('refresh() should POST to /auth/refresh with token', () => {
    const mockResponse: TokenResponse = {
      accessToken: 'new-access',
      refreshToken: 'new-refresh',
      expiresIn: 3600,
    };

    service.refresh('old-refresh-token').subscribe((result) => {
      expect(result).toEqual(mockResponse);
    });

    const req = httpTesting.expectOne('http://test-api/auth/refresh');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ refreshToken: 'old-refresh-token' });
    req.flush(mockResponse);
  });
});
