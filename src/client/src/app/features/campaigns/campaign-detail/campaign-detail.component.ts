import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import {
  LucideAngularModule,
  ArrowLeft,
  Megaphone,
  Building2,
  Calendar,
  DollarSign,
  Users,
  Edit,
  Trash2,
  FolderKanban,
  FileCheck2,
  BarChart3,
  CheckCircle2,
  Clock,
  Ban,
  Loader2,
  Sparkles,
  ExternalLink,
  Plus,
  AlertCircle
} from 'lucide-angular';
import { toast } from 'ngx-sonner';
import { CampaignService } from '../../../core/services/campaign.service';
import { ClientService } from '../../../core/services/client.service';
import { Campaign, CampaignCreator } from '../../../core/models/campaign.model';
import { Client } from '../../../core/models/client.model';
import { CampaignModalComponent } from '../campaign-modal/campaign-modal.component';

@Component({
  selector: 'app-campaign-detail',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    LucideAngularModule,
    CampaignModalComponent
  ],
  template: `
    <div class="space-y-6">
      <!-- Breadcrumb & Top Bar -->
      <div class="flex items-center justify-between">
        <a
          routerLink="/campaigns"
          class="inline-flex items-center gap-2 text-xs font-bold text-slate-500 hover:text-slate-900 transition"
        >
          <lucide-icon [img]="ArrowLeftIcon" class="w-4 h-4"></lucide-icon>
          <span>Back to Campaigns</span>
        </a>

        @if (campaign()) {
          <div class="flex items-center gap-2">
            <button
              type="button"
              (click)="isEditModalOpen.set(true)"
              class="px-3.5 py-2 rounded-xl border border-slate-200 text-slate-700 hover:bg-slate-50 text-xs font-semibold transition flex items-center gap-1.5"
            >
              <lucide-icon [img]="EditIcon" class="w-3.5 h-3.5"></lucide-icon>
              <span>Edit Brief</span>
            </button>

            <button
              type="button"
              (click)="isDeleteModalOpen.set(true)"
              class="px-3.5 py-2 rounded-xl border border-rose-200 text-rose-600 hover:bg-rose-50 text-xs font-semibold transition flex items-center gap-1.5"
            >
              <lucide-icon [img]="Trash2Icon" class="w-3.5 h-3.5"></lucide-icon>
              <span>Delete</span>
            </button>
          </div>
        }
      </div>

      @if (isLoading()) {
        <div class="p-16 bg-white rounded-3xl border border-slate-200/80 text-center space-y-3 shadow-xs">
          <lucide-icon [img]="Loader2Icon" class="w-8 h-8 text-indigo-600 animate-spin mx-auto"></lucide-icon>
          <p class="text-xs text-slate-500 font-medium">Loading campaign overview...</p>
        </div>
      } @else if (campaign()) {
        <!-- Campaign Header Hero Card -->
        <div class="p-6 sm:p-8 rounded-3xl bg-slate-900 text-white relative overflow-hidden shadow-xl border border-slate-800">
          <div class="relative z-10 space-y-6">
            <div class="flex flex-col md:flex-row md:items-start justify-between gap-4">
              <div class="space-y-2">
                <div class="flex flex-wrap items-center gap-2">
                  <span
                    class="px-3 py-1 rounded-full text-xs font-bold uppercase tracking-wider inline-flex items-center gap-1.5"
                    [ngClass]="getStatusBadgeClass(campaign()!.status)"
                  >
                    <span class="w-2 h-2 rounded-full" [ngClass]="getStatusDotClass(campaign()!.status)"></span>
                    <span>{{ campaign()!.status }}</span>
                  </span>

                  @if (client()) {
                    <span class="px-3 py-1 rounded-full bg-white/10 text-indigo-200 text-xs font-semibold flex items-center gap-1.5 border border-white/10">
                      <lucide-icon [img]="Building2Icon" class="w-3.5 h-3.5 text-indigo-300"></lucide-icon>
                      <span>{{ client()!.name }}</span>
                    </span>
                  }
                </div>

                <h1 class="text-2xl sm:text-3xl font-black text-white tracking-tight">
                  {{ campaign()!.title }}
                </h1>
              </div>

              <!-- Status Switcher Dropdown -->
              <div class="flex items-center gap-2 bg-slate-800/80 p-1.5 rounded-2xl border border-slate-700 shrink-0">
                <span class="text-[11px] font-semibold text-slate-400 pl-2">Status:</span>
                <select
                  [ngModel]="campaign()!.status"
                  (ngModelChange)="onStatusChange($event)"
                  class="text-xs font-bold bg-slate-900 text-white rounded-xl px-3 py-1.5 border border-slate-700 focus:outline-hidden focus:border-indigo-500 transition cursor-pointer"
                >
                  <option value="Draft">Draft</option>
                  <option value="Active">Active</option>
                  <option value="Completed">Completed</option>
                  <option value="Cancelled">Cancelled</option>
                </select>
              </div>
            </div>

            <!-- Stats Bar -->
            <div class="grid grid-cols-1 sm:grid-cols-3 gap-4 pt-4 border-t border-slate-800">
              <div class="space-y-1">
                <span class="text-[11px] font-bold text-slate-400 uppercase tracking-wider">Total Campaign Budget</span>
                <p class="text-xl font-black text-emerald-400">{{ formatCurrency(campaign()!.budget) }}</p>
              </div>

              <div class="space-y-1">
                <span class="text-[11px] font-bold text-slate-400 uppercase tracking-wider">Campaign Schedule</span>
                <p class="text-sm font-bold text-slate-200 flex items-center gap-1.5">
                  <lucide-icon [img]="CalendarIcon" class="w-4 h-4 text-indigo-400"></lucide-icon>
                  <span>{{ campaign()!.startDate | date:'MMM d' }} - {{ campaign()!.endDate | date:'MMM d, yyyy' }}</span>
                </p>
              </div>

              <div class="space-y-1">
                <span class="text-[11px] font-bold text-slate-400 uppercase tracking-wider">Influencer Roster</span>
                <p class="text-sm font-bold text-slate-200 flex items-center gap-1.5">
                  <lucide-icon [img]="UsersIcon" class="w-4 h-4 text-violet-400"></lucide-icon>
                  <span>{{ roster().length }} Creator{{ roster().length === 1 ? '' : 's' }} Enrolled</span>
                </p>
              </div>
            </div>
          </div>
        </div>

        <!-- Quick Module Navigation Shortcuts -->
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <!-- Roster Board Link -->
          <a
            routerLink="/creators"
            class="p-5 bg-white rounded-2xl border border-slate-200/80 shadow-xs hover:shadow-md hover:border-indigo-300 transition group flex items-center justify-between"
          >
            <div class="flex items-center gap-3.5">
              <div class="w-10 h-10 rounded-xl bg-violet-50 text-violet-600 flex items-center justify-center font-bold">
                <lucide-icon [img]="FolderKanbanIcon" class="w-5 h-5"></lucide-icon>
              </div>
              <div>
                <h4 class="text-xs font-bold text-slate-900 group-hover:text-indigo-600 transition">Creator Roster Board</h4>
                <p class="text-[11px] text-slate-400">Kanban stage & creator assignments</p>
              </div>
            </div>
            <lucide-icon [img]="ExternalLinkIcon" class="w-4 h-4 text-slate-400 group-hover:text-indigo-600 transition"></lucide-icon>
          </a>

          <!-- Deliverables Workflow Link -->
          <a
            routerLink="/deliverables"
            class="p-5 bg-white rounded-2xl border border-slate-200/80 shadow-xs hover:shadow-md hover:border-indigo-300 transition group flex items-center justify-between"
          >
            <div class="flex items-center gap-3.5">
              <div class="w-10 h-10 rounded-xl bg-amber-50 text-amber-600 flex items-center justify-center font-bold">
                <lucide-icon [img]="FileCheck2Icon" class="w-5 h-5"></lucide-icon>
              </div>
              <div>
                <h4 class="text-xs font-bold text-slate-900 group-hover:text-indigo-600 transition">Deliverables & Review</h4>
                <p class="text-[11px] text-slate-400">Content submissions & approval loop</p>
              </div>
            </div>
            <lucide-icon [img]="ExternalLinkIcon" class="w-4 h-4 text-slate-400 group-hover:text-indigo-600 transition"></lucide-icon>
          </a>

          <!-- Reports Link -->
          <a
            routerLink="/reports"
            class="p-5 bg-white rounded-2xl border border-slate-200/80 shadow-xs hover:shadow-md hover:border-indigo-300 transition group flex items-center justify-between"
          >
            <div class="flex items-center gap-3.5">
              <div class="w-10 h-10 rounded-xl bg-emerald-50 text-emerald-600 flex items-center justify-center font-bold">
                <lucide-icon [img]="BarChart3Icon" class="w-5 h-5"></lucide-icon>
              </div>
              <div>
                <h4 class="text-xs font-bold text-slate-900 group-hover:text-indigo-600 transition">Campaign Analytics & PDF</h4>
                <p class="text-[11px] text-slate-400">Async PDF report generation engine</p>
              </div>
            </div>
            <lucide-icon [img]="ExternalLinkIcon" class="w-4 h-4 text-slate-400 group-hover:text-indigo-600 transition"></lucide-icon>
          </a>
        </div>

        <!-- Main Info Grid (Brief + Roster List) -->
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <!-- Left 2 Cols: Campaign Brief Details -->
          <div class="lg:col-span-2 space-y-6">
            <div class="p-6 bg-white rounded-3xl border border-slate-200/80 shadow-xs space-y-4">
              <h3 class="text-sm font-bold text-slate-900 uppercase tracking-wider flex items-center gap-2">
                <span>Campaign Brief & Objectives</span>
              </h3>

              @if (campaign()!.description) {
                <p class="text-xs text-slate-700 leading-relaxed whitespace-pre-line bg-slate-50 p-4 rounded-2xl border border-slate-100">
                  {{ campaign()!.description }}
                </p>
              } @else {
                <div class="p-4 rounded-2xl bg-slate-50 border border-slate-100 text-xs text-slate-400 text-center">
                  No brief description provided for this campaign. Click "Edit Brief" to add guidelines.
                </div>
              }
            </div>

            <!-- Client & Brand Information Card -->
            @if (client()) {
              <div class="p-6 bg-white rounded-3xl border border-slate-200/80 shadow-xs space-y-4">
                <div class="flex items-center justify-between">
                  <h3 class="text-sm font-bold text-slate-900 uppercase tracking-wider">Brand & PIC Contacts</h3>
                  <span class="text-xs font-bold text-indigo-600">{{ client()!.name }}</span>
                </div>

                @if (client()!.contacts && client()!.contacts.length > 0) {
                  <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
                    @for (contact of client()!.contacts; track $index) {
                      <div class="p-3.5 rounded-2xl bg-slate-50 border border-slate-200/70 space-y-1 text-xs">
                        <p class="font-bold text-slate-900">{{ contact.name }}</p>
                        @if (contact.position) {
                          <p class="text-[11px] text-indigo-600 font-semibold">{{ contact.position }}</p>
                        }
                        <p class="text-[11px] text-slate-500">{{ contact.email }}</p>
                        @if (contact.phoneNumber) {
                          <p class="text-[11px] text-slate-400">{{ contact.phoneNumber }}</p>
                        }
                      </div>
                    }
                  </div>
                } @else {
                  <p class="text-xs text-slate-400">No PIC contacts registered for this brand.</p>
                }
              </div>
            }
          </div>

          <!-- Right 1 Col: Enrolled Creators Roster -->
          <div class="space-y-6">
            <div class="p-6 bg-white rounded-3xl border border-slate-200/80 shadow-xs space-y-4">
              <div class="flex items-center justify-between">
                <h3 class="text-sm font-bold text-slate-900 uppercase tracking-wider">
                  Creators Lineup ({{ roster().length }})
                </h3>
                <a
                  routerLink="/creators"
                  class="text-xs font-bold text-indigo-600 hover:text-indigo-700 transition"
                >
                  Manage
                </a>
              </div>

              @if (roster().length === 0) {
                <div class="p-6 rounded-2xl bg-slate-50 border border-slate-200/60 text-center space-y-2">
                  <lucide-icon [img]="UsersIcon" class="w-6 h-6 text-slate-400 mx-auto"></lucide-icon>
                  <p class="text-xs text-slate-600 font-medium">No creators assigned yet.</p>
                  <p class="text-[11px] text-slate-400">Go to Creator CRM / Roster to enroll influencers.</p>
                </div>
              } @else {
                <div class="space-y-3">
                  @for (creator of roster(); track creator.id) {
                    <div class="p-3.5 rounded-2xl bg-slate-50 border border-slate-200/70 flex items-center justify-between gap-3 text-xs">
                      <div>
                        <span class="font-bold text-slate-900 block truncate max-w-[150px]">
                          Creator #{{ creator.creatorId.substring(0, 8) }}
                        </span>
                        <span class="text-[11px] font-semibold text-emerald-600">
                          {{ formatCurrency(creator.agreedRate) }}
                        </span>
                      </div>

                      <span class="px-2 py-0.5 rounded-md bg-indigo-50 text-indigo-700 text-[10px] font-bold">
                        {{ creator.status }}
                      </span>
                    </div>
                  }
                </div>
              }
            </div>
          </div>
        </div>
      }

      <!-- Edit Campaign Modal -->
      <app-campaign-modal
        [isOpen]="isEditModalOpen()"
        [campaign]="campaign()"
        (close)="isEditModalOpen.set(false)"
        (saved)="onCampaignUpdated($event)"
      ></app-campaign-modal>

      <!-- Delete Campaign Modal -->
      @if (isDeleteModalOpen() && campaign()) {
        <div class="fixed inset-0 z-50 overflow-y-auto bg-slate-900/60 backdrop-blur-xs flex items-center justify-center p-4 animate-in fade-in duration-200">
          <div
            class="bg-white rounded-3xl shadow-2xl border border-slate-100 w-full max-w-md p-6 space-y-5"
            (click)="$event.stopPropagation()"
          >
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-2xl bg-rose-50 text-rose-600 flex items-center justify-center shrink-0">
                <lucide-icon [img]="AlertCircleIcon" class="w-5 h-5"></lucide-icon>
              </div>
              <div>
                <h3 class="text-base font-bold text-slate-900">Delete Campaign</h3>
                <p class="text-xs text-slate-500">This action cannot be undone.</p>
              </div>
            </div>

            <p class="text-xs text-slate-600 leading-relaxed">
              Are you sure you want to delete <span class="font-bold text-slate-900">"{{ campaign()?.title }}"</span>?
            </p>

            <div class="flex items-center justify-end gap-3 pt-2">
              <button
                type="button"
                (click)="isDeleteModalOpen.set(false)"
                [disabled]="isDeleting()"
                class="px-4 py-2 rounded-xl border border-slate-200 text-slate-700 text-xs font-semibold hover:bg-slate-50 transition"
              >
                Cancel
              </button>

              <button
                type="button"
                (click)="executeDelete()"
                [disabled]="isDeleting()"
                class="px-4 py-2 rounded-xl bg-rose-600 hover:bg-rose-500 text-white text-xs font-semibold shadow-xs transition flex items-center gap-2"
              >
                @if (isDeleting()) {
                  <lucide-icon [img]="Loader2Icon" class="w-3.5 h-3.5 animate-spin"></lucide-icon>
                  <span>Deleting...</span>
                } @else {
                  <lucide-icon [img]="Trash2Icon" class="w-3.5 h-3.5"></lucide-icon>
                  <span>Confirm Delete</span>
                }
              </button>
            </div>
          </div>
        </div>
      }
    </div>
  `
})
export class CampaignDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private campaignService = inject(CampaignService);
  private clientService = inject(ClientService);

  readonly campaign = signal<Campaign | null>(null);
  readonly client = signal<Client | null>(null);
  readonly roster = signal<CampaignCreator[]>([]);
  readonly isLoading = signal<boolean>(false);
  readonly isDeleting = signal<boolean>(false);

  readonly isEditModalOpen = signal<boolean>(false);
  readonly isDeleteModalOpen = signal<boolean>(false);

  // Icons
  readonly ArrowLeftIcon = ArrowLeft;
  readonly MegaphoneIcon = Megaphone;
  readonly Building2Icon = Building2;
  readonly CalendarIcon = Calendar;
  readonly DollarSignIcon = DollarSign;
  readonly UsersIcon = Users;
  readonly EditIcon = Edit;
  readonly Trash2Icon = Trash2;
  readonly FolderKanbanIcon = FolderKanban;
  readonly FileCheck2Icon = FileCheck2;
  readonly BarChart3Icon = BarChart3;
  readonly CheckCircle2Icon = CheckCircle2;
  readonly ClockIcon = Clock;
  readonly BanIcon = Ban;
  readonly Loader2Icon = Loader2;
  readonly SparklesIcon = Sparkles;
  readonly ExternalLinkIcon = ExternalLink;
  readonly PlusIcon = Plus;
  readonly AlertCircleIcon = AlertCircle;

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadCampaignData(id);
    }
  }

  loadCampaignData(id: string): void {
    this.isLoading.set(true);
    this.campaignService.getCampaignById(id).subscribe({
      next: res => {
        this.isLoading.set(false);
        if (res.success && res.data) {
          this.campaign.set(res.data);
          this.loadClientData(res.data.clientId);
          this.loadRosterData(res.data.id);
        }
      },
      error: () => {
        this.isLoading.set(false);
        toast.error('Failed to load campaign');
        this.router.navigate(['/campaigns']);
      }
    });
  }

  private loadClientData(clientId: string): void {
    this.clientService.getClientById(clientId).subscribe({
      next: res => {
        if (res.success && res.data) {
          this.client.set(res.data);
        }
      }
    });
  }

  private loadRosterData(campaignId: string): void {
    this.campaignService.getCampaignRoster(campaignId).subscribe({
      next: res => {
        if (res.success && res.data) {
          this.roster.set(res.data);
        }
      }
    });
  }

  onStatusChange(newStatus: string): void {
    const current = this.campaign();
    if (!current) return;

    this.campaignService.updateCampaignStatus(current.id, newStatus).subscribe({
      next: res => {
        if (res.success && res.data) {
          this.campaign.set(res.data);
          toast.success(`Campaign status updated to ${newStatus}`);
        }
      },
      error: err => {
        const msg = err.error?.detail || err.error?.message || 'Failed to update status';
        toast.error(msg);
      }
    });
  }

  onCampaignUpdated(updated: Campaign): void {
    this.campaign.set(updated);
    this.isEditModalOpen.set(false);
  }

  executeDelete(): void {
    const current = this.campaign();
    if (!current) return;

    this.isDeleting.set(true);
    this.campaignService.deleteCampaign(current.id).subscribe({
      next: () => {
        this.isDeleting.set(false);
        toast.success(`Campaign "${current.title}" deleted`);
        this.router.navigate(['/campaigns']);
      },
      error: err => {
        this.isDeleting.set(false);
        const msg = err.error?.detail || err.error?.message || 'Failed to delete campaign';
        toast.error(msg);
      }
    });
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('id-ID', {
      style: 'currency',
      currency: 'IDR',
      maximumFractionDigits: 0
    }).format(amount);
  }

  getStatusBadgeClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'active':
        return 'bg-emerald-500/20 text-emerald-300 border border-emerald-400/30';
      case 'draft':
        return 'bg-slate-700/60 text-slate-300 border border-slate-600';
      case 'completed':
        return 'bg-indigo-500/20 text-indigo-300 border border-indigo-400/30';
      case 'cancelled':
        return 'bg-rose-500/20 text-rose-300 border border-rose-400/30';
      default:
        return 'bg-slate-700/60 text-slate-300 border border-slate-600';
    }
  }

  getStatusDotClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'active':
        return 'bg-emerald-400';
      case 'draft':
        return 'bg-slate-400';
      case 'completed':
        return 'bg-indigo-400';
      case 'cancelled':
        return 'bg-rose-400';
      default:
        return 'bg-slate-400';
    }
  }
}
