import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { CampaignService } from '../../core/services/campaign.service';
import { DeliverableService } from '../../core/services/deliverable.service';
import { CampaignSummary } from '../../core/models/campaign.model';
import { Deliverable, ContentSubmission } from '../../core/models/deliverable.model';
import { toast } from 'ngx-sonner';
import {
  LucideAngularModule,
  Sparkles,
  Megaphone,
  Video,
  UploadCloud,
  CheckCircle2,
  Clock,
  AlertCircle,
  Link,
  Calendar,
  X,
  FileCheck,
  RotateCcw,
  ExternalLink,
  ChevronRight,
  Send
} from 'lucide-angular';

@Component({
  selector: 'app-creator-portal',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LucideAngularModule
  ],
  template: `
    <div class="space-y-8">
      <!-- Creator Welcome Header -->
      <div class="p-6 sm:p-8 rounded-3xl bg-gradient-to-r from-violet-950 via-purple-900 to-slate-900 border border-purple-800/40 text-white relative overflow-hidden shadow-xl shadow-purple-950/30">
        <div class="absolute -right-10 -bottom-10 w-64 h-64 bg-violet-500/10 rounded-full blur-3xl pointer-events-none"></div>

        <div class="relative z-10 flex flex-col md:flex-row md:items-center justify-between gap-6">
          <div class="space-y-2">
            <div class="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-violet-500/20 border border-violet-400/30 text-violet-200 text-xs font-semibold">
              <lucide-icon [img]="SparklesIcon" class="w-3.5 h-3.5 text-violet-300"></lucide-icon>
              <span>Creator Workspace Portal</span>
            </div>
            <h1 class="text-2xl sm:text-3xl font-extrabold tracking-tight text-white">
              Hello, {{ authService.userFullName() }}!
            </h1>
            <p class="text-violet-200/80 text-sm max-w-2xl leading-relaxed">
              Here are your assigned campaigns and deliverables. Upload your draft media for agency review and submit live publication proof when approved.
            </p>
          </div>

          <div class="flex items-center gap-3 shrink-0">
            <div class="px-4 py-2 rounded-2xl bg-white/10 border border-white/10 text-center">
              <span class="text-xs text-violet-200 uppercase tracking-wider block">Assigned Tasks</span>
              <span class="text-xl font-bold text-white">{{ allDeliverables().length }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Quick KPI Stat Cards -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 sm:gap-6">
        <div class="p-5 bg-white rounded-2xl border border-slate-200/80 shadow-xs space-y-2">
          <div class="flex items-center justify-between">
            <span class="text-xs font-medium text-slate-500 uppercase tracking-wider">Active Campaigns</span>
            <div class="w-9 h-9 rounded-xl bg-violet-50 text-violet-600 flex items-center justify-center">
              <lucide-icon [img]="MegaphoneIcon" class="w-4 h-4"></lucide-icon>
            </div>
          </div>
          <span class="text-2xl font-bold text-slate-900">{{ campaigns().length }}</span>
          <p class="text-[11px] text-slate-400">Enrolled roster campaigns</p>
        </div>

        <div class="p-5 bg-white rounded-2xl border border-slate-200/80 shadow-xs space-y-2">
          <div class="flex items-center justify-between">
            <span class="text-xs font-medium text-slate-500 uppercase tracking-wider">Drafts Needed</span>
            <div class="w-9 h-9 rounded-xl bg-blue-50 text-blue-600 flex items-center justify-center">
              <lucide-icon [img]="UploadCloudIcon" class="w-4 h-4"></lucide-icon>
            </div>
          </div>
          <span class="text-2xl font-bold text-slate-900">{{ pendingSubmissionCount() }}</span>
          <p class="text-[11px] text-slate-400">Awaiting your upload</p>
        </div>

        <div class="p-5 bg-white rounded-2xl border border-slate-200/80 shadow-xs space-y-2">
          <div class="flex items-center justify-between">
            <span class="text-xs font-medium text-slate-500 uppercase tracking-wider">Revision Requests</span>
            <div class="w-9 h-9 rounded-xl bg-rose-50 text-rose-600 flex items-center justify-center">
              <lucide-icon [img]="RotateCcwIcon" class="w-4 h-4"></lucide-icon>
            </div>
          </div>
          <span class="text-2xl font-bold text-rose-600">{{ revisionCount() }}</span>
          <p class="text-[11px] text-slate-400">Feedback notes to address</p>
        </div>

        <div class="p-5 bg-white rounded-2xl border border-slate-200/80 shadow-xs space-y-2">
          <div class="flex items-center justify-between">
            <span class="text-xs font-medium text-slate-500 uppercase tracking-wider">Approved & Done</span>
            <div class="w-9 h-9 rounded-xl bg-emerald-50 text-emerald-600 flex items-center justify-center">
              <lucide-icon [img]="CheckCircle2Icon" class="w-4 h-4"></lucide-icon>
            </div>
          </div>
          <span class="text-2xl font-bold text-emerald-600">{{ approvedCount() }}</span>
          <p class="text-[11px] text-slate-400">Ready to post / Published</p>
        </div>
      </div>

      <!-- Deliverables Action List -->
      <div class="p-6 bg-white rounded-2xl border border-slate-200/80 shadow-xs space-y-5">
        <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
          <div>
            <h2 class="text-base font-bold text-slate-900">My Deliverable Tasks</h2>
            <p class="text-xs text-slate-500 mt-0.5">Track your content deliverables from brief to approved live post</p>
          </div>

          <!-- Campaign Select Filter -->
          <div class="flex items-center gap-3">
            <select
              [(ngModel)]="selectedCampaignId"
              (ngModelChange)="onCampaignFilterChange()"
              class="px-3 py-2 text-xs rounded-xl border border-slate-200 bg-slate-50 focus:bg-white focus:outline-hidden font-medium"
            >
              <option value="">All My Campaigns</option>
              @for (c of campaigns(); track c.id) {
                <option [value]="c.id">{{ c.title }}</option>
              }
            </select>
          </div>
        </div>

        @if (loading()) {
          <div class="space-y-3 py-4">
            @for (i of [1, 2, 3]; track i) {
              <div class="h-20 bg-slate-100 animate-pulse rounded-2xl"></div>
            }
          </div>
        } @else if (filteredDeliverables().length === 0) {
          <div class="text-center py-12 px-4 rounded-xl bg-slate-50 border border-dashed border-slate-200 space-y-2">
            <lucide-icon [img]="CheckCircle2Icon" class="w-8 h-8 text-emerald-500 mx-auto"></lucide-icon>
            <p class="text-xs font-semibold text-slate-700">No pending deliverables found</p>
            <p class="text-[11px] text-slate-400">You are all caught up on your campaign tasks.</p>
          </div>
        } @else {
          <div class="space-y-4">
            @for (d of filteredDeliverables(); track d.id) {
              <div class="p-5 rounded-2xl border transition bg-slate-50/50 hover:bg-white hover:border-violet-200 hover:shadow-xs space-y-4">
                <div class="flex flex-col md:flex-row md:items-center justify-between gap-3">
                  <div class="space-y-1">
                    <div class="flex items-center gap-2">
                      <span class="px-2 py-0.5 rounded-md bg-violet-100 text-violet-800 text-[10px] font-bold uppercase tracking-wider">
                        {{ d.platform }} • {{ d.contentType }}
                      </span>
                      <span class="text-xs text-slate-400">• Version {{ d.latestVersion }}</span>
                    </div>
                    <h3 class="text-sm font-bold text-slate-900">{{ d.title }}</h3>
                    <p class="text-xs text-slate-500 max-w-xl">
                      {{ d.briefNotes || 'Follow standard platform guidelines and creator brief notes.' }}
                    </p>
                  </div>

                  <div class="flex items-center gap-3 shrink-0">
                    <div class="text-right">
                      <span class="text-[11px] text-slate-400 block">Due Date</span>
                      <span class="text-xs font-bold text-slate-700">{{ d.dueDate }}</span>
                    </div>

                    <span
                      class="px-3 py-1 rounded-full text-xs font-semibold"
                      [ngClass]="{
                        'bg-blue-100 text-blue-700': d.status === 'Pending',
                        'bg-amber-100 text-amber-700': d.status === 'Submitted' || d.status === 'DraftSubmitted',
                        'bg-rose-100 text-rose-700': d.status === 'Revision',
                        'bg-emerald-100 text-emerald-700': d.status === 'Approved',
                        'bg-purple-100 text-purple-700': d.status === 'Published'
                      }"
                    >
                      {{ d.status }}
                    </span>
                  </div>
                </div>

                <!-- Action Button Bar -->
                <div class="pt-3 border-t border-slate-200/60 flex flex-wrap items-center justify-between gap-3">
                  <div class="text-[11px] text-slate-500 flex items-center gap-2">
                    @if (d.liveUrl) {
                      <a [href]="d.liveUrl" target="_blank" class="text-indigo-600 hover:underline inline-flex items-center gap-1 font-semibold">
                        <span>Live Post URL</span>
                        <lucide-icon [img]="ExternalLinkIcon" class="w-3 h-3"></lucide-icon>
                      </a>
                    } @else {
                      <span>Awaiting publication link</span>
                    }
                  </div>

                  <div class="flex items-center gap-2">
                    <!-- Submit Draft Button -->
                    @if (d.status === 'Pending' || d.status === 'Revision') {
                      <button
                        (click)="openSubmissionModal(d)"
                        class="px-3.5 py-1.5 rounded-xl bg-violet-600 hover:bg-violet-500 text-white text-xs font-semibold shadow-xs transition flex items-center gap-1.5"
                      >
                        <lucide-icon [img]="UploadCloudIcon" class="w-3.5 h-3.5"></lucide-icon>
                        <span>{{ d.status === 'Revision' ? 'Submit Revised Draft' : 'Upload Draft' }}</span>
                      </button>
                    }

                    <!-- Submit Live Proof Button -->
                    @if (d.status === 'Approved') {
                      <button
                        (click)="openProofModal(d)"
                        class="px-3.5 py-1.5 rounded-xl bg-emerald-600 hover:bg-emerald-500 text-white text-xs font-semibold shadow-xs transition flex items-center gap-1.5"
                      >
                        <lucide-icon [img]="FileCheckIcon" class="w-3.5 h-3.5"></lucide-icon>
                        <span>Submit Live Post Proof</span>
                      </button>
                    }

                    <!-- View Submissions History -->
                    <button
                      (click)="viewSubmissionsHistory(d)"
                      class="px-3 py-1.5 rounded-xl bg-slate-200/70 hover:bg-slate-300 text-slate-700 text-xs font-semibold transition"
                    >
                      History ({{ d.latestVersion }})
                    </button>
                  </div>
                </div>
              </div>
            }
          </div>
        }
      </div>

      <!-- Submission Modal -->
      @if (selectedDeliverableForSubmit()) {
        <div class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/60 backdrop-blur-xs">
          <div class="bg-white rounded-3xl max-w-lg w-full p-6 sm:p-8 space-y-6 shadow-2xl border border-slate-100">
            <div class="flex items-center justify-between">
              <div>
                <h3 class="text-lg font-bold text-slate-900">Upload Content Draft</h3>
                <p class="text-xs text-slate-500 mt-0.5">{{ selectedDeliverableForSubmit()?.title }}</p>
              </div>
              <button (click)="closeModals()" class="p-2 rounded-xl text-slate-400 hover:text-slate-600 hover:bg-slate-100 transition">
                <lucide-icon [img]="XIcon" class="w-5 h-5"></lucide-icon>
              </button>
            </div>

            <div class="space-y-4">
              <!-- Mock File Upload or Key -->
              <div class="space-y-1.5">
                <label class="text-xs font-semibold text-slate-700">Media File Name / Attachment</label>
                <input
                  type="text"
                  [(ngModel)]="submissionForm.mediaFileName"
                  placeholder="e.g. draft_video_v1.mp4 or photo_proof.jpg"
                  class="w-full px-3.5 py-2.5 text-xs rounded-xl border border-slate-200 bg-slate-50 focus:bg-white focus:outline-hidden focus:border-violet-500 font-medium"
                />
              </div>

              <!-- Media Storage Key -->
              <div class="space-y-1.5">
                <label class="text-xs font-semibold text-slate-700">Storage Media Key (MinIO Object)</label>
                <input
                  type="text"
                  [(ngModel)]="submissionForm.mediaObjectKey"
                  placeholder="e.g. submissions/sample_video_v1.mp4"
                  class="w-full px-3.5 py-2.5 text-xs font-mono rounded-xl border border-slate-200 bg-slate-50 focus:bg-white focus:outline-hidden focus:border-violet-500 font-medium"
                />
              </div>

              <!-- Caption -->
              <div class="space-y-1.5">
                <label class="text-xs font-semibold text-slate-700">Draft Post Caption & Hashtags</label>
                <textarea
                  [(ngModel)]="submissionForm.caption"
                  rows="4"
                  placeholder="Paste your prepared draft caption, brand tags, and promotional hashtags..."
                  class="w-full px-3.5 py-2.5 text-xs rounded-xl border border-slate-200 bg-slate-50 focus:bg-white focus:outline-hidden focus:border-violet-500"
                ></textarea>
              </div>
            </div>

            <div class="flex items-center justify-end gap-3 pt-3 border-t border-slate-100">
              <button
                (click)="closeModals()"
                class="px-4 py-2 rounded-xl text-xs font-semibold text-slate-600 hover:bg-slate-100 transition"
              >
                Cancel
              </button>
              <button
                (click)="submitDraft()"
                [disabled]="isSubmitting() || !submissionForm.mediaFileName"
                class="px-5 py-2.5 rounded-xl bg-violet-600 hover:bg-violet-500 disabled:opacity-50 text-white text-xs font-semibold shadow-md shadow-violet-600/30 transition flex items-center gap-2"
              >
                <lucide-icon [img]="SendIcon" class="w-3.5 h-3.5"></lucide-icon>
                <span>{{ isSubmitting() ? 'Submitting...' : 'Submit for Agency Review' }}</span>
              </button>
            </div>
          </div>
        </div>
      }

      <!-- Live Proof Modal -->
      @if (selectedDeliverableForProof()) {
        <div class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/60 backdrop-blur-xs">
          <div class="bg-white rounded-3xl max-w-lg w-full p-6 sm:p-8 space-y-6 shadow-2xl border border-slate-100">
            <div class="flex items-center justify-between">
              <div>
                <h3 class="text-lg font-bold text-slate-900">Submit Live Post Proof</h3>
                <p class="text-xs text-slate-500 mt-0.5">{{ selectedDeliverableForProof()?.title }}</p>
              </div>
              <button (click)="closeModals()" class="p-2 rounded-xl text-slate-400 hover:text-slate-600 hover:bg-slate-100 transition">
                <lucide-icon [img]="XIcon" class="w-5 h-5"></lucide-icon>
              </button>
            </div>

            <div class="space-y-4">
              <div class="space-y-1.5">
                <label class="text-xs font-semibold text-slate-700">Live Post URL</label>
                <input
                  type="url"
                  [(ngModel)]="proofForm.liveUrl"
                  placeholder="https://www.tiktok.com/@creator/video/123456789"
                  class="w-full px-3.5 py-2.5 text-xs rounded-xl border border-slate-200 bg-slate-50 focus:bg-white focus:outline-hidden focus:border-emerald-500 font-medium"
                />
              </div>

              <div class="space-y-1.5">
                <label class="text-xs font-semibold text-slate-700">Proof Media Key / Screenshot</label>
                <input
                  type="text"
                  [(ngModel)]="proofForm.proofMediaKey"
                  placeholder="e.g. proofs/screenshot_published.png"
                  class="w-full px-3.5 py-2.5 text-xs font-mono rounded-xl border border-slate-200 bg-slate-50 focus:bg-white focus:outline-hidden focus:border-emerald-500 font-medium"
                />
              </div>

              <div class="space-y-1.5">
                <label class="text-xs font-semibold text-slate-700">Posting Date</label>
                <input
                  type="date"
                  [(ngModel)]="proofForm.postingDate"
                  class="w-full px-3.5 py-2.5 text-xs rounded-xl border border-slate-200 bg-slate-50 focus:bg-white focus:outline-hidden focus:border-emerald-500"
                />
              </div>
            </div>

            <div class="flex items-center justify-end gap-3 pt-3 border-t border-slate-100">
              <button
                (click)="closeModals()"
                class="px-4 py-2 rounded-xl text-xs font-semibold text-slate-600 hover:bg-slate-100 transition"
              >
                Cancel
              </button>
              <button
                (click)="submitProof()"
                [disabled]="isSubmitting() || !proofForm.liveUrl"
                class="px-5 py-2.5 rounded-xl bg-emerald-600 hover:bg-emerald-500 disabled:opacity-50 text-white text-xs font-semibold shadow-md shadow-emerald-600/30 transition flex items-center gap-2"
              >
                <lucide-icon [img]="FileCheckIcon" class="w-3.5 h-3.5"></lucide-icon>
                <span>{{ isSubmitting() ? 'Saving...' : 'Submit Published Proof' }}</span>
              </button>
            </div>
          </div>
        </div>
      }

      <!-- Submissions History Modal -->
      @if (selectedDeliverableForHistory()) {
        <div class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/60 backdrop-blur-xs">
          <div class="bg-white rounded-3xl max-w-xl w-full p-6 sm:p-8 space-y-6 shadow-2xl border border-slate-100 max-h-[85vh] flex flex-col">
            <div class="flex items-center justify-between shrink-0">
              <div>
                <h3 class="text-lg font-bold text-slate-900">Submission History</h3>
                <p class="text-xs text-slate-500 mt-0.5">{{ selectedDeliverableForHistory()?.title }}</p>
              </div>
              <button (click)="closeModals()" class="p-2 rounded-xl text-slate-400 hover:text-slate-600 hover:bg-slate-100 transition">
                <lucide-icon [img]="XIcon" class="w-5 h-5"></lucide-icon>
              </button>
            </div>

            <div class="overflow-y-auto space-y-4 pr-1">
              @if (submissionsHistory().length === 0) {
                <div class="text-center py-10 px-4 rounded-xl bg-slate-50 border border-dashed border-slate-200">
                  <p class="text-xs text-slate-500">No submissions uploaded yet.</p>
                </div>
              } @else {
                @for (sub of submissionsHistory(); track sub.id) {
                  <div class="p-4 rounded-2xl bg-slate-50 border border-slate-200/70 space-y-2">
                    <div class="flex items-center justify-between">
                      <span class="px-2.5 py-0.5 rounded-full bg-violet-100 text-violet-800 text-[11px] font-bold">
                        Version {{ sub.versionNumber }}
                      </span>
                      <span class="text-[11px] text-slate-400">{{ sub.submittedAt | date:'medium' }}</span>
                    </div>
                    <p class="text-xs font-semibold text-slate-800">{{ sub.mediaFileName }}</p>
                    @if (sub.caption) {
                      <p class="text-xs text-slate-600 bg-white p-2.5 rounded-xl border border-slate-200/60 italic">
                        "{{ sub.caption }}"
                      </p>
                    }
                  </div>
                }
              }
            </div>

            <div class="pt-3 border-t border-slate-100 text-right shrink-0">
              <button
                (click)="closeModals()"
                class="px-4 py-2 rounded-xl bg-slate-100 hover:bg-slate-200 text-xs font-semibold text-slate-700 transition"
              >
                Close
              </button>
            </div>
          </div>
        </div>
      }
    </div>
  `
})
export class CreatorPortalComponent implements OnInit {
  readonly authService = inject(AuthService);
  private readonly campaignService = inject(CampaignService);
  private readonly deliverableService = inject(DeliverableService);

