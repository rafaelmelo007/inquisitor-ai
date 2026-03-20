import { Component, inject, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AuthStore } from './core/auth/auth.store';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [RouterModule],
  templateUrl: './app.component.html',
})
export class AppComponent implements OnInit {
  readonly authStore = inject(AuthStore);

  ngOnInit(): void {
    this.authStore.restoreSession();
  }
}
