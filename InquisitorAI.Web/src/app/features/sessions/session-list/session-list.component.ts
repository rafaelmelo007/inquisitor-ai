import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { DatePipe } from '@angular/common';
import { SessionsStore } from '../sessions.store';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { ScoreBadgeComponent } from '../../../shared/components/score-badge/score-badge.component';

@Component({
  standalone: true,
  selector: 'app-session-list',
  imports: [RouterModule, DatePipe, LoadingSpinnerComponent, ScoreBadgeComponent],
  templateUrl: './session-list.component.html',
})
export class SessionListComponent {
  readonly store = inject(SessionsStore);

  ngOnInit(): void {
    this.store.loadAll();
  }

  onDelete(id: number): void {
    this.store.remove(id);
  }

  formatDuration(seconds: number | null): string {
    if (seconds === null) return '--';
    const minutes = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${minutes}m ${secs}s`;
  }
}
