---
name: angular-coder
description: Use this skill whenever the user asks to create, scaffold, or modify an Angular frontend application, SPA, or web portal. Triggers include any mention of 'Angular', 'TypeScript', 'component', 'signal', 'store', 'NgRx', 'Tailwind', or requests to build web UIs, dashboards, portals, or single-page applications in Angular. Also use when the user asks to add features, pages, components, services, stores, guards, interceptors, or routes to an existing Angular project. Covers project structure (Vertical Slices), standalone components, Tailwind CSS, Angular signals, NgRx SignalStore, RxJS observables, HTTP interceptors, OAuth flows, and testing with Jasmine/Karma. If the user mentions building a 'frontend', 'web app', 'portal', or 'SPA' and the tech stack is Angular or TypeScript, use this skill.
---

# Angular Frontend Coder

Generate production-ready Angular SPA applications following Vertical Slice Architecture with consistent patterns for standalone components, signals-based state management, Tailwind CSS, and observability.

## TypeScript Rules

Use strict TypeScript. Enable `strict: true` in `tsconfig.json`. Never use `any` — prefer `unknown` for truly dynamic values, or define proper interfaces/types. Use `readonly` for properties that should not change after initialization.

**Prefer interfaces for DTOs/models:**
```typescript
// Correct — interface for a data shape
export interface QuestionnaireDto {
  readonly id: number;
  readonly name: string;
  readonly questionCount: number;
  readonly isPublic: boolean;
  readonly createdByUserId: number;
}

// Wrong — class for a plain data shape
export class QuestionnaireDto { ... }
```

**Use `const` assertions for literal types:**
```typescript
export const OAUTH_PROVIDERS = ['google', 'github', 'linkedin'] as const;
export type OAuthProvider = (typeof OAUTH_PROVIDERS)[number];
```

## Project Structure

Every Angular project follows Vertical Slice Architecture. Every UI component is set to `standalone: true`.

```
<AppName>.Web/
├── src/
│   ├── app/
│   │   ├── app.component.ts
│   │   ├── app.component.html
│   │   ├── app.routes.ts
│   │   ├── app.config.ts
│   │   ├── core/                         # Cross-cutting: auth, HTTP, guards
│   │   │   └── auth/
│   │   │       ├── auth.service.ts
│   │   │       ├── auth.store.ts
│   │   │       ├── token-storage.service.ts
│   │   │       ├── auth.interceptor.ts
│   │   │       ├── token-refresh.interceptor.ts
│   │   │       └── auth.guard.ts
│   │   ├── features/
│   │   │   └── <feature-name>/           # One folder per feature
│   │   │       ├── <feature>.routes.ts
│   │   │       ├── <feature>.service.ts  # HTTP calls — returns Observable<T>
│   │   │       ├── <feature>.store.ts    # NgRx SignalStore — bridges Observable → signal
│   │   │       ├── models/               # Feature-specific interfaces
│   │   │       │   └── <model>.model.ts
│   │   │       └── <sub-component>/      # One folder per component
│   │   │           ├── <component>.component.ts
│   │   │           └── <component>.component.html
│   │   └── shared/
│   │       ├── models/                   # Interfaces shared across features
│   │       └── components/               # Reusable UI components
│   │           └── <component>/
│   │               └── <component>.component.ts
│   ├── environments/
│   │   ├── environment.ts
│   │   └── environment.prod.ts
│   ├── index.html
│   ├── main.ts
│   └── styles.scss
├── angular.json
├── package.json
├── tsconfig.json
├── tailwind.config.js
├── postcss.config.js
├── nginx.conf
└── Dockerfile
```

**Only create subfolders that have files.** `models/` is created only when the feature defines its own interfaces. Features that don't need a service (e.g., static home page) skip `<feature>.service.ts` and `<feature>.store.ts`. Never scaffold empty folders.

### Feature Isolation Rules

- Features **never import from other features**. If two features share a model, move it to `shared/models/`.
- `core/` is the only cross-cutting code. Features may import from `core/` and `shared/` but never the reverse.
- Each feature is lazy-loaded via `loadChildren` in `app.routes.ts`.

