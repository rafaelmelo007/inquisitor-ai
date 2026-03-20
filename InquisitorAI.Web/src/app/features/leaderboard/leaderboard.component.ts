import { Component, inject } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { LeaderboardStore } from './leaderboard.store';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  standalone: true,
  selector: 'app-leaderboard',
  imports: [DecimalPipe, LoadingSpinnerComponent],
  templateUrl: './leaderboard.component.html',
})
export class LeaderboardComponent {
  readonly store = inject(LeaderboardStore);

  ngOnInit(): void {
    this.store.loadTop(50);
  }

  rowClass(rank: number): string {
    switch (rank) {
      case 1:
        return 'bg-yellow-50';
      case 2:
        return 'bg-gray-50';
      case 3:
        return 'bg-orange-50';
      default:
        return 'bg-white';
    }
  }
}
