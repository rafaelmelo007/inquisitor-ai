import { Component, input } from '@angular/core';
import { MarkdownComponent } from 'ngx-markdown';

@Component({
  standalone: true,
  selector: 'app-markdown-viewer',
  imports: [MarkdownComponent],
  template: `
    <markdown [data]="content()" />
  `,
})
export class MarkdownViewerComponent {
  readonly content = input.required<string>();
}
