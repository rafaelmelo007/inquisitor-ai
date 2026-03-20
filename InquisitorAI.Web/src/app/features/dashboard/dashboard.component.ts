import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { DatePipe, DecimalPipe } from '@angular/common';
import { AuthStore } from '../../core/auth/auth.store';
import { DashboardStore } from './dashboard.store';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';
import { ScoreBadgeComponent } from '../../shared/components/score-badge/score-badge.component';

@Component({
  standalone: true,
  selector: 'app-dashboard',
  imports: [RouterModule, DatePipe, DecimalPipe, LoadingSpinnerComponent, ScoreBadgeComponent],
  templateUrl: './dashboard.component.html',
})
export class DashboardComponent {
  readonly authStore = inject(AuthStore);
  readonly dashboardStore = inject(DashboardStore);

  ngOnInit(): void {
    this.dashboardStore.loadDashboard();
  }
}
