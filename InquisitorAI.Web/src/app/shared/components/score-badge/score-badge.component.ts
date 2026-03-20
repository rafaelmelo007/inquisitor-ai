import { Component, computed, input } from '@angular/core';

@Component({
  standalone: true,
  selector: 'app-score-badge',
  template: `
    <span [class]="'inline-flex items-center rounded-full px-3 py-0.5 text-xs font-medium ' + badgeClass()">
      {{ label() }}
    </span>
  `,
})
export class ScoreBadgeComponent {
  readonly classification = input.required<'Approved' | 'ApprovedWithReservations' | 'Failed'>();

  readonly label = computed(() => {
    switch (this.classification()) {
      case 'Approved':
        return 'Approved';
      case 'ApprovedWithReservations':
        return 'With Reservations';
      case 'Failed':
        return 'Failed';
    }
  });

  readonly badgeClass = computed(() => {
    switch (this.classification()) {
      case 'Approved':
        return 'bg-green-100 text-green-800';
      case 'ApprovedWithReservations':
        return 'bg-orange-100 text-orange-800';
      case 'Failed':
        return 'bg-red-100 text-red-800';
    }
  });
}
