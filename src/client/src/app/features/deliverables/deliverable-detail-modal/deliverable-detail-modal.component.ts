import { Component, EventEmitter, Input, Output, OnInit, inject, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  LucideAngularModule,
  X,
  FileCheck2,
  Calendar,
  Layers,
  UploadCloud,
  MessageSquare,
  ExternalLink,
  CheckCircle2,
  RefreshCw,
  XCircle,
  FileVideo,
  FileImage,
  File,
  Clock,
  Loader2,
  Globe,
  Plus
} from 'lucide-angular';
import { DeliverableService } from '../../../core/services/deliverable.service';
import { ApprovalService } from '../../../core/services/approval.service';
import { Deliverable, ContentSubmission } from '../../../core/models/deliverable.model';
import { ApprovalReview } from '../../../core/models/approval.model';
import { CreatorSummary } from '../../../core/models/creator.model';

@Component({
  selector: 'app-deliverable-detail-modal',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  template: `
    @if (isOpen && deliverable) {
      <div class="fixed inset-0 z-50 overflow-y-auto bg-slate-900/60 backdrop-blur-xs flex items-center justify-center p-4 sm:p-6 animate-in fade-in duration-200">
        <div
          class="bg-white rounded-3xl shadow-2xl border border-slate-100 w-full max-w-3xl overflow-hidden transform transition-all"
          (click)="$event.stopPropagation()"
        >
          <!-- Header Banner -->
          <div class="p-6 bg-gradient-to-r from-slate-900 via-indigo-950 to-slate-900 text-white relative">
            <button
              type="button"
              (click)="close.emit()"
              class="absolute top-5 right-5 w-8 h-8 rounded-full text-slate-400 hover:text-white hover:bg-white/10 flex items-center justify-center transition"
            >
              <lucide-icon [img]="XIcon" class="w-5 h-5"></lucide-icon>
            </button>

            <div class="space-y-3">
              <div class="flex flex-wrap items-center gap-2">
                <span
                  class="px-2.5 py-0.5 rounded-full text-xs font-bold uppercase tracking-wider"
                  [ngClass]="getStatusBadgeClass(deliverable.status)"
                >
                  {{ deliverable.status }}
                </span>

                <span class="px-2.5 py-0.5 rounded-full bg-white/10 text-indigo-200 text-xs font-semibold">
                  {{ deliverable.platform }} • {{ deliverable.contentType }}
                </span>

                <span class="px-2.5 py-0.5 rounded-full bg-indigo-500/20 text-indigo-300 text-xs font-semibold">
                  v{{ deliverable.latestVersion }} Latest
                </span>
              </div>

              <h2 class="text-xl font-black text-white tracking-tight">{{ deliverable.title }}</h2>

              <div class="flex flex-wrap items-center gap-4 text-xs text-indigo-200/80 pt-1">
                <span class="flex items-center gap-1.5">
                  <lucide-icon [img]="CalendarIcon" class="w-3.5 h-3.5 text-indigo-300"></lucide-icon>
                  <span>Due: {{ deliverable.dueDate | date:'mediumDate' }}</span>
                </span>

                @if (deliverable.liveUrl) {
                  <a
                    [href]="deliverable.liveUrl"
                    target="_blank"
                    class="flex items-center gap-1.5 text-emerald-300 hover:underline font-bold"
                  >
                    <lucide-icon [img]="GlobeIcon" class="w-3.5 h-3.5"></lucide-icon>
                    <span>View Published Post</span>
                  </a>
                }
              </div>
            </div>
          </div>

          <!-- Body Content -->
          <div class="p-6 space-y-6 max-h-[70vh] overflow-y-auto">
            <!-- Brief Notes -->
            @if (deliverable.briefNotes) {
              <div class="p-4 rounded-2xl bg-slate-50 border border-slate-200/80 space-y-1.5 text-xs">
                <span class="font-bold text-slate-700 uppercase tracking-wider text-[10px] block">Brief Guidelines</span>
                <p class="text-slate-700 whitespace-pre-line leading-relaxed">{{ deliverable.briefNotes }}</p>
              </div>
            }

            <!-- Submissions & Approval Review History -->
            <div class="space-y-4">
              <div class="flex items-center justify-between">
                <h3 class="text-xs font-bold text-slate-900 uppercase tracking-wider">
                  Submission History & Reviews ({{ submissions().length }})
                </h3>

                <button
                  type="button"
                  (click)="submitDraft.emit(deliverable)"
                  class="inline-flex items-center gap-1.5 px-3 py-1.5 rounded-xl bg-indigo-50 hover:bg-indigo-100 text-indigo-700 text-xs font-semibold transition"
                >
                  <lucide-icon [img]="UploadCloudIcon" class="w-3.5 h-3.5"></lucide-icon>
                  <span>Submit New Draft</span>
                </button>
              </div>

              @if (isLoadingSubmissions()) {
                <div class="p-8 text-center space-y-2">
                  <lucide-icon [img]="Loader2Icon" class="w-6 h-6 text-indigo-600 animate-spin mx-auto"></lucide-icon>
                  <p class="text-xs text-slate-400">Loading version history...</p>
                </div>
              } @else if (submissions().length === 0) {
                <div class="p-8 rounded-2xl bg-slate-50 border border-slate-200/60 text-center space-y-2">
                  <lucide-icon [img]="UploadCloudIcon" class="w-6 h-6 text-slate-400 mx-auto"></lucide-icon>
                  <p class="text-xs text-slate-600 font-bold">No Drafts Submitted Yet</p>
                  <p class="text-[11px] text-slate-400">Click "Submit New Draft" to upload the first draft asset.</p>
                </div>
              } @else {
                <div class="space-y-4">
                  @for (sub of submissions(); track sub.id) {
                    <div class="p-5 rounded-3xl bg-slate-50 border border-slate-200/80 space-y-4">
                      <!-- Version Header -->
                      <div class="flex items-center justify-between">
                        <div class="flex items-center gap-2">
                          <span class="px-2.5 py-1 rounded-xl bg-slate-900 text-white font-black text-xs shadow-xs">
                            Version {{ sub.versionNumber }}
                          </span>
                          <span class="text-[11px] text-slate-400">
                            Submitted {{ sub.submittedAt | date:'medium' }}
                          </span>
                        </div>

                        <!-- Review Trigger Button -->
                        <button
                          type="button"
                          (click)="reviewSubmission.emit({ submission: sub, deliverable: deliverable })"
                          class="px-3 py-1.5 rounded-xl bg-amber-500 hover:bg-amber-600 text-white text-xs font-semibold shadow-2xs transition flex items-center gap-1.5"
                        >
                          <lucide-icon [img]="MessageSquareIcon" class="w-3.5 h-3.5"></lucide-icon>
                          <span>Review Draft</span>
                        </button>
                      </div>

                      <!-- Media Details & Caption -->
                      <div class="p-3.5 bg-white rounded-2xl border border-slate-200 space-y-2 text-xs">
                        <div class="flex items-center justify-between">
                          <div class="flex items-center gap-2 truncate max-w-[350px]">
                            <lucide-icon [img]="FileVideoIcon" class="w-4 h-4 text-indigo-600 shrink-0"></lucide-icon>
                            <span class="font-bold text-slate-900 truncate">{{ sub.mediaFileName }}</span>
                            <span class="text-[10px] text-slate-400">({{ formatFileSize(sub.mediaFileSize) }})</span>
                          </div>

                          @if (sub.downloadUrl) {
                            <a
                              [href]="sub.downloadUrl"
                              target="_blank"
                              class="text-indigo-600 hover:underline font-bold text-xs flex items-center gap-1"
                            >
                              <span>Download Media</span>
                              <lucide-icon [img]="ExternalLinkIcon" class="w-3 h-3"></lucide-icon>
                            </a>
                          }
                        </div>

                        @if (sub.caption) {
                          <div class="pt-2 border-t border-slate-100 text-slate-600 text-[11px] leading-relaxed">
                            <span class="font-bold text-slate-800">Caption: </span>{{ sub.caption }}
                          </div>
                        }
                      </div>

                      <!-- Reviews Given on this Version -->
                      @if (reviewsMap().get(sub.id)?.length) {
                        <div class="space-y-2 pt-1">
                          <span class="text-[10px] font-bold text-slate-500 uppercase tracking-wider block">
                            Review Decisions & Feedback
                          </span>

                          <div class="space-y-2">
                            @for (rev of reviewsMap().get(sub.id); track rev.id) {
                              <div class="p-3 rounded-xl border text-xs space-y-1.5" [ngClass]="getReviewCardClass(rev.decision)">
                                <div class="flex items-center justify-between">
                                  <span class="font-bold uppercase text-[10px] tracking-wider">{{ rev.decision }}</span>
                                  <span class="text-[10px] opacity-75">{{ rev.reviewedAt | date:'short' }}</span>
                                </div>
                                @if (rev.feedbackNotes) {
                                  <p class="text-[11px] leading-relaxed whitespace-pre-line">{{ rev.feedbackNotes }}</p>
                                }
                              </div>
                            }
                          </div>
                        </div>
                      }
                    </div>
                  }
                </div>
              }
            </div>
          </div>

          <!-- Footer Actions -->
          <div class="p-4 bg-slate-50 border-t border-slate-100 flex items-center justify-between gap-3">
            <button
              type="button"
              (click)="close.emit()"
              class="px-4 py-2 rounded-xl border border-slate-200 text-slate-700 hover:bg-slate-100 text-xs font-semibold transition"
            >
              Close
            </button>

            <div class="flex items-center gap-2">
              @if (deliverable.status === 'Approved') {
                <button
                  type="button"
                  (click)="submitProof.emit(deliverable)"
                  class="px-4 py-2 rounded-xl bg-emerald-600 hover:bg-emerald-500 text-white text-xs font-semibold shadow-xs transition flex items-center gap-1.5"
                >
                  <lucide-icon [img]="GlobeIcon" class="w-3.5 h-3.5"></lucide-icon>
                  <span>Submit Live Proof</span>
                </button>
              }

              <button
                type="button"
                (click)="editDeliverable.emit(deliverable)"
                class="px-4 py-2 rounded-xl border border-slate-200 text-slate-700 hover:bg-slate-100 text-xs font-semibold transition"
              >
                Edit Scope
              </button>
            </div>
          </div>
        </div>
      </div>
    }
  `
})
export class DeliverableDetailModalComponent implements OnInit {
  @Input() isOpen = false;
  @Input() deliverable: Deliverable | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() editDeliverable = new EventEmitter<Deliverable>();
  @Output() submitDraft = new EventEmitter<Deliverable>();
  @Output() reviewSubmission = new EventEmitter<{ submission: ContentSubmission; deliverable: Deliverable }>();
  @Output() submitProof = new EventEmitter<Deliverable>();

