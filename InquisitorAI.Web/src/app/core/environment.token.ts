import { InjectionToken } from '@angular/core';

export interface Environment {
  readonly apiBaseUrl: string;
  readonly downloadUrl: string;
}

export const ENVIRONMENT = new InjectionToken<Environment>('environment');