## Standalone Components

All components are standalone (Angular default). No `NgModule` anywhere. Imports are declared per-component.

```typescript
@Component({
  standalone: true,
  selector: 'app-questionnaire-list',
  imports: [RouterModule, LoadingSpinnerComponent, ScoreBadgeComponent],
  templateUrl: './questionnaire-list.component.html',
})
export class QuestionnaireListComponent {
  readonly store = inject(QuestionnairesStore);

  ngOnInit(): void {
    this.store.loadAll();
  }
}
```

### Component Rules

- Use `inject()` function — never constructor injection for DI.
- Use `readonly` for all injected dependencies.
- Template files are always separate (`.component.html`) — no inline templates except for trivial shared components (< 5 lines).
- No per-component `.scss` files. All styling uses Tailwind utility classes directly in the template.
- Use Angular's built-in control flow (`@if`, `@for`, `@switch`) — never `*ngIf`, `*ngFor`, `*ngSwitch`.
- Always provide `track` expression in `@for` loops.

```html
@if (store.loading()) {
  <app-loading-spinner />
} @else {
  <div class="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
    @for (item of store.items(); track item.id) {
      <div class="rounded-xl border border-gray-200 bg-white p-4 shadow-sm">
        <h3 class="text-lg font-semibold">{{ item.name }}</h3>
      </div>
    }
  </div>
}
```

## Tailwind CSS

All styling uses Tailwind utility classes. No component-level `.scss` or `.css` files. Only `styles.scss` exists at the project level.

### styles.scss

```scss
@tailwind base;
@tailwind components;
@tailwind utilities;
```

Nothing else goes in this file unless defining a global Tailwind `@layer` (rare).

### tailwind.config.js

```javascript
/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ['./src/**/*.{html,ts}'],
  theme: {
    extend: {
      // Brand colours, custom spacing, etc.
    },
  },
  plugins: [],
};
```

### postcss.config.js

```javascript
module.exports = {
  plugins: {
    tailwindcss: {},
    autoprefixer: {},
  },
};
```

### Responsive Design

Use Tailwind's responsive prefixes (`sm:`, `md:`, `lg:`, `xl:`) for all breakpoint-dependent layouts. Mobile-first: base classes apply to the smallest viewport.

```html
<div class="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
  <!-- Cards scale from 1 to 4 columns -->
</div>
```

## Signals

Angular signals are the primary reactive primitive for component state. Components read signals directly in templates — never subscribe to observables in components.

### Signal Rules

- Store state is exposed as signals via `@ngrx/signals`.
- Components read signals with `store.property()` in templates.
- **No `async` pipe** — signals replace the need for it.
- Use `computed()` for derived state.
- Use `effect()` sparingly — only for side effects like syncing form values from signals.
- Use `input()` and `output()` signal-based APIs for component I/O (not `@Input()` / `@Output()` decorators).

```typescript
// Signal-based component inputs (Angular 17.1+)
@Component({ ... })
export class ScoreBadgeComponent {
  readonly classification = input.required<'Approved' | 'ApprovedWithReservations' | 'Failed'>();

  readonly badgeClass = computed(() => {
    switch (this.classification()) {
      case 'Approved': return 'bg-green-100 text-green-800';
      case 'ApprovedWithReservations': return 'bg-orange-100 text-orange-800';
      case 'Failed': return 'bg-red-100 text-red-800';
    }
  });
}
```

## Observables (RxJS)

Services return `Observable<T>` from `HttpClient`. Observables are the transport layer — they flow from services into stores, where `rxMethod` bridges them to signals.

### Observable Rules

- Services **never call `.subscribe()`** — that's the store's job (via `rxMethod`).
- Components **never call `.subscribe()`** — they read signals from the store.
- Use `pipe()` operators (`map`, `switchMap`, `catchError`, `tap`, `filter`, `debounceTime`, `distinctUntilChanged`, `forkJoin`, `combineLatest`) — import from `rxjs` and `rxjs/operators`.
- For one-off HTTP calls that don't need store state, a component may use `toSignal()` from `@angular/core/rxjs-interop`.
- Never store `Subscription` objects manually. If you must subscribe (rare), use `takeUntilDestroyed()` from `@angular/core/rxjs-interop`.

