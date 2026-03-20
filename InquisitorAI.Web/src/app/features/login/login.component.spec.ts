import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { Router } from '@angular/router';
import { LoginComponent } from './login.component';
import { AuthStore } from '../../core/auth/auth.store';
import { AuthService } from '../../core/auth/auth.service';

describe('LoginComponent', () => {
  let fixture: ComponentFixture<LoginComponent>;
  let component: LoginComponent;
  let mockAuthStore: { isAuthenticated: ReturnType<typeof signal> };
  let mockAuthService: jasmine.SpyObj<AuthService>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    mockAuthStore = {
      isAuthenticated: signal(false),
    };

    mockAuthService = jasmine.createSpyObj('AuthService', ['login']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [LoginComponent],
      providers: [
        { provide: AuthStore, useValue: mockAuthStore },
        { provide: AuthService, useValue: mockAuthService },
        { provide: Router, useValue: mockRouter },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  it('should redirect to /dashboard if already authenticated', () => {
    mockAuthStore.isAuthenticated = signal(true);
    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(mockRouter.navigate).toHaveBeenCalledWith(['/dashboard']);
  });

  it('should render three OAuth buttons', () => {
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const buttons = compiled.querySelectorAll('button');
    expect(buttons.length).toBe(3);
    expect(buttons[0].textContent).toContain('Google');
    expect(buttons[1].textContent).toContain('GitHub');
    expect(buttons[2].textContent).toContain('LinkedIn');
  });
});
