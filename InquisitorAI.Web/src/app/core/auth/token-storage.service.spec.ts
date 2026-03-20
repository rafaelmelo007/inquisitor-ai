import { TestBed } from '@angular/core/testing';
import { TokenStorageService } from './token-storage.service';

describe('TokenStorageService', () => {
  let service: TokenStorageService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [TokenStorageService],
    });
    service = TestBed.inject(TokenStorageService);
    localStorage.clear();
  });

  afterEach(() => {
    localStorage.clear();
  });

  it('should save and retrieve access token', () => {
    service.saveTokens('access-123', 'refresh-456');
    expect(service.getAccessToken()).toBe('access-123');
  });

  it('should save and retrieve refresh token', () => {
    service.saveTokens('access-123', 'refresh-456');
    expect(service.getRefreshToken()).toBe('refresh-456');
  });

  it('should clear all tokens', () => {
    service.saveTokens('access-123', 'refresh-456');
    service.clear();
    expect(service.getAccessToken()).toBeNull();
    expect(service.getRefreshToken()).toBeNull();
  });

  it('should return null when no token exists', () => {
    expect(service.getAccessToken()).toBeNull();
    expect(service.getRefreshToken()).toBeNull();
  });
});
