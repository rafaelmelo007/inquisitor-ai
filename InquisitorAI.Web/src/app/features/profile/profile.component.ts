import { Component, effect, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProfileStore } from './profile.store';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  standalone: true,
  selector: 'app-profile',
  imports: [ReactiveFormsModule, LoadingSpinnerComponent],
  templateUrl: './profile.component.html',
})
export class ProfileComponent {
  readonly store = inject(ProfileStore);

  readonly form = new FormGroup({
    displayName: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(200)],
    }),
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
