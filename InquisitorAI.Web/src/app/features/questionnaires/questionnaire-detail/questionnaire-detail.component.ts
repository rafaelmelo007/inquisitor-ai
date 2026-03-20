import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { QuestionnairesStore } from '../questionnaires.store';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  standalone: true,
  selector: 'app-questionnaire-detail',
  imports: [LoadingSpinnerComponent],
  templateUrl: './questionnaire-detail.component.html',
})
export class QuestionnaireDetailComponent {
  readonly store = inject(QuestionnairesStore);
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

  toggleQuestion(index: number): void {
    this.expandedIndex.set(this.expandedIndex() === index ? null : index);
  }
}