  private deliverableService = inject(DeliverableService);
  private approvalService = inject(ApprovalService);

  readonly submissions = signal<ContentSubmission[]>([]);
  readonly reviewsMap = signal<Map<string, ApprovalReview[]>>(new Map());
  readonly isLoadingSubmissions = signal<boolean>(false);

  readonly XIcon = X;
  readonly FileCheck2Icon = FileCheck2;
  readonly CalendarIcon = Calendar;
  readonly LayersIcon = Layers;
  readonly UploadCloudIcon = UploadCloud;
  readonly MessageSquareIcon = MessageSquare;
  readonly ExternalLinkIcon = ExternalLink;
  readonly CheckCircle2Icon = CheckCircle2;
  readonly RefreshCwIcon = RefreshCw;
  readonly XCircleIcon = XCircle;
  readonly FileVideoIcon = FileVideo;
  readonly FileImageIcon = FileImage;
  readonly FileIcon = File;
  readonly ClockIcon = Clock;
  readonly Loader2Icon = Loader2;
  readonly GlobeIcon = Globe;
  readonly PlusIcon = Plus;

  constructor() {
    effect(() => {
      if (this.isOpen && this.deliverable) {
        this.loadSubmissions(this.deliverable.id);
      }
    });
  }

  ngOnInit(): void {}

