import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  LucideAngularModule,
  Users,
  FolderKanban,
  Search,
  Plus,
  Edit,
  Trash2,
  Eye,
  UserPlus,
  ExternalLink,
  ChevronLeft,
  ChevronRight,
  AlertCircle,
  Loader2,
  RefreshCw,
  Tag,
  Share2,
  CheckCircle2,
  SlidersHorizontal
} from 'lucide-angular';
import { toast } from 'ngx-sonner';
import { CreatorService } from '../../core/services/creator.service';
import { Creator, CreatorSummary } from '../../core/models/creator.model';
import { CreatorModalComponent } from './creator-modal/creator-modal.component';
import { CreatorDetailModalComponent } from './creator-detail-modal/creator-detail-modal.component';
import { AddToRosterModalComponent } from './add-to-roster-modal/add-to-roster-modal.component';
import { RosterKanbanComponent } from './roster-kanban/roster-kanban.component';

@Component({
  selector: 'app-creator-hub',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LucideAngularModule,
    CreatorModalComponent,
    CreatorDetailModalComponent,
    AddToRosterModalComponent,
    RosterKanbanComponent
  ],
  template: `
    <div class="space-y-6">
      <!-- Page Header Banner -->
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <div class="flex items-center gap-2">
            <h1 class="text-2xl font-black tracking-tight text-slate-900">Creator CRM & Roster</h1>
            <span class="px-2.5 py-0.5 rounded-full bg-violet-50 text-violet-700 text-xs font-bold">
              {{ totalCount() }} Talents
            </span>
          </div>
          <p class="text-xs text-slate-500 mt-1">
            Influencer CRM database, social media reach discovery, and drag-and-drop campaign roster boards.
          </p>
        </div>

        <div class="flex items-center gap-3">
          @if (activeTab() === 'crm') {
            <button
              type="button"
              (click)="loadCreators()"
              class="p-2.5 rounded-xl border border-slate-200 text-slate-600 hover:bg-slate-50 hover:text-slate-900 transition"
              title="Refresh Directory"
            >
              <lucide-icon [img]="RefreshCwIcon" class="w-4 h-4" [class.animate-spin]="isLoading()"></lucide-icon>
            </button>

            <button
              type="button"
              (click)="openCreateModal()"
              class="px-4 py-2.5 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white text-xs font-semibold shadow-md shadow-indigo-600/20 transition flex items-center gap-2"
            >
              <lucide-icon [img]="PlusIcon" class="w-4 h-4"></lucide-icon>
              <span>Add Influencer</span>
            </button>
          }
        </div>
      </div>

      <!-- Navigation Tabs: CRM Directory vs Roster Kanban -->
      <div class="flex items-center gap-2 border-b border-slate-200/80 pb-3">
        <button
          type="button"
          (click)="activeTab.set('crm')"
          class="px-4 py-2 rounded-xl text-xs font-bold transition flex items-center gap-2"
          [class]="activeTab() === 'crm'
            ? 'bg-slate-900 text-white shadow-xs'
            : 'text-slate-600 hover:bg-slate-100'"
        >
          <lucide-icon [img]="UsersIcon" class="w-4 h-4"></lucide-icon>
          <span>Creator CRM Directory</span>
        </button>

        <button
          type="button"
          (click)="activeTab.set('roster')"
          class="px-4 py-2 rounded-xl text-xs font-bold transition flex items-center gap-2"
          [class]="activeTab() === 'roster'
            ? 'bg-slate-900 text-white shadow-xs'
            : 'text-slate-600 hover:bg-slate-100'"
        >
          <lucide-icon [img]="FolderKanbanIcon" class="w-4 h-4"></lucide-icon>
          <span>Campaign Roster Board</span>
        </button>
      </div>

      <!-- TAB 1: CREATOR CRM DIRECTORY -->
      @if (activeTab() === 'crm') {
        <!-- Filter Toolbar -->
        <div class="p-4 bg-white rounded-2xl border border-slate-200/80 shadow-xs space-y-3">
          <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
            <!-- Search input -->
            <div class="relative flex-1 max-w-md">
              <lucide-icon
                [img]="SearchIcon"
                class="w-4 h-4 absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-400"
              ></lucide-icon>
              <input
                type="text"
                [(ngModel)]="searchQuery"
                (ngModelChange)="onSearchChange()"
                placeholder="Search by creator name..."
                class="w-full pl-9.5 pr-4 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition"
              />
            </div>

            <!-- Filters (Niche, Status) -->
            <div class="flex items-center gap-2 overflow-x-auto">
              <!-- Niche Filter -->
              <select
                [(ngModel)]="selectedNiche"
                (ngModelChange)="onFilterChange()"
                class="px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-700 focus:outline-hidden focus:border-indigo-600 transition"
              >
                <option value="ALL">All Niches</option>
                @for (n of nicheOptions; track n) {
                  <option [value]="n">{{ n }}</option>
                }
              </select>

              <!-- Status Filter -->
              <select
                [(ngModel)]="selectedStatus"
                (ngModelChange)="onFilterChange()"
                class="px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-700 focus:outline-hidden focus:border-indigo-600 transition"
              >
                <option value="ALL">All Statuses</option>
                <option value="Active">Active</option>
                <option value="Inactive">Inactive</option>
                <option value="Blacklisted">Blacklisted</option>
              </select>
            </div>
          </div>
        </div>

        <!-- Creators Data Table -->
        <div class="bg-white rounded-2xl border border-slate-200/80 shadow-xs overflow-hidden">
          @if (isLoading()) {
            <div class="p-16 text-center space-y-3">
              <lucide-icon [img]="Loader2Icon" class="w-8 h-8 text-indigo-600 animate-spin mx-auto"></lucide-icon>
              <p class="text-xs text-slate-500 font-medium">Loading creator database...</p>
            </div>
          } @else if (creators().length === 0) {
            <div class="p-16 text-center space-y-3 max-w-md mx-auto">
              <div class="w-12 h-12 rounded-2xl bg-violet-50 text-violet-600 flex items-center justify-center mx-auto">
                <lucide-icon [img]="UsersIcon" class="w-6 h-6"></lucide-icon>
              </div>
              <h3 class="text-sm font-bold text-slate-900">No Creators Found</h3>
              <p class="text-xs text-slate-500 leading-relaxed">
                {{ searchQuery || selectedNiche !== 'ALL' || selectedStatus !== 'ALL' ? 'No influencer profiles match your filter options.' : 'Start discovering and adding KOL influencers into your agency CRM.' }}
              </p>
              @if (!searchQuery && selectedNiche === 'ALL') {
                <button
                  type="button"
                  (click)="openCreateModal()"
                  class="mt-2 inline-flex items-center gap-1.5 px-4 py-2 rounded-xl bg-indigo-600 text-white text-xs font-semibold hover:bg-indigo-500 transition"
                >
                  <lucide-icon [img]="PlusIcon" class="w-3.5 h-3.5"></lucide-icon>
                  <span>Add Influencer</span>
                </button>
              }
            </div>
          } @else {
            <div class="overflow-x-auto">
              <table class="w-full text-left border-collapse">
                <thead>
                  <tr class="border-b border-slate-100 bg-slate-50/50 text-[11px] font-bold text-slate-500 uppercase tracking-wider">
                    <th class="py-3.5 px-6">Creator / KOL</th>
                    <th class="py-3.5 px-6">Category / Niche</th>
                    <th class="py-3.5 px-6">Total Reach</th>
                    <th class="py-3.5 px-6">Channels</th>
                    <th class="py-3.5 px-6">Status</th>
                    <th class="py-3.5 px-6 text-right">Actions</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-100 text-xs">
                  @for (creator of creators(); track creator.id) {
                    <tr class="hover:bg-slate-50/75 transition group">
                      <!-- Creator Name -->
                      <td class="py-4 px-6">
                        <div class="flex items-center gap-3">
                          <div class="w-9 h-9 rounded-xl bg-violet-50 text-violet-700 font-black flex items-center justify-center shrink-0 border border-violet-100">
                            {{ creator.fullName.charAt(0).toUpperCase() }}
                          </div>
                          <div>
                            <p class="font-bold text-slate-900 group-hover:text-indigo-600 transition">
                              {{ creator.fullName }}
                            </p>
                            @if (creator.email) {
                              <p class="text-[11px] text-slate-400 truncate max-w-[180px]">{{ creator.email }}</p>
                            }
                          </div>
                        </div>
                      </td>

                      <!-- Niche -->
                      <td class="py-4 px-6">
                        <span class="inline-flex items-center gap-1 px-2.5 py-1 rounded-full bg-slate-100 text-slate-700 font-semibold text-[11px]">
                          {{ creator.niche }}
                        </span>
                      </td>

                      <!-- Total Reach -->
                      <td class="py-4 px-6 font-extrabold text-slate-900">
                        {{ formatFollowers(creator.totalFollowers) }}
                      </td>

                      <!-- Channels count -->
                      <td class="py-4 px-6 text-slate-500 font-medium">
                        {{ creator.socialAccountsCount }} {{ creator.socialAccountsCount === 1 ? 'channel' : 'channels' }}
                      </td>

                      <!-- Status -->
                      <td class="py-4 px-6">
                        <span
                          class="px-2.5 py-0.5 rounded-md text-[11px] font-bold"
                          [class]="creator.status === 'Active' ? 'bg-emerald-50 text-emerald-700' : 'bg-slate-100 text-slate-600'"
                        >
                          {{ creator.status }}
                        </span>
                      </td>

                      <!-- Actions -->
                      <td class="py-4 px-6 text-right">
                        <div class="flex items-center justify-end gap-1.5">
                          <button
                            type="button"
                            (click)="openAssignToRosterModal(creator)"
                            class="p-2 rounded-lg text-slate-400 hover:text-emerald-600 hover:bg-emerald-50 transition"
                            title="Assign to Campaign Roster"
                          >
                            <lucide-icon [img]="UserPlusIcon" class="w-4 h-4"></lucide-icon>
                          </button>

                          <button
                            type="button"
                            (click)="viewCreatorDetail(creator.id)"
                            class="p-2 rounded-lg text-slate-400 hover:text-indigo-600 hover:bg-indigo-50 transition"
                            title="View Profile"
                          >
                            <lucide-icon [img]="EyeIcon" class="w-4 h-4"></lucide-icon>
                          </button>

                          <button
                            type="button"
                            (click)="openEditModal(creator.id)"
                            class="p-2 rounded-lg text-slate-400 hover:text-indigo-600 hover:bg-indigo-50 transition"
                            title="Edit Creator"
                          >
                            <lucide-icon [img]="EditIcon" class="w-4 h-4"></lucide-icon>
                          </button>

                          <button
                            type="button"
                            (click)="confirmDelete(creator)"
                            class="p-2 rounded-lg text-slate-400 hover:text-rose-600 hover:bg-rose-50 transition"
                            title="Delete Creator"
                          >
                            <lucide-icon [img]="Trash2Icon" class="w-4 h-4"></lucide-icon>
                          </button>
                        </div>
                      </td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>

            <!-- Pagination Footer -->
            <div class="px-6 py-4 border-t border-slate-100 flex items-center justify-between">
              <span class="text-xs text-slate-500">
                Showing {{ (currentPage() - 1) * pageSize + 1 }} to {{ Math.min(currentPage() * pageSize, totalCount()) }} of {{ totalCount() }} creators
              </span>

              <div class="flex items-center gap-2">
                <button
                  type="button"
                  [disabled]="currentPage() <= 1"
                  (click)="goToPage(currentPage() - 1)"
                  class="p-2 rounded-xl border border-slate-200 text-slate-600 hover:bg-slate-50 disabled:opacity-40 disabled:cursor-not-allowed transition"
                >
                  <lucide-icon [img]="ChevronLeftIcon" class="w-4 h-4"></lucide-icon>
                </button>

                <span class="text-xs font-semibold text-slate-700 px-2">
                  {{ currentPage() }} / {{ totalPages() || 1 }}
                </span>

                <button
                  type="button"
                  [disabled]="currentPage() >= totalPages()"
                  (click)="goToPage(currentPage() + 1)"
                  class="p-2 rounded-xl border border-slate-200 text-slate-600 hover:bg-slate-50 disabled:opacity-40 disabled:cursor-not-allowed transition"
                >
                  <lucide-icon [img]="ChevronRightIcon" class="w-4 h-4"></lucide-icon>
                </button>
              </div>
            </div>
          }
        </div>
      }

      <!-- TAB 2: CAMPAIGN ROSTER KANBAN BOARD -->
      @if (activeTab() === 'roster') {
        <app-roster-kanban></app-roster-kanban>
      }

      <!-- Create & Edit Modal -->
      <app-creator-modal
        [isOpen]="isModalOpen()"
        [creator]="selectedCreatorForEdit()"
        (close)="closeModal()"
        (saved)="onCreatorSaved()"
      ></app-creator-modal>

      <!-- View Detail Modal -->
      <app-creator-detail-modal
        [isOpen]="isDetailModalOpen()"
        [creator]="selectedCreatorForDetail()"
        (close)="closeDetailModal()"
        (edit)="onEditFromDetail($event)"
        (assignToRoster)="onAssignFromDetail($event)"
      ></app-creator-detail-modal>

      <!-- Assign to Campaign Roster Modal -->
      <app-add-to-roster-modal
        [isOpen]="isAssignModalOpen()"
        [preselectedCreator]="creatorToAssign()"
        (close)="closeAssignModal()"
        (enrolled)="onRosterEnrolled()"
      ></app-add-to-roster-modal>

      <!-- Delete Confirmation Modal -->
      @if (creatorToDelete()) {
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
                <h3 class="text-base font-bold text-slate-900">Delete Influencer</h3>
                <p class="text-xs text-slate-500">This action cannot be undone.</p>
              </div>
            </div>

            <p class="text-xs text-slate-600 leading-relaxed">
              Are you sure you want to remove <span class="font-bold text-slate-900">"{{ creatorToDelete()?.fullName }}"</span> from your agency CRM directory?
            </p>

            <div class="flex items-center justify-end gap-3 pt-2">
              <button
                type="button"
                (click)="creatorToDelete.set(null)"
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
export class CreatorHubComponent implements OnInit {
  private creatorService = inject(CreatorService);

  readonly Math = Math;

  readonly activeTab = signal<'crm' | 'roster'>('crm');

  // Directory Signals
  readonly creators = signal<CreatorSummary[]>([]);
  readonly totalCount = signal<number>(0);
  readonly totalPages = signal<number>(1);
  readonly currentPage = signal<number>(1);
  readonly pageSize = 10;
  readonly isLoading = signal<boolean>(false);
  readonly isDeleting = signal<boolean>(false);

  // Filters
  searchQuery = '';
  selectedNiche = 'ALL';
  selectedStatus = 'ALL';
  private searchTimeout: any;

  readonly nicheOptions = [
    'Beauty & Skincare',
    'Fashion & Style',
    'Tech & Gadgets',
    'Gaming & Esports',
    'Food & Culinary',
    'Lifestyle & Daily',
    'Travel & Hospitality',
    'Fitness & Health',
    'Parenting & Family',
    'Entertainment & Comedy',
    'Finance & Business',
    'Education & Books'
  ];

  // Modals Signals
  readonly isModalOpen = signal<boolean>(false);
  readonly isDetailModalOpen = signal<boolean>(false);
  readonly isAssignModalOpen = signal<boolean>(false);
  readonly selectedCreatorForEdit = signal<Creator | null>(null);
  readonly selectedCreatorForDetail = signal<Creator | null>(null);
  readonly creatorToAssign = signal<Creator | CreatorSummary | null>(null);
  readonly creatorToDelete = signal<CreatorSummary | null>(null);

  // Icons
  readonly UsersIcon = Users;
  readonly FolderKanbanIcon = FolderKanban;
  readonly SearchIcon = Search;
  readonly PlusIcon = Plus;
  readonly EditIcon = Edit;
  readonly Trash2Icon = Trash2;
  readonly EyeIcon = Eye;
  readonly UserPlusIcon = UserPlus;
  readonly ExternalLinkIcon = ExternalLink;
  readonly ChevronLeftIcon = ChevronLeft;
  readonly ChevronRightIcon = ChevronRight;
  readonly AlertCircleIcon = AlertCircle;
  readonly Loader2Icon = Loader2;
  readonly RefreshCwIcon = RefreshCw;
  readonly TagIcon = Tag;
  readonly Share2Icon = Share2;
  readonly CheckCircle2Icon = CheckCircle2;
  readonly SlidersHorizontalIcon = SlidersHorizontal;

  ngOnInit(): void {
    this.loadCreators();
  }

  loadCreators(): void {
    this.isLoading.set(true);
    this.creatorService
      .getCreators(
        this.searchQuery,
        this.selectedNiche,
        undefined,
        this.selectedStatus,
        this.currentPage(),
        this.pageSize
      )
      .subscribe({
        next: res => {
          this.isLoading.set(false);
          if (res.success && res.data) {
            this.creators.set(res.data.items);
            this.totalCount.set(res.data.totalCount);
            this.totalPages.set(res.data.totalPages);
          }
        },
        error: () => {
          this.isLoading.set(false);
          toast.error('Failed to load creator directory');
        }
      });
  }

  onSearchChange(): void {
    clearTimeout(this.searchTimeout);
    this.searchTimeout = setTimeout(() => {
      this.currentPage.set(1);
      this.loadCreators();
    }, 300);
  }

  onFilterChange(): void {
    this.currentPage.set(1);
    this.loadCreators();
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages()) {
      this.currentPage.set(page);
      this.loadCreators();
    }
  }

  openCreateModal(): void {
    this.selectedCreatorForEdit.set(null);
    this.isModalOpen.set(true);
  }

  openEditModal(creatorId: string): void {
    this.creatorService.getCreatorById(creatorId).subscribe({
      next: res => {
        if (res.success && res.data) {
          this.selectedCreatorForEdit.set(res.data);
          this.isModalOpen.set(true);
        }
      },
      error: () => toast.error('Failed to fetch creator details')
    });
  }

  viewCreatorDetail(creatorId: string): void {
    this.creatorService.getCreatorById(creatorId).subscribe({
      next: res => {
        if (res.success && res.data) {
          this.selectedCreatorForDetail.set(res.data);
          this.isDetailModalOpen.set(true);
        }
      },
      error: () => toast.error('Failed to fetch creator profile')
    });
  }

  openAssignToRosterModal(creator: CreatorSummary | Creator): void {
    this.creatorToAssign.set(creator);
    this.isAssignModalOpen.set(true);
  }

  closeModal(): void {
    this.isModalOpen.set(false);
    this.selectedCreatorForEdit.set(null);
  }

  closeDetailModal(): void {
    this.isDetailModalOpen.set(false);
    this.selectedCreatorForDetail.set(null);
  }

  closeAssignModal(): void {
    this.isAssignModalOpen.set(false);
    this.creatorToAssign.set(null);
  }

  onEditFromDetail(creator: Creator): void {
    this.closeDetailModal();
    this.selectedCreatorForEdit.set(creator);
    this.isModalOpen.set(true);
  }

  onAssignFromDetail(creator: Creator): void {
    this.closeDetailModal();
    this.openAssignToRosterModal(creator);
  }

  onCreatorSaved(): void {
    this.loadCreators();
  }

  onRosterEnrolled(): void {
    toast.success('Creator assigned to roster');
  }

  confirmDelete(creator: CreatorSummary): void {
    this.creatorToDelete.set(creator);
  }

  executeDelete(): void {
    const target = this.creatorToDelete();
    if (!target) return;

    this.isDeleting.set(true);
    this.creatorService.deleteCreator(target.id).subscribe({
      next: () => {
        this.isDeleting.set(false);
        this.creatorToDelete.set(null);
        toast.success(`Creator "${target.fullName}" removed`);
        this.loadCreators();
      },
      error: err => {
        this.isDeleting.set(false);
        const msg = err.error?.detail || err.error?.message || 'Failed to delete creator';
        toast.error(msg);
      }
    });
  }

  formatFollowers(count: number): string {
    if (count >= 1_000_000) {
      return (count / 1_000_000).toFixed(1) + 'M';
    }
    if (count >= 1_000) {
      return (count / 1_000).toFixed(1) + 'K';
    }
    return count.toString();
  }
}
