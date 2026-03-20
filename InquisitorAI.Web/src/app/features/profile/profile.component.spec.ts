import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { ProfileComponent } from './profile.component';
import { ProfileStore } from './profile.store';
import { UserDto } from '../../shared/models/user.model';

describe('ProfileComponent', () => {
  let fixture: ComponentFixture<ProfileComponent>;
  let component: ProfileComponent;
  let mockStore: jasmine.SpyObj<InstanceType<typeof ProfileStore>>;

  beforeEach(async () => {
    mockStore = jasmine.createSpyObj('ProfileStore', ['load', 'save'], {
      profile: signal<UserDto | null>(null),
      saving: signal(false),
      loading: signal(false),
      error: signal<string | null>(null),
    });

    await TestBed.configureTestingModule({
      imports: [ProfileComponent, ReactiveFormsModule],
      providers: [{ provide: ProfileStore, useValue: mockStore }],
    }).compileComponents();

    fixture = TestBed.createComponent(ProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call store.load on init', () => {
    expect(mockStore.load).toHaveBeenCalled();
  });

  it('should submit form calling store.save', () => {
    component.form.controls.displayName.setValue('New Name');
    component.onSubmit();

    expect(mockStore.save).toHaveBeenCalledWith({ displayName: 'New Name' });
  });

  it('should disable save button while saving', () => {
    component.form.controls.displayName.setValue('Valid Name');
    fixture.detectChanges();

    const button: HTMLButtonElement = fixture.nativeElement.querySelector('button[type="submit"]');
    expect(button.disabled).toBeFalse();

    (mockStore.saving as unknown as ReturnType<typeof signal>).set(true);
    fixture.detectChanges();

    expect(button.disabled).toBeTrue();
  });
});