```typescript
// Service — returns Observable, nothing else
@Injectable({ providedIn: 'root' })
export class QuestionnairesService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = inject(ENVIRONMENT).apiBaseUrl;

  getAll(): Observable<QuestionnaireDto[]> {
    return this.http.get<QuestionnaireDto[]>(`${this.apiUrl}/questionnaires`);
  }

  getById(id: number): Observable<QuestionnaireDto> {
    return this.http.get<QuestionnaireDto>(`${this.apiUrl}/questionnaires/${id}`);
  }

  import(file: File, isPublic: boolean): Observable<QuestionnaireDto> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('isPublic', String(isPublic));
    return this.http.post<QuestionnaireDto>(`${this.apiUrl}/questionnaires`, formData);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/questionnaires/${id}`);
  }
}
```

## Stores (`@ngrx/signals`)

Every feature that fetches data has a `signalStore()`. The store is the **single source of truth** for that feature's state. It bridges `Observable<T>` from the service into signals consumed by components.

### Store Pattern

```typescript
import { computed, inject } from '@angular/core';
import { signalStore, withState, withComputed, withMethods, patchState } from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap, map } from 'rxjs';
import { tapResponse } from '@ngrx/operators';

interface QuestionnairesState {
  questionnaires: QuestionnaireDto[];
  selected: QuestionnaireDto | null;
  loading: boolean;
  error: string | null;
}

export const QuestionnairesStore = signalStore(
  { providedIn: 'root' },
  withState<QuestionnairesState>({
    questionnaires: [],
    selected: null,
    loading: false,
    error: null,
  }),
  withComputed(({ questionnaires }) => ({
    count: computed(() => questionnaires().length),
  })),
  withMethods((store, service = inject(QuestionnairesService)) => ({
    loadAll: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap(() => service.getAll()),
        tapResponse({
          next: (questionnaires) => patchState(store, { questionnaires, loading: false }),
          error: (err: unknown) => patchState(store, { error: String(err), loading: false }),
        }),
      ),
    ),
    loadById: rxMethod<number>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap((id) => service.getById(id)),
        tapResponse({
          next: (selected) => patchState(store, { selected, loading: false }),
          error: (err: unknown) => patchState(store, { error: String(err), loading: false }),
        }),
      ),
    ),
    import: rxMethod<{ file: File; isPublic: boolean }>(
      pipe(
        switchMap(({ file, isPublic }) => service.import(file, isPublic)),
        tapResponse({
          next: (q) =>
            patchState(store, (s) => ({
              questionnaires: [...s.questionnaires, q],
            })),
          error: (err: unknown) => patchState(store, { error: String(err) }),
        }),
      ),
    ),
    remove: rxMethod<number>(
      pipe(
        switchMap((id) => service.delete(id).pipe(map(() => id))),
        tapResponse({
          next: (id) =>
            patchState(store, (s) => ({
              questionnaires: s.questionnaires.filter((q) => q.id !== id),
            })),
          error: (err: unknown) => patchState(store, { error: String(err) }),
        }),
      ),
    ),
  })),
);
```

### Store Rules

- Every store has `loading: boolean` and `error: string | null` in state.
- Use `rxMethod<T>` for all async operations that call a service.
- Use `tapResponse` (from `@ngrx/operators`) for success/error handling inside `rxMethod` — never raw `catchError`.
- Use `patchState` to update state immutably — never mutate state directly.
- Stores are `{ providedIn: 'root' }` — singleton per app.
- Stores never import other stores. If a store needs data from another, the component orchestrates.
- Error type in `tapResponse` must be `unknown`, then cast with `String(err)`.

## Data Flow Pattern

The complete data flow is always:

```
Component → injects Store → rxMethod calls Service → Service calls HttpClient
                ↓
         Store updates signals via patchState
                ↓
         Component reads signals in template
