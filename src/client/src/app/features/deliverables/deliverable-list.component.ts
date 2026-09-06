import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  LucideAngularModule,
  FileCheck2,
  Calendar,
  Layers,
  Search,
  Plus,
  Edit,
  Trash2,
  Eye,
  UploadCloud,
  MessageSquare,
  Globe,
  ExternalLink,
  CheckCircle2,
  Clock,
  AlertTriangle,
  Loader2,
  RefreshCw,
  Users,
  Megaphone,
  Share2
} from 'lucide-angular';
import { toast } from 'ngx-sonner';
import { DeliverableService } from '../../core/services/deliverable.service';
import { CampaignService } from '../../core/services/campaign.service';
import { CreatorService } from '../../core/services/creator.service';
import { Deliverable, ContentSubmission } from '../../core/models/deliverable.model';
import { CampaignSummary, CampaignCreator } from '../../core/models/campaign.model';
import { CreatorSummary } from '../../core/models/creator.model';
import { DeliverableModalComponent } from './deliverable-modal/deliverable-modal.component';
import { SubmissionModalComponent } from './submission-modal/submission-modal.component';
import { ReviewModalComponent } from './review-modal/review-modal.component';
import { PublishProofModalComponent } from './publish-proof-modal/publish-proof-modal.component';
import { DeliverableDetailModalComponent } from './deliverable-detail-modal/deliverable-detail-modal.component';