  readonly campaigns = signal<CampaignSummary[]>([]);
  readonly allDeliverables = signal<Deliverable[]>([]);
  readonly loading = signal<boolean>(true);

  selectedCampaignId = '';

  readonly selectedDeliverableForSubmit = signal<Deliverable | null>(null);
  readonly selectedDeliverableForProof = signal<Deliverable | null>(null);
  readonly selectedDeliverableForHistory = signal<Deliverable | null>(null);
  readonly submissionsHistory = signal<ContentSubmission[]>([]);
  readonly isSubmitting = signal<boolean>(false);

  submissionForm = {
    mediaFileName: '',
    mediaObjectKey: '',
    mediaFileSize: 1048576,
    caption: ''
  };

  proofForm = {
    liveUrl: '',
    proofMediaKey: '',
    postingDate: new Date().toISOString().split('T')[0]
  };

  readonly SparklesIcon = Sparkles;
  readonly MegaphoneIcon = Megaphone;
  readonly VideoIcon = Video;
  readonly UploadCloudIcon = UploadCloud;
  readonly CheckCircle2Icon = CheckCircle2;
  readonly ClockIcon = Clock;
  readonly AlertCircleIcon = AlertCircle;
  readonly LinkIcon = Link;
  readonly CalendarIcon = Calendar;
  readonly XIcon = X;
  readonly FileCheckIcon = FileCheck;
  readonly RotateCcwIcon = RotateCcw;
  readonly ExternalLinkIcon = ExternalLink;
  readonly ChevronRightIcon = ChevronRight;
  readonly SendIcon = Send;

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading.set(true);
    this.campaignService.getCampaigns(undefined, undefined, undefined, 1, 100).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.campaigns.set(res.data.items);
          this.loadAllCampaignDeliverables(res.data.items);
        } else {
          this.loading.set(false);
        }
      },
      error: () => {
        this.loading.set(false);
      }
    });
  }

  loadAllCampaignDeliverables(campaigns: CampaignSummary[]): void {
    const deliverablesList: Deliverable[] = [];
    if (campaigns.length === 0) {
      this.allDeliverables.set([]);
      this.loading.set(false);
      return;
    }

    let loadedCount = 0;
    campaigns.forEach((c) => {
      this.deliverableService.getDeliverablesByCampaign(c.id).subscribe({
        next: (res) => {
          if (res.success && res.data) {
            deliverablesList.push(...res.data);
          }
          loadedCount++;
          if (loadedCount === campaigns.length) {
            this.allDeliverables.set(deliverablesList);
            this.loading.set(false);
          }
        },
        error: () => {
          loadedCount++;
          if (loadedCount === campaigns.length) {
            this.allDeliverables.set(deliverablesList);
            this.loading.set(false);
          }
        }
      });
    });
  }

  filteredDeliverables(): Deliverable[] {
    if (!this.selectedCampaignId) {
      return this.allDeliverables();
    }
    return this.allDeliverables().filter(d => d.campaignId === this.selectedCampaignId);
  }

  pendingSubmissionCount(): number {
    return this.allDeliverables().filter(d => d.status === 'Pending').length;
  }

  revisionCount(): number {
    return this.allDeliverables().filter(d => d.status === 'Revision').length;
  }

  approvedCount(): number {
    return this.allDeliverables().filter(d => d.status === 'Approved' || d.status === 'Published').length;
  }

  onCampaignFilterChange(): void {}

  openSubmissionModal(d: Deliverable): void {
    this.selectedDeliverableForSubmit.set(d);
    this.submissionForm = {
      mediaFileName: `draft_video_v${(d.latestVersion || 0) + 1}.mp4`,
      mediaObjectKey: `submissions/${d.id}_v${(d.latestVersion || 0) + 1}.mp4`,
      mediaFileSize: 5242880,
      caption: ''
    };
  }

  openProofModal(d: Deliverable): void {
    this.selectedDeliverableForProof.set(d);
    this.proofForm = {
      liveUrl: d.liveUrl || '',
      proofMediaKey: d.proofMediaKey || `proofs/${d.id}_screenshot.png`,
      postingDate: d.postingDate || new Date().toISOString().split('T')[0]
    };
  }

  viewSubmissionsHistory(d: Deliverable): void {
    this.selectedDeliverableForHistory.set(d);
    this.deliverableService.getContentSubmissions(d.id).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.submissionsHistory.set(res.data);
        }
      }
    });
  }

  closeModals(): void {
    this.selectedDeliverableForSubmit.set(null);
    this.selectedDeliverableForProof.set(null);
    this.selectedDeliverableForHistory.set(null);
  }

  submitDraft(): void {
    const d = this.selectedDeliverableForSubmit();
    if (!d) return;

    this.isSubmitting.set(true);
    this.deliverableService.createContentSubmission(d.id, {
      mediaObjectKey: this.submissionForm.mediaObjectKey,
      mediaFileName: this.submissionForm.mediaFileName,
      mediaFileSize: this.submissionForm.mediaFileSize,
      caption: this.submissionForm.caption
    }).subscribe({
      next: (res) => {
        this.isSubmitting.set(false);
        if (res.success) {
          toast.success('Draft submitted successfully for review!');
          this.closeModals();
          this.loadData();
        }
      },
      error: () => {
        this.isSubmitting.set(false);
        toast.error('Failed to submit draft');
      }
    });
  }

  submitProof(): void {
    const d = this.selectedDeliverableForProof();
    if (!d) return;

    this.isSubmitting.set(true);
    this.deliverableService.submitPublishProof(d.id, {
      liveUrl: this.proofForm.liveUrl,
      proofMediaKey: this.proofForm.proofMediaKey,
      postingDate: this.proofForm.postingDate
    }).subscribe({
      next: (res) => {
        this.isSubmitting.set(false);
        if (res.success) {
          toast.success('Live proof submitted successfully!');
          this.closeModals();
          this.loadData();
        }
      },
      error: () => {
        this.isSubmitting.set(false);
        toast.error('Failed to submit publication proof');
      }
    });
  }
}
