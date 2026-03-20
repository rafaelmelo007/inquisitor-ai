import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthStore } from '../../core/auth/auth.store';
import { AuthService, OAuthProvider } from '../../core/auth/auth.service';

@Component({
  standalone: true,
  selector: 'app-login',
  templateUrl: './login.component.html',
})
export class LoginComponent {
  readonly authStore = inject(AuthStore);
  readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  ngOnInit(): void {
    if (this.authStore.isAuthenticated()) {
      this.router.navigate(['/dashboard']);
    }
  }

  login(provider: OAuthProvider): void {
    this.authService.login(provider);
  }
}
