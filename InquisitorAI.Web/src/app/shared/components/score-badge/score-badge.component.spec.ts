import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Component, signal } from '@angular/core';
import { ScoreBadgeComponent } from './score-badge.component';

@Component({
  standalone: true,
  imports: [ScoreBadgeComponent],
  template: `<app-score-badge [classification]="classification()" />`,
})
class TestHostComponent {
  readonly classification = signal<'Approved' | 'ApprovedWithReservations' | 'Failed'>('Approved');
}

describe('ScoreBadgeComponent', () => {
  let fixture: ComponentFixture<TestHostComponent>;
  let host: TestHostComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestHostComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TestHostComponent);
    host = fixture.componentInstance;
  });

  it("should show 'Approved' with green class", () => {
    host.classification.set('Approved');
    fixture.detectChanges();
    const badge: HTMLElement = fixture.nativeElement.querySelector('span');
    expect(badge.textContent?.trim()).toBe('Approved');
    expect(badge.className).toContain('bg-green-100');
    expect(badge.className).toContain('text-green-800');
  });

  it("should show 'With Reservations' with orange class", () => {
    host.classification.set('ApprovedWithReservations');
    fixture.detectChanges();
    const badge: HTMLElement = fixture.nativeElement.querySelector('span');
    expect(badge.textContent?.trim()).toBe('With Reservations');
    expect(badge.className).toContain('bg-orange-100');
    expect(badge.className).toContain('text-orange-800');
  });

  it("should show 'Failed' with red class", () => {
    host.classification.set('Failed');
    fixture.detectChanges();
    const badge: HTMLElement = fixture.nativeElement.querySelector('span');
    expect(badge.textContent?.trim()).toBe('Failed');
    expect(badge.className).toContain('bg-red-100');
    expect(badge.className).toContain('text-red-800');
  });
});
