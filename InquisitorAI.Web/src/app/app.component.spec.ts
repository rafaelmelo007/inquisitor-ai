import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { AuthStore } from './core/auth/auth.store';
import { UserDto } from './shared/models/user.model';

describe('AppComponent', () => {
  let fixture: ComponentFixture<AppComponent>;
  let component: AppComponent;
  let mockAuthStore: jasmine.SpyObj<InstanceType<typeof AuthStore>>;

  beforeEach(async () => {
    mockAuthStore = jasmine.createSpyObj('AuthStore', ['restoreSession', 'logout'], {
      currentUser: signal<UserDto | null>(null),
      isAuthenticated: signal(false),
    });

    await TestBed.configureTestingModule({
      imports: [AppComponent, RouterModule.forRoot([])],
      providers: [{ provide: AuthStore, useValue: mockAuthStore }],
    }).compileComponents();

    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call authStore.restoreSession on init', () => {
    expect(mockAuthStore.restoreSession).toHaveBeenCalled();
  });

  it('should render router-outlet', () => {
    const outlet = fixture.nativeElement.querySelector('router-outlet');
    expect(outlet).toBeTruthy();
  });
});