@Component({
  selector: 'app-deliverable-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LucideAngularModule,
    DeliverableModalComponent,
    SubmissionModalComponent,
    ReviewModalComponent,
    PublishProofModalComponent,
    DeliverableDetailModalComponent
  ],
  template: `
    <div class="space-y-6">
      <!-- Header Banner -->
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <div class="flex items-center gap-2">
            <h1 class="text-2xl font-black tracking-tight text-slate-900">Deliverables & Review</h1>
            <span class="px-2.5 py-0.5 rounded-full bg-amber-50 text-amber-700 text-xs font-bold">
              {{ filteredDeliverables().length }} Items
            </span>
          </div>
          <p class="text-xs text-slate-500 mt-1">
            Track content submission deadlines, media assets in MinIO, and the review/revision loop.
          </p>
        </div>

        <div class="flex items-center gap-3">
          <button
            type="button"
            (click)="loadDeliverables()"
            [disabled]="!selectedCampaignId"
            class="p-2.5 rounded-xl border border-slate-200 text-slate-600 hover:bg-slate-50 disabled:opacity-40 transition"
            title="Refresh Deliverables"
          >
            <lucide-icon [img]="RefreshCwIcon" class="w-4 h-4" [class.animate-spin]="isLoading()"></lucide-icon>
          </button>

          <button
            type="button"
            (click)="openCreateModal()"
            [disabled]="!selectedCampaignId"
            class="px-4 py-2.5 rounded-xl bg-amber-600 hover:bg-amber-500 disabled:bg-slate-200 disabled:text-slate-400 text-white text-xs font-semibold shadow-md shadow-amber-600/20 transition flex items-center gap-2"
          >
            <lucide-icon [img]="PlusIcon" class="w-4 h-4"></lucide-icon>
            <span>Add Deliverable</span>
          </button>
        </div>
      </div>

      <!-- Campaign Selector & Filters -->
      <div class="p-4 bg-white rounded-2xl border border-slate-200/80 shadow-xs space-y-4">
        <!-- Campaign Selection Row -->
        <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
          <div class="flex items-center gap-3 flex-1 max-w-xl">
            <div class="flex items-center gap-2 shrink-0">
              <div class="w-8 h-8 rounded-xl bg-amber-50 text-amber-600 flex items-center justify-center font-bold">
                <lucide-icon [img]="FileCheck2Icon" class="w-4 h-4"></lucide-icon>
              </div>
              <span class="text-xs font-bold text-slate-700">Campaign:</span>
            </div>

            <select
              [(ngModel)]="selectedCampaignId"
              (ngModelChange)="onCampaignChange()"
              class="w-full px-3.5 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs font-semibold text-slate-800 focus:outline-hidden focus:ring-2 focus:ring-amber-500/20 focus:border-amber-600 transition"
            >
              <option value="" disabled selected>Select active campaign...</option>
              @for (c of campaigns(); track c.id) {
                <option [value]="c.id">{{ c.title }} ({{ c.status }})</option>
              }
            </select>
          </div>

          <div class="text-xs text-slate-500">
            @if (selectedCampaignId) {
              Showing deliverables for selected brief
            }
          </div>
        </div>

        <!-- Status Filter Tabs -->
        <div class="flex items-center gap-1.5 overflow-x-auto pb-1 border-t border-slate-100 pt-3">
          @for (tab of statusTabs; track tab.value) {
            <button
              type="button"
              (click)="selectedStatus.set(tab.value)"
              class="px-3 py-1.5 rounded-xl text-xs font-bold transition flex items-center gap-1.5 shrink-0"
              [class]="selectedStatus() === tab.value
                ? 'bg-amber-600 text-white shadow-xs'
                : 'text-slate-600 hover:bg-slate-100'"
            >
              <span>{{ tab.label }}</span>
            </button>
          }
        </div>
      </div>

      <!-- Deliverables List Content Area -->
      @if (isLoading()) {
        <div class="p-16 bg-white rounded-2xl border border-slate-200/80 shadow-xs text-center space-y-3">
          <lucide-icon [img]="Loader2Icon" class="w-8 h-8 text-amber-600 animate-spin mx-auto"></lucide-icon>
          <p class="text-xs text-slate-500 font-medium">Loading deliverables...</p>
        </div>
      } @else if (!selectedCampaignId) {
        <div class="p-16 bg-white rounded-2xl border border-slate-200/80 shadow-xs text-center space-y-3 max-w-md mx-auto">
          <div class="w-12 h-12 rounded-2xl bg-amber-50 text-amber-600 flex items-center justify-center mx-auto">
            <lucide-icon [img]="FileCheck2Icon" class="w-6 h-6"></lucide-icon>
          </div>
          <h3 class="text-sm font-bold text-slate-900">Select a Campaign</h3>
          <p class="text-xs text-slate-500">
            Choose an active campaign to view its deliverables, pending submissions, and review approval loop.
          </p>
        </div>
      } @else if (filteredDeliverables().length === 0) {
        <div class="p-16 bg-white rounded-2xl border border-slate-200/80 shadow-xs text-center space-y-3 max-w-md mx-auto">
          <div class="w-12 h-12 rounded-2xl bg-amber-50 text-amber-600 flex items-center justify-center mx-auto">
            <lucide-icon [img]="FileCheck2Icon" class="w-6 h-6"></lucide-icon>
          </div>
          <h3 class="text-sm font-bold text-slate-900">No Deliverables Found</h3>
          <p class="text-xs text-slate-500 leading-relaxed">
            {{ selectedStatus() !== 'ALL' ? 'No deliverables match the selected status filter.' : 'Create your first deliverable item to assign content tasks to creators.' }}
          </p>
          @if (selectedStatus() === 'ALL') {
            <button
              type="button"
              (click)="openCreateModal()"
              class="mt-2 inline-flex items-center gap-1.5 px-4 py-2 rounded-xl bg-amber-600 text-white text-xs font-semibold hover:bg-amber-500 transition"
            >
              <lucide-icon [img]="PlusIcon" class="w-3.5 h-3.5"></lucide-icon>
              <span>Add Deliverable</span>
            </button>
          }
        </div>
      } @else {
        <!-- Deliverables Table -->
        <div class="bg-white rounded-2xl border border-slate-200/80 shadow-xs overflow-hidden">
          <div class="overflow-x-auto">
            <table class="w-full text-left border-collapse">
              <thead>
                <tr class="border-b border-slate-100 bg-slate-50/50 text-[11px] font-bold text-slate-500 uppercase tracking-wider">
                  <th class="py-3.5 px-6">Deliverable Title</th>
                  <th class="py-3.5 px-6">Platform & Format</th>
                  <th class="py-3.5 px-6">Assigned Creator</th>
                  <th class="py-3.5 px-6">Due Date</th>
                  <th class="py-3.5 px-6">Status & Version</th>
                  <th class="py-3.5 px-6 text-right">Actions</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-100 text-xs">
                @for (del of filteredDeliverables(); track del.id) {
                  <tr class="hover:bg-slate-50/75 transition group">
                    <!-- Title -->
                    <td class="py-4 px-6">
                      <div class="space-y-0.5">
                        <button
                          type="button"
                          (click)="openDetailModal(del)"
                          class="font-bold text-slate-900 group-hover:text-amber-600 transition text-left"
                        >
                          {{ del.title }}
                        </button>
                        @if (del.briefNotes) {
                          <p class="text-[11px] text-slate-400 truncate max-w-[200px]">{{ del.briefNotes }}</p>
                        }
                      </div>
                    </td>

                    <!-- Platform & Content Type -->
                    <td class="py-4 px-6">
                      <span class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full bg-slate-100 text-slate-700 font-semibold text-[11px]">
                        <span>{{ del.platform }}</span>
                        <span class="text-slate-400">•</span>
                        <span>{{ del.contentType }}</span>
                      </span>
                    </td>

                    <!-- Creator -->
                    <td class="py-4 px-6 font-medium text-slate-700">
                      {{ getCreatorNameForRoster(del.campaignCreatorId) }}
                    </td>

                    <!-- Due Date -->
                    <td class="py-4 px-6">
                      <span
                        class="px-2.5 py-0.5 rounded-full text-[11px] font-bold inline-flex items-center gap-1"
                        [ngClass]="getDueDateBadgeClass(del.dueDate, del.status)"
                      >
                        <lucide-icon [img]="CalendarIcon" class="w-3 h-3"></lucide-icon>
                        <span>{{ del.dueDate | date:'mediumDate' }}</span>
                      </span>
                    </td>

                    <!-- Status & Version -->
                    <td class="py-4 px-6">
                      <div class="flex items-center gap-1.5">
                        <span
                          class="px-2.5 py-0.5 rounded-full text-[11px] font-bold uppercase tracking-wider"
                          [ngClass]="getStatusBadgeClass(del.status)"
                        >
                          {{ del.status }}
                        </span>
                        @if (del.latestVersion > 0) {
                          <span class="px-2 py-0.5 rounded-md bg-slate-100 text-slate-600 text-[10px] font-bold">
                            v{{ del.latestVersion }}
                          </span>
                        }
                      </div>
                    </td>

                    <!-- Actions -->
                    <td class="py-4 px-6 text-right">
                      <div class="flex items-center justify-end gap-1.5">
                        <!-- If Submitted -> Review action button -->
                        @if (del.status === 'Submitted') {
                          <button
                            type="button"
                            (click)="triggerReviewLatest(del)"
                            class="px-2.5 py-1.5 rounded-xl bg-amber-500 hover:bg-amber-600 text-white font-bold text-[11px] transition flex items-center gap-1 shadow-2xs"
                            title="Review Content"
                          >
                            <lucide-icon [img]="MessageSquareIcon" class="w-3.5 h-3.5"></lucide-icon>
                            <span>Review</span>
                          </button>
                        }

                        <!-- If Approved -> Submit live proof -->
                        @if (del.status === 'Approved') {
                          <button
                            type="button"
                            (click)="openPublishProofModal(del)"
                            class="px-2.5 py-1.5 rounded-xl bg-emerald-600 hover:bg-emerald-500 text-white font-bold text-[11px] transition flex items-center gap-1 shadow-2xs"
                            title="Submit Live Post Proof"
                          >
                            <lucide-icon [img]="GlobeIcon" class="w-3.5 h-3.5"></lucide-icon>
                            <span>Live Proof</span>
                          </button>
                        }

                        <!-- Submit Draft (for Draft or RevisionRequested) -->
                        @if (del.status === 'Draft' || del.status === 'RevisionRequested') {
                          <button
                            type="button"
                            (click)="openSubmissionModal(del)"
                            class="p-2 rounded-lg text-slate-400 hover:text-indigo-600 hover:bg-indigo-50 transition"
                            title="Upload Draft"
                          >
                            <lucide-icon [img]="UploadCloudIcon" class="w-4 h-4"></lucide-icon>
                          </button>
                        }

                        <!-- View Details -->
                        <button
                          type="button"
                          (click)="openDetailModal(del)"
                          class="p-2 rounded-lg text-slate-400 hover:text-amber-600 hover:bg-amber-50 transition"
                          title="View Details & History"
                        >
                          <lucide-icon [img]="EyeIcon" class="w-4 h-4"></lucide-icon>
                        </button>

                        <!-- Edit Scope -->
                        <button
                          type="button"
                          (click)="openEditModal(del)"
                          class="p-2 rounded-lg text-slate-400 hover:text-amber-600 hover:bg-amber-50 transition"
                          title="Edit Deliverable Scope"
                        >
                          <lucide-icon [img]="EditIcon" class="w-4 h-4"></lucide-icon>
                        </button>
                      </div>
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        </div>
      }

      <!-- Modals -->
      <!-- Create & Edit Deliverable Modal -->
      <app-deliverable-modal
        [isOpen]="isModalOpen()"
        [campaignId]="selectedCampaignId"
        [rosterCreators]="rosterCreators()"
        [deliverable]="selectedDeliverableForEdit()"
        [creatorsMap]="creatorsMap()"
        (close)="closeModal()"
        (saved)="onDeliverableSaved()"
      ></app-deliverable-modal>

      <!-- Submit Draft Modal -->
      <app-submission-modal
        [isOpen]="isSubmissionModalOpen()"
        [deliverable]="selectedDeliverableForAction()"
        (close)="closeSubmissionModal()"
        (submitted)="onDraftSubmitted()"
      ></app-submission-modal>

      <!-- Review Modal -->
      <app-review-modal
        [isOpen]="isReviewModalOpen()"
        [submission]="selectedSubmissionForReview()"
        [deliverable]="selectedDeliverableForAction()"
        (close)="closeReviewModal()"
        (reviewed)="onReviewDone()"
      ></app-review-modal>

      <!-- Publish Proof Modal -->
      <app-publish-proof-modal
        [isOpen]="isProofModalOpen()"
        [deliverable]="selectedDeliverableForAction()"
        (close)="closeProofModal()"
        (saved)="onProofSaved()"
      ></app-publish-proof-modal>

      <!-- Deliverable Detail & Version History Modal -->
      <app-deliverable-detail-modal
        [isOpen]="isDetailModalOpen()"
        [deliverable]="selectedDeliverableForDetail()"
        (close)="closeDetailModal()"
        (editDeliverable)="openEditModal($event)"
        (submitDraft)="openSubmissionModal($event)"
        (reviewSubmission)="onReviewFromDetail($event)"
        (submitProof)="openPublishProofModal($event)"
      ></app-deliverable-detail-modal>
    </div>
  `
})
export class DeliverableListComponent implements OnInit {
  private deliverableService = inject(DeliverableService);
  private campaignService = inject(CampaignService);
  private creatorService = inject(CreatorService);

