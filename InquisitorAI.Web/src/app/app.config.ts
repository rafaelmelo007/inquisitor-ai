import { ApplicationConfig, provideExperimentalZonelessChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideMarkdown } from 'ngx-markdown';
import { routes } from './app.routes';
import { authInterceptor } from './core/auth/auth.interceptor';
import { tokenRefreshInterceptor } from './core/auth/token-refresh.interceptor';
import { ENVIRONMENT } from './core/environment.token';
import { environment } from '../environments/environment';

export const appConfig: ApplicationConfig = {
  providers: [
    provideExperimentalZonelessChangeDetection(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor, tokenRefreshInterceptor])),
    provideAnimationsAsync(),
    { provide: ENVIRONMENT, useValue: environment },
    provideMarkdown(),
  ],
};
