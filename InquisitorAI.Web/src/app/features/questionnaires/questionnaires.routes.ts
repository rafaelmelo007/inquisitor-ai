import { Routes } from '@angular/router';

export const QUESTIONNAIRES_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./questionnaire-list/questionnaire-list.component').then(
        (m) => m.QuestionnaireListComponent,
      ),
  },
  {
    path: ':id',
    loadComponent: () =>
      import('./questionnaire-detail/questionnaire-detail.component').then(
        (m) => m.QuestionnaireDetailComponent,
      ),
  },
];
