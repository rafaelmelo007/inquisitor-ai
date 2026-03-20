import { Routes } from '@angular/router';

export const LEADERBOARD_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./leaderboard.component').then((m) => m.LeaderboardComponent),
  },
];
