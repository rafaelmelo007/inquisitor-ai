import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TokenStorageService } from '../../core/auth/token-storage.service';
import { AuthStore } from '../../core/auth/auth.store';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  standalone: true,
  selector: 'app-auth-callback',
  imports: [LoadingSpinnerComponent],
  templateUrl: './auth-callback.component.html',
})
export class AuthCallbackComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly tokenStorage = inject(TokenStorageService);
  private readonly authStore = inject(AuthStore);
  private readonly router = inject(Router);

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      const accessToken = params['access_token'];
      const refreshToken = params['refresh_token'];

      if (accessToken && refreshToken) {
        this.tokenStorage.saveTokens(accessToken, refreshToken);
        this.authStore.restoreSession();
        this.router.navigate(['/dashboard']);
      } else {
        this.router.navigate(['/login']);
      }
    });
  }
}
