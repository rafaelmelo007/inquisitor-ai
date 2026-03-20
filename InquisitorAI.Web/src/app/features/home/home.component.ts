import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AuthStore } from '../../core/auth/auth.store';
import { AuthService } from '../../core/auth/auth.service';
import { LeaderboardStore } from '../leaderboard/leaderboard.store';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';
import { ENVIRONMENT } from '../../core/environment.token';
import { OAuthProvider } from '../../core/auth/auth.service';

@Component({
  standalone: true,
  selector: 'app-home',
  imports: [RouterModule, LoadingSpinnerComponent],
  templateUrl: './home.component.html',
})
export class HomeComponent {
  readonly authStore = inject(AuthStore);
  readonly leaderboardStore = inject(LeaderboardStore);
  readonly authService = inject(AuthService);
  readonly downloadUrl = inject(ENVIRONMENT).downloadUrl;

  ngOnInit(): void {
    this.leaderboardStore.loadTop(5);
  }

  login(provider: OAuthProvider): void {
    this.authService.login(provider);
  }
}
