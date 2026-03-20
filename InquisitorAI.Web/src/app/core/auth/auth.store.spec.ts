import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { AuthStore } from './auth.store';
import { TokenStorageService } from './token-storage.service';
import { AuthService } from './auth.service';

function createTestJwt(payload: Record<string, unknown>): string {
  const header = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }));
  const body = btoa(JSON.stringify(payload));
  const signature = 'test-signature';
  return `${header}.${body}.${signature}`;
}

describe('AuthStore', () => {
  let store: InstanceType<typeof AuthStore>;
  let mockTokenStorage: jasmine.SpyObj<TokenStorageService>;
  let mockAuthService: jasmine.SpyObj<AuthService>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(() => {
    mockTokenStorage = jasmine.createSpyObj('TokenStorageService', [
      'getAccessToken',
      'getRefreshToken',
      'saveTokens',
      'clear',
    ]);
    mockAuthService = jasmine.createSpyObj('AuthService', ['login', 'logout', 'refresh']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        AuthStore,
        { provide: TokenStorageService, useValue: mockTokenStorage },
        { provide: AuthService, useValue: mockAuthService },
        { provide: Router, useValue: mockRouter },
      ],
    });

    store = TestBed.inject(AuthStore);
  });

  it('restoreSession should decode JWT and set currentUser', () => {
    const token = createTestJwt({
      sub: '42',
      email: 'test@example.com',
      name: 'Test User',
      avatar_url: 'https://example.com/avatar.png',
      provider: 'google',
      created_at: '2025-01-01',
    });
    mockTokenStorage.getAccessToken.and.returnValue(token);

    store.restoreSession();

    expect(store.currentUser()).toEqual({
      id: 42,
      email: 'test@example.com',
      displayName: 'Test User',
      avatarUrl: 'https://example.com/avatar.png',
      provider: 'google',
      createdAt: '2025-01-01',
    });
    expect(store.isAuthenticated()).toBe(true);
  });

  it('restoreSession should clear user when no token', () => {
    mockTokenStorage.getAccessToken.and.returnValue(null);

    store.restoreSession();

    expect(store.currentUser()).toBeNull();
    expect(store.isAuthenticated()).toBe(false);
  });

  it('clearSession should remove tokens and null user', () => {
    // First set a user
    const token = createTestJwt({
      sub: '1',
      email: 'a@b.com',
      name: 'A',
      provider: 'github',
    });
    mockTokenStorage.getAccessToken.and.returnValue(token);
    store.restoreSession();
    expect(store.isAuthenticated()).toBe(true);

    // Now clear
    store.clearSession();

    expect(mockTokenStorage.clear).toHaveBeenCalled();
    expect(store.currentUser()).toBeNull();
    expect(store.isAuthenticated()).toBe(false);
  });

  it('isAuthenticated should be true when user exists', () => {
    const token = createTestJwt({
      sub: '1',
      email: 'a@b.com',
      name: 'A',
      provider: 'github',
    });
    mockTokenStorage.getAccessToken.and.returnValue(token);

    store.restoreSession();

    expect(store.isAuthenticated()).toBe(true);
  });
});