```

Never skip a layer. Components never call services directly (unless using `toSignal()` for a one-off read that doesn't need store state). Stores never call `HttpClient` directly — always through a service.

## Routing

### app.routes.ts — Lazy Loading

Every feature's routes are lazy-loaded. Use `loadChildren` for features with child routes, `loadComponent` for single-component features.

```typescript
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
    path: 'leaderboard',
    loadChildren: () =>
      import('./features/leaderboard/leaderboard.routes').then((m) => m.LEADERBOARD_ROUTES),
  },
  { path: '**', redirectTo: '' },
];
```

### Feature Routes

Each feature exports a `const` routes array:

```typescript
// features/questionnaires/questionnaires.routes.ts
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
```

### Route Rules

- Export routes as `UPPER_SNAKE_CASE` const (e.g., `QUESTIONNAIRES_ROUTES`).
- Use `loadComponent` for leaf routes, `loadChildren` for feature groups.
- Apply `canActivate: [authGuard]` at the parent level — children inherit it.
- Route params are read via `ActivatedRoute` signals (`route.params`, `route.queryParams`).

## Authentication (Core)

Auth lives in `core/` because it's cross-cutting — consumed by routes (guard), HTTP (interceptors), and the app shell (nav bar).

### token-storage.service.ts

```typescript
@Injectable({ providedIn: 'root' })
export class TokenStorageService {
  private readonly ACCESS_KEY = 'inq_access_token';
  private readonly REFRESH_KEY = 'inq_refresh_token';

  getAccessToken(): string | null {
    return localStorage.getItem(this.ACCESS_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_KEY);
  }

  saveTokens(accessToken: string, refreshToken: string): void {
    localStorage.setItem(this.ACCESS_KEY, accessToken);
    localStorage.setItem(this.REFRESH_KEY, refreshToken);
  }

  clear(): void {
    localStorage.removeItem(this.ACCESS_KEY);
    localStorage.removeItem(this.REFRESH_KEY);
  }
}
```

### auth.service.ts

```typescript
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = inject(ENVIRONMENT).apiBaseUrl;

  login(provider: OAuthProvider): void {
    window.location.href = `${this.apiUrl}/auth/${provider}`;
  }

  logout(): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/auth/logout`, {});
  }

  refresh(refreshToken: string): Observable<TokenResponse> {
    return this.http.post<TokenResponse>(`${this.apiUrl}/auth/refresh`, { refreshToken });
  }
}
```

### auth.interceptor.ts

```typescript
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenStorage = inject(TokenStorageService);
  const token = tokenStorage.getAccessToken();

  // Skip auth header for refresh and public endpoints
  if (req.url.includes('/auth/refresh') || req.url.includes('/auth/logout')) {
    return next(req);
  }

  if (token) {
    req = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` },
    });
  }

  return next(req);
};
```

### token-refresh.interceptor.ts

```typescript
export const tokenRefreshInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenStorage = inject(TokenStorageService);
  const authService = inject(AuthService);
  const authStore = inject(AuthStore);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status !== 401 || req.url.includes('/auth/refresh')) {
        return throwError(() => error);
      }

      const refreshToken = tokenStorage.getRefreshToken();
      if (!refreshToken) {
        authStore.clearSession();
        router.navigate(['/login']);
        return throwError(() => error);
      }

      return authService.refresh(refreshToken).pipe(
        switchMap((tokens) => {
          tokenStorage.saveTokens(tokens.accessToken, tokens.refreshToken);
          return next(
            req.clone({
              setHeaders: { Authorization: `Bearer ${tokens.accessToken}` },
            }),
          );
        }),
        catchError((refreshError) => {
          authStore.clearSession();
          router.navigate(['/login']);
          return throwError(() => refreshError);
        }),
      );
    }),
  );
};
```

### auth.guard.ts

```typescript
export const authGuard: CanActivateFn = () => {
  const authStore = inject(AuthStore);
  const router = inject(Router);

  if (authStore.isAuthenticated()) {
    return true;
  }

  router.navigate(['/login']);
  return false;
};
```

### auth.store.ts

```typescript
import { jwtDecode } from 'jwt-decode';

