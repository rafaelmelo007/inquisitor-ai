import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { signal } from '@angular/core';
import { authGuard } from './auth.guard';
import { AuthStore } from './auth.store';

describe('authGuard', () => {
  let mockRouter: jasmine.SpyObj<Router>;

  function setup(isAuthenticated: boolean): void {
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        { provide: AuthStore, useValue: { isAuthenticated: signal(isAuthenticated) } },
        { provide: Router, useValue: mockRouter },
      ],
    });
  }

  it('should return true when authenticated', () => {
    setup(true);

    const result = TestBed.runInInjectionContext(() =>
      authGuard({} as never, {} as never),
    );

    expect(result).toBe(true);
    expect(mockRouter.navigate).not.toHaveBeenCalled();
  });

  it('should navigate to /login and return false when not authenticated', () => {
    setup(false);

    const result = TestBed.runInInjectionContext(() =>
      authGuard({} as never, {} as never),
    );

    expect(result).toBe(false);
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/login']);
  });
});
