import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { of } from 'rxjs';
import { AuthCallbackComponent } from './auth-callback.component';
import { TokenStorageService } from '../../core/auth/token-storage.service';
import { AuthStore } from '../../core/auth/auth.store';

describe('AuthCallbackComponent', () => {
  let fixture: ComponentFixture<AuthCallbackComponent>;
  let component: AuthCallbackComponent;
  let mockTokenStorage: jasmine.SpyObj<TokenStorageService>;
  let mockAuthStore: { restoreSession: jasmine.Spy };
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    mockTokenStorage = jasmine.createSpyObj('TokenStorageService', ['saveTokens', 'clear']);
    mockAuthStore = {
      restoreSession: jasmine.createSpy('restoreSession'),
    };
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [AuthCallbackComponent],
      providers: [
        {
          provide: ActivatedRoute,
          useValue: {
            queryParams: of({
              access_token: 'test-access-token',
              refresh_token: 'test-refresh-token',
            }),
          },
        },
        { provide: TokenStorageService, useValue: mockTokenStorage },
        { provide: AuthStore, useValue: mockAuthStore },
        { provide: Router, useValue: mockRouter },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(AuthCallbackComponent);
    component = fixture.componentInstance;
  });

  it('should save tokens from query params and navigate to /dashboard', () => {
    fixture.detectChanges();

    expect(mockTokenStorage.saveTokens).toHaveBeenCalledWith('test-access-token', 'test-refresh-token');
    expect(mockAuthStore.restoreSession).toHaveBeenCalled();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/dashboard']);
  });

  it('should show loading spinner while processing', () => {
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const spinner = compiled.querySelector('app-loading-spinner');
    expect(spinner).toBeTruthy();
  });
});