interface AuthState {
  currentUser: UserDto | null;
  loading: boolean;
}

export const AuthStore = signalStore(
  { providedIn: 'root' },
  withState<AuthState>({ currentUser: null, loading: false }),
  withComputed(({ currentUser }) => ({
    isAuthenticated: computed(() => currentUser() !== null),
  })),
  withMethods(
    (store, tokenStorage = inject(TokenStorageService), authService = inject(AuthService), router = inject(Router)) => ({
      restoreSession(): void {
        const token = tokenStorage.getAccessToken();
        if (!token) {
          patchState(store, { currentUser: null });
          return;
        }
        try {
          const decoded = jwtDecode<JwtPayload>(token);
          patchState(store, {
            currentUser: {
              id: Number(decoded.sub),
              email: decoded.email,
              displayName: decoded.name,
              avatarUrl: decoded.avatar_url ?? null,
              provider: decoded.provider,
            },
          });
        } catch {
          tokenStorage.clear();
          patchState(store, { currentUser: null });
        }
      },
      clearSession(): void {
        tokenStorage.clear();
        patchState(store, { currentUser: null });
      },
      logout: rxMethod<void>(
        pipe(
          switchMap(() => authService.logout()),
          tapResponse({
            next: () => {
              tokenStorage.clear();
              patchState(store, { currentUser: null });
              router.navigate(['/']);
            },
            error: () => {
              tokenStorage.clear();
              patchState(store, { currentUser: null });
              router.navigate(['/']);
            },
          }),
        ),
      ),
    }),
  ),
);
```

## App Configuration

### app.config.ts

```typescript
import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { routes } from './app.routes';
import { authInterceptor } from './core/auth/auth.interceptor';
import { tokenRefreshInterceptor } from './core/auth/token-refresh.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor, tokenRefreshInterceptor])),
    provideAnimationsAsync(),
  ],
};
```

### app.component.ts

```typescript
@Component({
  standalone: true,
  selector: 'app-root',
  imports: [RouterModule],
  templateUrl: './app.component.html',
})
export class AppComponent {
  readonly authStore = inject(AuthStore);

  ngOnInit(): void {
    this.authStore.restoreSession();
  }
}
```

### Environment Configuration

Use Angular's `environment.ts` files. Provide the environment object as an injection token:

```typescript
// environments/environment.ts
export const environment = {
  apiBaseUrl: 'http://localhost:8080',
  downloadUrl: 'https://github.com/...',
};

// Injection token — defined in a shared file
import { InjectionToken } from '@angular/core';
export const ENVIRONMENT = new InjectionToken<typeof environment>('environment');

// Provided in app.config.ts
import { environment } from '../environments/environment';
providers: [
  { provide: ENVIRONMENT, useValue: environment },
  // ...
]
```

Services inject `ENVIRONMENT` — never import `environment.ts` directly:

```typescript
private readonly apiUrl = inject(ENVIRONMENT).apiBaseUrl;
```

## HTTP Error Handling

### API Error Response Shape

Backend returns errors as: `{ errors: string[] }`. Services don't handle errors — stores handle them in `tapResponse`.

### Store Error Pattern

```typescript
tapResponse({
  next: (data) => patchState(store, { data, loading: false }),
  error: (err: unknown) => {
    const message = err instanceof HttpErrorResponse
      ? (err.error?.errors?.join(', ') ?? err.message)
      : String(err);
    patchState(store, { error: message, loading: false });
  },
}),
```

### Global Error Display

Components read `store.error()` and display it. No global toast service required — each feature handles its own errors inline with Tailwind alert styling:

```html
@if (store.error()) {
  <div class="rounded-lg border border-red-200 bg-red-50 p-4 text-sm text-red-700">
    {{ store.error() }}
  </div>
}
```

## Forms

Use Angular Reactive Forms (`FormGroup`, `FormControl`) for any user input. Import `ReactiveFormsModule` in the component.

### Form Rules

- Define `FormGroup` in the component class with typed controls.
- Use `Validators` from `@angular/forms` for client-side validation.
- Sync store signal → form using `effect()` (e.g., populate form when profile loads).
- On submit: extract form value and pass to store method.
- Disable submit button while `store.saving()` is true.

```typescript
@Component({
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './profile.component.html',
})
export class ProfileComponent {
  readonly store = inject(ProfileStore);

