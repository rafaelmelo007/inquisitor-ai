import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DatePipe, DecimalPipe } from '@angular/common';
import { SessionsStore } from '../sessions.store';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { ScoreBadgeComponent } from '../../../shared/components/score-badge/score-badge.component';
import { MarkdownViewerComponent } from '../../../shared/components/markdown-viewer/markdown-viewer.component';

@Component({
  standalone: true,
  selector: 'app-session-detail',
  imports: [DatePipe, DecimalPipe, LoadingSpinnerComponent, ScoreBadgeComponent, MarkdownViewerComponent],
  templateUrl: './session-detail.component.html',
})
export class SessionDetailComponent {
  readonly store = inject(SessionsStore);
  private readonly route = inject(ActivatedRoute);

  readonly expandedIndex = signal<number | null>(null);

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      const id = Number(params['id']);
      if (id) {
        this.store.loadById(id);
      }
    });
  }

  toggleAnswer(index: number): void {
    this.expandedIndex.set(this.expandedIndex() === index ? null : index);
  }

  formatDuration(seconds: number | null): string {
    if (seconds === null) return '--';
    const minutes = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${minutes}m ${secs}s`;
  }
}