  readonly campaigns = signal<CampaignSummary[]>([]);
  readonly deliverables = signal<Deliverable[]>([]);
  readonly rosterCreators = signal<CampaignCreator[]>([]);
  readonly rosterCreatorsMap = signal<Map<string, CampaignCreator>>(new Map());
  readonly creatorsMap = signal<Map<string, CreatorSummary>>(new Map());

  selectedCampaignId = '';
  selectedStatus = signal<string>('ALL');
  readonly isLoading = signal<boolean>(false);

  // Status Filter Tabs
  readonly statusTabs = [
    { label: 'All Items', value: 'ALL' },
    { label: 'Submitted (Review)', value: 'Submitted' },
    { label: 'Revision Requested', value: 'RevisionRequested' },
    { label: 'Approved', value: 'Approved' },
    { label: 'Completed', value: 'Completed' },
    { label: 'Draft', value: 'Draft' }
  ];

  // Modals signals
  readonly isModalOpen = signal<boolean>(false);
  readonly isSubmissionModalOpen = signal<boolean>(false);
  readonly isReviewModalOpen = signal<boolean>(false);
  readonly isProofModalOpen = signal<boolean>(false);
  readonly isDetailModalOpen = signal<boolean>(false);

  readonly selectedDeliverableForEdit = signal<Deliverable | null>(null);
  readonly selectedDeliverableForAction = signal<Deliverable | null>(null);
  readonly selectedDeliverableForDetail = signal<Deliverable | null>(null);
  readonly selectedSubmissionForReview = signal<ContentSubmission | null>(null);