  readonly form = new FormGroup({
    displayName: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(200)] }),
  });

  constructor() {
    effect(() => {
      const profile = this.store.profile();
      if (profile) {
        this.form.patchValue({ displayName: profile.displayName });
      }
    });
  }

  ngOnInit(): void {
    this.store.load();
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.store.save(this.form.getRawValue());
    }
  }
}
```

```html
<form [formGroup]="form" (ngSubmit)="onSubmit()" class="space-y-4">
  <div>
    <label for="displayName" class="block text-sm font-medium text-gray-700">Display Name</label>
    <input id="displayName" formControlName="displayName" type="text"
           class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm" />
    @if (form.controls.displayName.errors?.['required'] && form.controls.displayName.touched) {
      <p class="mt-1 text-sm text-red-600">Display name is required.</p>
    }
  </div>
  <button type="submit" [disabled]="store.saving() || form.invalid"
          class="rounded-md bg-indigo-600 px-4 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 disabled:opacity-50">
    @if (store.saving()) { Saving... } @else { Save }
  </button>
</form>
```

## Shared Components

Shared components live in `shared/components/`. They are stateless, input-driven, and reusable across features.

### Rules

- Every shared component is standalone.
- Use `input()` and `output()` signal APIs for all component I/O.
- No service injection in shared components — they receive all data via inputs.
- Keep templates minimal — shared components are UI primitives, not feature logic.

```typescript
// shared/components/loading-spinner/loading-spinner.component.ts
@Component({
  standalone: true,
  selector: 'app-loading-spinner',
  template: `
    <div class="flex items-center justify-center p-8">
      <div class="h-8 w-8 animate-spin rounded-full border-4 border-indigo-200 border-t-indigo-600"></div>
      @if (message()) {
        <span class="ml-3 text-sm text-gray-500">{{ message() }}</span>
      }
    </div>
  `,
})
export class LoadingSpinnerComponent {
  readonly message = input<string>();
}
```

```typescript
// shared/components/score-badge/score-badge.component.ts
@Component({
  standalone: true,
  selector: 'app-score-badge',
  template: `
    <span [class]="'inline-flex items-center rounded-full px-3 py-0.5 text-xs font-medium ' + badgeClass()">
      {{ label() }}
    </span>
  `,
})
export class ScoreBadgeComponent {
  readonly classification = input.required<'Approved' | 'ApprovedWithReservations' | 'Failed'>();

  readonly label = computed(() => {
    switch (this.classification()) {
      case 'Approved': return 'Approved';
      case 'ApprovedWithReservations': return 'With Reservations';
      case 'Failed': return 'Failed';
    }
  });

