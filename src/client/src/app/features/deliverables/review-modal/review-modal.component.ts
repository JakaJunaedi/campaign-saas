import { Component, EventEmitter, Input, Output, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  LucideAngularModule,
  X,
  CheckCircle2,
  RefreshCw,
  XCircle,
  FileVideo,
  FileImage,
  ExternalLink,
  MessageSquare,
  Loader2,
  AlertTriangle,
  Send
} from 'lucide-angular';
import { toast } from 'ngx-sonner';
import { ApprovalService } from '../../../core/services/approval.service';
import { Deliverable, ContentSubmission } from '../../../core/models/deliverable.model';
import { ApprovalReview, ReviewDecision } from '../../../core/models/approval.model';

@Component({
  selector: 'app-review-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  template: `
    @if (isOpen && submission && deliverable) {
      <div class="fixed inset-0 z-50 overflow-y-auto bg-slate-900/60 backdrop-blur-xs flex items-center justify-center p-4 sm:p-6 animate-in fade-in duration-200">
        <div
          class="bg-white rounded-3xl shadow-2xl border border-slate-100 w-full max-w-2xl overflow-hidden transform transition-all"
          (click)="$event.stopPropagation()"
        >
          <!-- Header -->
          <div class="px-6 py-5 border-b border-slate-100 flex items-center justify-between bg-slate-50/50">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-2xl bg-indigo-50 text-indigo-600 flex items-center justify-center font-bold">
                <lucide-icon [img]="MessageSquareIcon" class="w-5 h-5"></lucide-icon>
              </div>
              <div>
                <h3 class="text-lg font-bold text-slate-900">
                  Review Submission (v{{ submission.versionNumber }})
                </h3>
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

          <!-- Body -->
          <div class="p-6 space-y-6 max-h-[75vh] overflow-y-auto">
            <!-- Media File Preview Card -->
            <div class="p-4 rounded-2xl bg-slate-900 text-white space-y-3">
              <div class="flex items-center justify-between">
                <span class="text-xs font-bold text-slate-300">Submitted Media Asset</span>
                @if (submission.downloadUrl) {
                  <a
                    [href]="submission.downloadUrl"
                    target="_blank"
                    class="inline-flex items-center gap-1 text-[11px] font-bold text-indigo-300 hover:text-white transition"
                  >
                    <span>Open in MinIO Storage</span>
                    <lucide-icon [img]="ExternalLinkIcon" class="w-3.5 h-3.5"></lucide-icon>
                  </a>
                }
              </div>

              <div class="p-3 bg-slate-800 rounded-xl flex items-center justify-between text-xs">
                <div class="flex items-center gap-2.5 truncate max-w-[320px]">
                  <lucide-icon [img]="FileVideoIcon" class="w-4 h-4 text-indigo-400 shrink-0"></lucide-icon>
                  <span class="font-semibold truncate">{{ submission.mediaFileName }}</span>
                </div>
                <span class="text-[11px] text-slate-400 shrink-0">{{ formatFileSize(submission.mediaFileSize) }}</span>
              </div>

              @if (submission.caption) {
                <div class="p-3 bg-slate-800/80 rounded-xl space-y-1">
                  <span class="text-[10px] font-bold text-slate-400 uppercase tracking-wider block">Submitted Caption / Copy</span>
                  <p class="text-xs text-slate-200 whitespace-pre-line leading-relaxed">{{ submission.caption }}</p>
                </div>
              }
            </div>

            <!-- Review Decision Selector -->
            <div class="space-y-2">
              <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider">
                Review Decision <span class="text-rose-500">*</span>
              </label>

              <div class="grid grid-cols-1 sm:grid-cols-3 gap-3">
                <!-- Approve -->
                <button
                  type="button"
                  (click)="selectedDecision.set('Approved')"
                  class="p-4 rounded-2xl border-2 text-left transition flex flex-col justify-between space-y-2"
                  [class]="selectedDecision() === 'Approved'
                    ? 'border-emerald-500 bg-emerald-50/50 text-emerald-900 shadow-xs'
                    : 'border-slate-200 hover:border-slate-300 text-slate-700'"
                >
                  <div class="flex items-center justify-between">
                    <span class="text-xs font-bold">Approve</span>
                    <lucide-icon [img]="CheckCircle2Icon" class="w-4 h-4 text-emerald-600"></lucide-icon>
                  </div>
                  <p class="text-[11px] text-slate-500">Content is approved and ready for publishing.</p>
                </button>

                <!-- Request Revision -->
                <button
                  type="button"
                  (click)="selectedDecision.set('RevisionRequested')"
                  class="p-4 rounded-2xl border-2 text-left transition flex flex-col justify-between space-y-2"
                  [class]="selectedDecision() === 'RevisionRequested'
                    ? 'border-amber-500 bg-amber-50/50 text-amber-900 shadow-xs'
                    : 'border-slate-200 hover:border-slate-300 text-slate-700'"
                >
                  <div class="flex items-center justify-between">
                    <span class="text-xs font-bold">Request Revision</span>
                    <lucide-icon [img]="RefreshCwIcon" class="w-4 h-4 text-amber-600"></lucide-icon>
                  </div>
                  <p class="text-[11px] text-slate-500">Provide feedback notes for changes.</p>
                </button>

                <!-- Reject -->
                <button
                  type="button"
                  (click)="selectedDecision.set('Rejected')"
                  class="p-4 rounded-2xl border-2 text-left transition flex flex-col justify-between space-y-2"
                  [class]="selectedDecision() === 'Rejected'
                    ? 'border-rose-500 bg-rose-50/50 text-rose-900 shadow-xs'
                    : 'border-slate-200 hover:border-slate-300 text-slate-700'"
                >
                  <div class="flex items-center justify-between">
                    <span class="text-xs font-bold">Reject</span>
                    <lucide-icon [img]="XCircleIcon" class="w-4 h-4 text-rose-600"></lucide-icon>
                  </div>
                  <p class="text-[11px] text-slate-500">Submission does not meet requirements.</p>
                </button>
              </div>
            </div>

            <!-- Feedback Notes Textarea -->
            <div class="space-y-1.5">
              <label class="block text-xs font-semibold text-slate-700">
                Feedback Notes & Instructions
                @if (selectedDecision() === 'RevisionRequested') {
                  <span class="text-rose-500">* (Required for revision)</span>
                }
              </label>
              <textarea
                [(ngModel)]="feedbackNotes"
                rows="4"
                placeholder="Give constructive feedback: what needs adjusting (audio, timing, branding, hashtags)..."
                class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-xs focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition"
              ></textarea>
            </div>
          </div>

          <!-- Footer Actions -->
          <div class="p-4 bg-slate-50 border-t border-slate-100 flex items-center justify-end gap-3">
            <button
              type="button"
              (click)="onCancel()"
              [disabled]="isSubmitting()"
              class="px-4 py-2 rounded-xl border border-slate-200 text-slate-700 hover:bg-slate-100 text-xs font-semibold transition"
            >
              Cancel
            </button>

            <button
              type="button"
              (click)="onSubmitReview()"
              [disabled]="isSubmitting() || (selectedDecision() === 'RevisionRequested' && !feedbackNotes.trim())"
              class="px-5 py-2 rounded-xl bg-indigo-600 hover:bg-indigo-500 disabled:bg-slate-200 disabled:text-slate-400 text-white text-xs font-semibold shadow-xs transition flex items-center gap-2"
            >
              @if (isSubmitting()) {
                <lucide-icon [img]="Loader2Icon" class="w-4 h-4 animate-spin"></lucide-icon>
                <span>Submitting Review...</span>
              } @else {
                <lucide-icon [img]="SendIcon" class="w-4 h-4"></lucide-icon>
                <span>Submit Decision</span>
              }
            </button>
          </div>
        </div>
      </div>
    }
  `
})
export class ReviewModalComponent {
  @Input() isOpen = false;
  @Input() submission: ContentSubmission | null = null;
  @Input() deliverable: Deliverable | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() reviewed = new EventEmitter<ApprovalReview>();

  private approvalService = inject(ApprovalService);

  readonly selectedDecision = signal<ReviewDecision>('Approved');
  readonly isSubmitting = signal<boolean>(false);
  feedbackNotes = '';

  readonly XIcon = X;
  readonly CheckCircle2Icon = CheckCircle2;
  readonly RefreshCwIcon = RefreshCw;
  readonly XCircleIcon = XCircle;
  readonly FileVideoIcon = FileVideo;
  readonly FileImageIcon = FileImage;
  readonly ExternalLinkIcon = ExternalLink;
  readonly MessageSquareIcon = MessageSquare;
  readonly Loader2Icon = Loader2;
  readonly AlertTriangleIcon = AlertTriangle;
  readonly SendIcon = Send;

  formatFileSize(bytes: number): string {
    if (bytes >= 1_000_000) {
      return (bytes / 1_000_000).toFixed(1) + ' MB';
    }
    if (bytes >= 1_000) {
      return (bytes / 1_000).toFixed(1) + ' KB';
    }
    return bytes + ' B';
  }

  onCancel(): void {
    this.feedbackNotes = '';
    this.selectedDecision.set('Approved');
    this.close.emit();
  }

  onSubmitReview(): void {
    const currentSub = this.submission;
    if (!currentSub) return;

    this.isSubmitting.set(true);

    this.approvalService
      .submitReview(currentSub.id, {
        decision: this.selectedDecision(),
        feedbackNotes: this.feedbackNotes.trim() || undefined
      })
      .subscribe({
        next: res => {
          this.isSubmitting.set(false);
          toast.success(`Review decision recorded: ${res.data.decision}`);
          this.reviewed.emit(res.data);
          this.onCancel();
        },
        error: err => {
          this.isSubmitting.set(false);
          const msg = err.error?.detail || err.error?.message || 'Failed to submit review';
          toast.error(msg);
        }
      });
  }
}