  // Icons
  readonly FileCheck2Icon = FileCheck2;
  readonly CalendarIcon = Calendar;
  readonly LayersIcon = Layers;
  readonly SearchIcon = Search;
  readonly PlusIcon = Plus;
  readonly EditIcon = Edit;
  readonly Trash2Icon = Trash2;
  readonly EyeIcon = Eye;
  readonly UploadCloudIcon = UploadCloud;
  readonly MessageSquareIcon = MessageSquare;
  readonly GlobeIcon = Globe;
  readonly ExternalLinkIcon = ExternalLink;
  readonly CheckCircle2Icon = CheckCircle2;
  readonly ClockIcon = Clock;
  readonly AlertTriangleIcon = AlertTriangle;
  readonly Loader2Icon = Loader2;
  readonly RefreshCwIcon = RefreshCw;
  readonly UsersIcon = Users;
  readonly MegaphoneIcon = Megaphone;
  readonly Share2Icon = Share2;

  ngOnInit(): void {
    this.loadCampaigns();
    this.loadCreators();
  }

  private loadCampaigns(): void {
    this.campaignService.getCampaigns('', undefined, undefined, 1, 100).subscribe({
      next: res => {
        if (res.success && res.data) {
          this.campaigns.set(res.data.items);
          if (res.data.items.length > 0) {
            this.selectedCampaignId = res.data.items[0].id;
            this.onCampaignChange();
          }
        }
      }
    });
  }

