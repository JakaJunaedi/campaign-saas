import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-feature-placeholder',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="p-8 bg-white rounded-2xl border border-slate-200/80 shadow-xs text-center space-y-3">
      <div class="w-12 h-12 rounded-2xl bg-indigo-50 text-indigo-600 flex items-center justify-center mx-auto text-lg font-bold">
        {{ title().charAt(0) }}
      </div>
      <h2 class="text-xl font-bold text-slate-900">{{ title() }}</h2>
      <p class="text-sm text-slate-500 max-w-md mx-auto">{{ description() }}</p>
      <div class="inline-flex items-center gap-1.5 px-3 py-1 rounded-full bg-indigo-50 text-indigo-700 text-xs font-semibold">
        Backend API & Module Ready
      </div>
    </div>
  `
})
export class FeaturePlaceholderComponent {
  readonly title = input<string>('Feature Module');
  readonly description = input<string>('This feature is ready on the backend and scheduled for UI completion in upcoming phase.');
}
