import { Component, EventEmitter, Input, Output, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import {
  LucideAngularModule,
  X,
  ExternalLink,
  Calendar,
  CheckCircle2,
  Loader2,
  Save,
  Globe
} from 'lucide-angular';
import { toast } from 'ngx-sonner';
import { DeliverableService } from '../../../core/services/deliverable.service';
import { Deliverable, SubmitPublishProofRequest } from '../../../core/models/deliverable.model';

@Component({
  selector: 'app-publish-proof-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule],
  template: `
    @if (isOpen && deliverable) {
      <div class="fixed inset-0 z-50 overflow-y-auto bg-slate-900/60 backdrop-blur-xs flex items-center justify-center p-4 sm:p-6 animate-in fade-in duration-200">
        <div
          class="bg-white rounded-3xl shadow-2xl border border-slate-100 w-full max-w-md overflow-hidden transform transition-all"
          (click)="$event.stopPropagation()"
        >
          <!-- Header -->
          <div class="px-6 py-5 border-b border-slate-100 flex items-center justify-between bg-slate-50/50">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-2xl bg-emerald-50 text-emerald-600 flex items-center justify-center font-bold">
                <lucide-icon [img]="GlobeIcon" class="w-5 h-5"></lucide-icon>
              </div>
              <div>
                <h3 class="text-lg font-bold text-slate-900">Submit Live Post Proof</h3>
                <p class="text-xs text-slate-500">{{ deliverable.title }}</p>
              </div>
            </div>

            <button
              type="button"
              (click)="onCancel()"
              class="w-8 h-8 rounded-full text-slate-400 hover:text-slate-600 hover:bg-slate-100 flex items-center justify-center transition"
            >
              <lucide-icon [img]="XIcon" class="w-5 h-5"></lucide-icon>
            </button>
          </div>

          <!-- Form -->
          <form [formGroup]="proofForm" (ngSubmit)="onSubmit()" class="p-6 space-y-4">
            <!-- Live Post URL -->
            <div class="space-y-1.5">
              <label class="block text-xs font-semibold text-slate-700">
                Live URL on Platform <span class="text-rose-500">*</span>
              </label>
              <input
                type="url"
                formControlName="liveUrl"
                placeholder="https://www.instagram.com/reel/... or https://tiktok.com/@.../video/..."
                class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-xs focus:outline-hidden focus:ring-2 focus:ring-emerald-500/20 focus:border-emerald-600 transition"
                [class.border-rose-300]="isFieldInvalid('liveUrl')"
              />
              @if (isFieldInvalid('liveUrl')) {
                <p class="text-[11px] text-rose-500 font-medium">Valid live post URL is required.</p>
              }
            </div>

            <!-- Posting Date -->
            <div class="space-y-1.5">
              <label class="block text-xs font-semibold text-slate-700">Actual Posting Date</label>
              <input
                type="date"
                formControlName="postingDate"
                class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-xs focus:outline-hidden focus:ring-2 focus:ring-emerald-500/20 focus:border-emerald-600 transition"
              />
            </div>

            <!-- Footer Actions -->
            <div class="pt-4 border-t border-slate-100 flex items-center justify-end gap-3">
              <button
                type="button"
                (click)="onCancel()"
                [disabled]="isSubmitting()"
                class="px-4 py-2.5 rounded-xl border border-slate-200 text-slate-700 hover:bg-slate-50 text-xs font-semibold transition"
              >
                Cancel
              </button>

              <button
                type="submit"
                [disabled]="isSubmitting() || proofForm.invalid"
                class="px-5 py-2.5 rounded-xl bg-emerald-600 hover:bg-emerald-500 disabled:bg-slate-200 disabled:text-slate-400 text-white text-xs font-semibold shadow-xs transition flex items-center gap-2"
              >
                @if (isSubmitting()) {
                  <lucide-icon [img]="Loader2Icon" class="w-4 h-4 animate-spin"></lucide-icon>
                  <span>Saving...</span>
                } @else {
                  <lucide-icon [img]="SaveIcon" class="w-4 h-4"></lucide-icon>
                  <span>Save Proof</span>
                }
              </button>
            </div>
          </form>
        </div>
      </div>
    }
  `
})
export class PublishProofModalComponent {
  @Input() isOpen = false;
  @Input() deliverable: Deliverable | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<Deliverable>();

  private fb = inject(FormBuilder);
  private deliverableService = inject(DeliverableService);

  readonly isSubmitting = signal<boolean>(false);

  readonly XIcon = X;
  readonly GlobeIcon = Globe;
  readonly ExternalLinkIcon = ExternalLink;
  readonly CalendarIcon = Calendar;
  readonly CheckCircle2Icon = CheckCircle2;
  readonly Loader2Icon = Loader2;
  readonly SaveIcon = Save;

  proofForm: FormGroup = this.fb.group({
    liveUrl: ['', [Validators.required]],
    postingDate: [new Date().toISOString().split('T')[0]]
  });

  isFieldInvalid(fieldName: string): boolean {
    const field = this.proofForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  onCancel(): void {
    this.proofForm.reset({
      liveUrl: '',
      postingDate: new Date().toISOString().split('T')[0]
    });
    this.close.emit();
  }

  onSubmit(): void {
    if (this.proofForm.invalid || !this.deliverable) {
      this.proofForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    const formVal = this.proofForm.value;

    const req: SubmitPublishProofRequest = {
      liveUrl: formVal.liveUrl.trim(),
      postingDate: formVal.postingDate || undefined
    };

    this.deliverableService.submitPublishProof(this.deliverable.id, req).subscribe({
      next: res => {
        this.isSubmitting.set(false);
        toast.success('Live publish proof recorded');
        this.saved.emit(res.data);
        this.onCancel();
      },
      error: err => {
        this.isSubmitting.set(false);
        const msg = err.error?.detail || err.error?.message || 'Failed to submit publish proof';
        toast.error(msg);
      }
    });
  }
}
