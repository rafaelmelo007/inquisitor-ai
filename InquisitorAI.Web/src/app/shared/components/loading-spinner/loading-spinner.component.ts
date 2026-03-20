import { Component, input } from '@angular/core';

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
