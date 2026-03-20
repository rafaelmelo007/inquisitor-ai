import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { QuestionnairesStore } from '../questionnaires.store';
import { AuthStore } from '../../../core/auth/auth.store';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  standalone: true,
  selector: 'app-questionnaire-list',
  imports: [RouterModule, LoadingSpinnerComponent],
  templateUrl: './questionnaire-list.component.html',
})
export class QuestionnaireListComponent {
  readonly store = inject(QuestionnairesStore);
  readonly authStore = inject(AuthStore);

  selectedFile: File | null = null;
  isPublic = false;

  ngOnInit(): void {
    this.store.loadAll();
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedFile = input.files?.[0] ?? null;
  }

  onUpload(): void {
    if (this.selectedFile) {
      this.store.import({ file: this.selectedFile, isPublic: this.isPublic });
      this.selectedFile = null;
    }
  }

  onDelete(id: number): void {
    this.store.remove(id);
  }
}
