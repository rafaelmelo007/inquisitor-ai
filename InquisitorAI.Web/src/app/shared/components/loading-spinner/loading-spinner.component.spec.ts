import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Component, signal } from '@angular/core';
import { LoadingSpinnerComponent } from './loading-spinner.component';

@Component({
  standalone: true,
  imports: [LoadingSpinnerComponent],
  template: `<app-loading-spinner [message]="message()" />`,
})
class TestHostComponent {
  readonly message = signal<string | undefined>(undefined);
}

describe('LoadingSpinnerComponent', () => {
  let fixture: ComponentFixture<TestHostComponent>;
  let host: TestHostComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestHostComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TestHostComponent);
    host = fixture.componentInstance;
  });

  it('should render spinner', () => {
    fixture.detectChanges();
    const spinnerEl = fixture.nativeElement.querySelector('.animate-spin');
    expect(spinnerEl).toBeTruthy();
  });

  it('should display message when provided', () => {
    host.message.set('Loading data...');
    fixture.detectChanges();
    const span = fixture.nativeElement.querySelector('span');
    expect(span).toBeTruthy();
    expect(span.textContent.trim()).toBe('Loading data...');
  });

  it('should not display message when not provided', () => {
    fixture.detectChanges();
    const span = fixture.nativeElement.querySelector('span');
    expect(span).toBeNull();
  });
});