  private loadCreators(): void {
    this.creatorService.getCreators('', undefined, undefined, undefined, 1, 100).subscribe({
      next: res => {
        if (res.success && res.data) {
          const map = new Map<string, CreatorSummary>();
          for (const c of res.data.items) {
            map.set(c.id, c);
          }
          this.creatorsMap.set(map);
        }
      }
    });
  }

  onCampaignChange(): void {
    this.loadRosterCreators();
    this.loadDeliverables();
  }

  private loadRosterCreators(): void {
    if (!this.selectedCampaignId) return;

    this.campaignService.getCampaignRoster(this.selectedCampaignId).subscribe({
      next: res => {
        if (res.success && res.data) {
          this.rosterCreators.set(res.data);
          const map = new Map<string, CampaignCreator>();
          for (const rc of res.data) {
            map.set(rc.id, rc);
          }
          this.rosterCreatorsMap.set(map);
        }
      }
    });
  }

  loadDeliverables(): void {
    if (!this.selectedCampaignId) return;

    this.isLoading.set(true);
    this.deliverableService.getDeliverablesByCampaign(this.selectedCampaignId).subscribe({
      next: res => {
        this.isLoading.set(false);
        if (res.success && res.data) {
          this.deliverables.set(res.data);
        }
      },
      error: () => {
        this.isLoading.set(false);
        toast.error('Failed to load deliverables');
      }
    });
  }

  filteredDeliverables(): Deliverable[] {
    const list = this.deliverables();
    const status = this.selectedStatus();
    if (status === 'ALL') return list;
    if (status === 'Completed') {
      return list.filter(d => d.status === 'Completed' || d.status === 'Published');
    }
    return list.filter(d => d.status === status);
  }