  private loadSubmissions(deliverableId: string): void {
    this.isLoadingSubmissions.set(true);
    this.deliverableService.getContentSubmissions(deliverableId).subscribe({
      next: res => {
        this.isLoadingSubmissions.set(false);
        if (res.success && res.data) {
          this.submissions.set(res.data);
          this.loadReviewsForSubmissions(res.data);
        }
      },
      error: () => {
        this.isLoadingSubmissions.set(false);
      }
    });
  }

  private loadReviewsForSubmissions(subs: ContentSubmission[]): void {
    const map = new Map<string, ApprovalReview[]>();
    for (const sub of subs) {
      this.approvalService.getReviewsBySubmissionId(sub.id).subscribe({
        next: res => {
          if (res.success && res.data) {
            map.set(sub.id, res.data);
            this.reviewsMap.set(new Map(map));
          }
        }
      });
    }
  }

  formatFileSize(bytes: number): string {
    if (bytes >= 1_000_000) {
      return (bytes / 1_000_000).toFixed(1) + ' MB';
    }
    if (bytes >= 1_000) {
      return (bytes / 1_000).toFixed(1) + ' KB';
    }
    return bytes + ' B';
  }

  getStatusBadgeClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'approved':
        return 'bg-emerald-500/20 text-emerald-300 border border-emerald-400/30';
      case 'submitted':
        return 'bg-amber-500/20 text-amber-300 border border-amber-400/30';
      case 'revisionrequested':
        return 'bg-rose-500/20 text-rose-300 border border-rose-400/30';
      case 'completed':
      case 'published':
        return 'bg-indigo-500/20 text-indigo-300 border border-indigo-400/30';
      default:
        return 'bg-slate-700 text-slate-300';
    }
  }

  getReviewCardClass(decision: string): string {
    switch (decision?.toLowerCase()) {
      case 'approved':
        return 'bg-emerald-50 border-emerald-200 text-emerald-900';
      case 'revisionrequested':
        return 'bg-amber-50 border-amber-200 text-amber-900';
      case 'rejected':
        return 'bg-rose-50 border-rose-200 text-rose-900';
      default:
        return 'bg-slate-50 border-slate-200 text-slate-700';
    }
  }
}
