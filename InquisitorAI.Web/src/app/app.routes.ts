import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadChildren: () => import('./features/home/home.routes').then((m) => m.HOME_ROUTES),
  },
  {
    path: 'login',
    loadChildren: () => import('./features/login/login.routes').then((m) => m.LOGIN_ROUTES),
  },
  {
    path: 'auth/callback',
    loadChildren: () =>
      import('./features/auth-callback/auth-callback.routes').then((m) => m.AUTH_CALLBACK_ROUTES),
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadChildren: () =>
      import('./features/dashboard/dashboard.routes').then((m) => m.DASHBOARD_ROUTES),
  },
  {
    path: 'questionnaires',
    canActivate: [authGuard],
    loadChildren: () =>
      import('./features/questionnaires/questionnaires.routes').then(
        (m) => m.QUESTIONNAIRES_ROUTES,
      ),
  },
  {
    path: 'sessions',
    canActivate: [authGuard],
    loadChildren: () =>
      import('./features/sessions/sessions.routes').then((m) => m.SESSIONS_ROUTES),
  },
  {
    path: 'leaderboard',
    loadChildren: () =>
      import('./features/leaderboard/leaderboard.routes').then((m) => m.LEADERBOARD_ROUTES),
  },
  {
    path: 'profile',
    canActivate: [authGuard],
    loadChildren: () =>
      import('./features/profile/profile.routes').then((m) => m.PROFILE_ROUTES),
  },
  { path: '**', redirectTo: '' },
];