  readonly badgeClass = computed(() => {
    switch (this.classification()) {
      case 'Approved': return 'bg-green-100 text-green-800';
      case 'ApprovedWithReservations': return 'bg-orange-100 text-orange-800';
      case 'Failed': return 'bg-red-100 text-red-800';
    }
  });
}
```

## Testing

### Conventions

- Use **Jasmine** as the test framework and **Karma** as the test runner (Angular defaults).
- Every component, service, store, guard, and interceptor gets at least one test.
- Test files are co-located: `component.spec.ts` next to `component.ts`.
- Use `TestBed.configureTestingModule()` with standalone component imports.
- Mock services with Jasmine spies: `jasmine.createSpyObj<T>('name', ['method1', 'method2'])`.
- Mock stores by providing a fake `signalStore` or overriding with `{ provide: Store, useValue: mockStore }`.
- For HTTP tests, use `provideHttpClientTesting` + `HttpTestingController`.

### Service Test

```typescript
describe('QuestionnairesService', () => {
  let service: QuestionnairesService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        QuestionnairesService,
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: ENVIRONMENT, useValue: { apiBaseUrl: 'http://test-api' } },
      ],
    });
    service = TestBed.inject(QuestionnairesService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('should GET all questionnaires', () => {
    const expected: QuestionnaireDto[] = [{ id: 1, name: 'Test', questionCount: 5, isPublic: true, createdByUserId: 1 }];

    service.getAll().subscribe((result) => {
      expect(result).toEqual(expected);
    });

    const req = httpTesting.expectOne('http://test-api/questionnaires');
    expect(req.request.method).toBe('GET');
    req.flush(expected);
  });
});
```

### Component Test

```typescript
describe('QuestionnaireListComponent', () => {
  let fixture: ComponentFixture<QuestionnaireListComponent>;
  let mockStore: jasmine.SpyObj<InstanceType<typeof QuestionnairesStore>>;

  beforeEach(async () => {
    mockStore = jasmine.createSpyObj('QuestionnairesStore', ['loadAll', 'remove'], {
      questionnaires: signal<QuestionnaireDto[]>([]),
      loading: signal(false),
      error: signal<string | null>(null),
    });

    await TestBed.configureTestingModule({
      imports: [QuestionnaireListComponent],
      providers: [{ provide: QuestionnairesStore, useValue: mockStore }],
    }).compileComponents();

    fixture = TestBed.createComponent(QuestionnaireListComponent);
    fixture.detectChanges();
  });

  it('should call loadAll on init', () => {
    expect(mockStore.loadAll).toHaveBeenCalled();
  });
});
```

### Guard Test

```typescript
describe('authGuard', () => {
  it('should return true when authenticated', () => {
    TestBed.configureTestingModule({
      providers: [
        { provide: AuthStore, useValue: { isAuthenticated: signal(true) } },
        { provide: Router, useValue: jasmine.createSpyObj('Router', ['navigate']) },
      ],
    });

    const result = TestBed.runInInjectionContext(() => authGuard({} as any, {} as any));
    expect(result).toBe(true);
  });
});
```

### Interceptor Test

```typescript
describe('authInterceptor', () => {
  it('should add Authorization header when token exists', () => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([authInterceptor])),
        provideHttpClientTesting(),
        { provide: TokenStorageService, useValue: { getAccessToken: () => 'test-token' } },
      ],
    });

    const http = TestBed.inject(HttpClient);
    const httpTesting = TestBed.inject(HttpTestingController);

    http.get('/test').subscribe();

    const req = httpTesting.expectOne('/test');
    expect(req.request.headers.get('Authorization')).toBe('Bearer test-token');
    req.flush({});
  });
});
```

## Docker

### nginx.conf

```nginx
server {
    listen 80;
    server_name _;
    root /usr/share/nginx/html;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }

    location ~* \.(?:js|css|woff2?|svg|png|jpg|jpeg|gif|ico)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }
}
```

### Dockerfile

```dockerfile
# Stage 1: Build
FROM node:22-alpine AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
RUN npx ng build --configuration production

# Stage 2: Serve
FROM nginx:alpine AS runtime
COPY --from=build /app/dist/<app-name>/browser/ /usr/share/nginx/html/
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
```

### docker-compose.yml (Web service entry)

```yaml
  web:
    build:
      context: ./<AppName>.Web
      dockerfile: Dockerfile
    ports:
      - "4200:80"
    depends_on:
      - api
```

## npm Packages (latest stable versions)

Always target the latest stable Angular and package versions:

| Purpose | Package |
|---|---|
| Framework | `@angular/core`, `@angular/common`, `@angular/router`, `@angular/forms`, `@angular/animations` |
| HTTP | `@angular/common/http` (built-in) |
| State management | `@ngrx/signals` |
| RxJS utilities | `@ngrx/operators` |
| CSS framework | `tailwindcss`, `postcss`, `autoprefixer` |
| Markdown rendering | `ngx-markdown` |
| JWT decoding | `jwt-decode` |
| Test framework | Jasmine (built-in with Angular CLI) |
| Test runner | Karma (built-in with Angular CLI) |
| E2E (optional) | Cypress or Playwright |