  getCreatorNameForRoster(campaignCreatorId: string): string {
    const rosterItem = this.rosterCreatorsMap().get(campaignCreatorId);
    if (!rosterItem) return 'Creator';
    const creator = this.creatorsMap().get(rosterItem.creatorId);
    return creator ? creator.fullName : `Creator #${rosterItem.creatorId.substring(0, 6)}`;
  }

  getDueDateBadgeClass(dueDateStr: string, status: string): string {
    if (status === 'Completed' || status === 'Published' || status === 'Approved') {
      return 'bg-slate-100 text-slate-600';
    }
    const due = new Date(dueDateStr).getTime();
    const now = new Date().getTime();
    const diffDays = Math.ceil((due - now) / (1000 * 60 * 60 * 24));

    if (diffDays < 0) {
      return 'bg-rose-100 text-rose-700 font-bold'; // Overdue
    }
    if (diffDays <= 3) {
      return 'bg-amber-100 text-amber-800 font-bold'; // Urgency
    }
    return 'bg-slate-100 text-slate-700';
  }

  getStatusBadgeClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'approved':
        return 'bg-emerald-50 text-emerald-700 border border-emerald-200/60';
      case 'submitted':
        return 'bg-amber-50 text-amber-700 border border-amber-200/60';
      case 'revisionrequested':
        return 'bg-rose-50 text-rose-700 border border-rose-200/60';
      case 'completed':
      case 'published':
        return 'bg-indigo-50 text-indigo-700 border border-indigo-200/60';
      default:
        return 'bg-slate-100 text-slate-700 border border-slate-200/60';
    }
  }

  openCreateModal(): void {
    this.selectedDeliverableForEdit.set(null);
    this.isModalOpen.set(true);
  }

  openEditModal(del: Deliverable): void {
    this.selectedDeliverableForEdit.set(del);
    this.isModalOpen.set(true);
  }

  openDetailModal(del: Deliverable): void {
    this.selectedDeliverableForDetail.set(del);
    this.isDetailModalOpen.set(true);
  }

  openSubmissionModal(del: Deliverable): void {
    this.selectedDeliverableForAction.set(del);
    this.isSubmissionModalOpen.set(true);
  }

  openPublishProofModal(del: Deliverable): void {
    this.selectedDeliverableForAction.set(del);
    this.isProofModalOpen.set(true);
  }

  triggerReviewLatest(del: Deliverable): void {
    this.selectedDeliverableForAction.set(del);
    this.deliverableService.getContentSubmissions(del.id).subscribe({
      next: res => {
        if (res.success && res.data && res.data.length > 0) {
          // Latest submission
          const latest = res.data[res.data.length - 1];
          this.selectedSubmissionForReview.set(latest);
          this.isReviewModalOpen.set(true);
        } else {
          toast.info('No draft submissions found to review.');
        }
      }
    });
  }

  onReviewFromDetail(event: { submission: ContentSubmission; deliverable: Deliverable }): void {
    this.selectedDeliverableForAction.set(event.deliverable);
    this.selectedSubmissionForReview.set(event.submission);
    this.isReviewModalOpen.set(true);
  }

  closeModal(): void {
    this.isModalOpen.set(false);
    this.selectedDeliverableForEdit.set(null);
  }

  closeSubmissionModal(): void {
    this.isSubmissionModalOpen.set(false);
    this.selectedDeliverableForAction.set(null);
  }

  closeReviewModal(): void {
    this.isReviewModalOpen.set(false);
    this.selectedSubmissionForReview.set(null);
    this.selectedDeliverableForAction.set(null);
  }

  closeProofModal(): void {
    this.isProofModalOpen.set(false);
    this.selectedDeliverableForAction.set(null);
  }

  closeDetailModal(): void {
    this.isDetailModalOpen.set(false);
    this.selectedDeliverableForDetail.set(null);
  }

  onDeliverableSaved(): void {
    this.loadDeliverables();
  }

  onDraftSubmitted(): void {
    this.loadDeliverables();
  }

  onReviewDone(): void {
    this.loadDeliverables();
  }

  onProofSaved(): void {
    this.loadDeliverables();
  }
}
