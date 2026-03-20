import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { ProfileService, UpdateProfileRequest } from './profile.service';
import { ENVIRONMENT } from '../../core/environment.token';
import { UserDto } from '../../shared/models/user.model';

describe('ProfileService', () => {
  let service: ProfileService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        ProfileService,
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: ENVIRONMENT, useValue: { apiBaseUrl: 'http://test-api' } },
      ],
    });
    service = TestBed.inject(ProfileService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('should GET current user profile', () => {
    const expected: UserDto = {
      id: 1,
      email: 'alice@example.com',
      displayName: 'Alice',
      avatarUrl: null,
      provider: 'google',
      createdAt: '2026-01-01T00:00:00Z',
    };

    service.getMe().subscribe((result) => {
      expect(result).toEqual(expected);
    });

    const req = httpTesting.expectOne('http://test-api/users/me');
    expect(req.request.method).toBe('GET');
    req.flush(expected);
  });

  it('should PUT update profile', () => {
    const request: UpdateProfileRequest = { displayName: 'Alice Updated' };
    const expected: UserDto = {
      id: 1,
      email: 'alice@example.com',
      displayName: 'Alice Updated',
      avatarUrl: null,
      provider: 'google',
      createdAt: '2026-01-01T00:00:00Z',
    };

    service.update(request).subscribe((result) => {
      expect(result).toEqual(expected);
    });

    const req = httpTesting.expectOne('http://test-api/users/me');
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(request);
    req.flush(expected);
  });
});
