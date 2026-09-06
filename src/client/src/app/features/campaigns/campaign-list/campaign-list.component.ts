import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import {
  LucideAngularModule,
  Megaphone,
  Search,
  Plus,
  Edit,
  Trash2,
  Eye,
  Users,
  Calendar,
  DollarSign,
  LayoutGrid,
  List,
  ChevronLeft,
  ChevronRight,
  AlertCircle,
  Loader2,
  RefreshCw,
  CheckCircle2,
  Clock,
  Ban,
  ArrowUpRight
} from 'lucide-angular';
import { toast } from 'ngx-sonner';
import { CampaignService } from '../../../core/services/campaign.service';
import { ClientService } from '../../../core/services/client.service';
import { Campaign, CampaignSummary } from '../../../core/models/campaign.model';
import { ClientSummary } from '../../../core/models/client.model';
import { CampaignModalComponent } from '../campaign-modal/campaign-modal.component';

@Component({
  selector: 'app-campaign-list',
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
      <!-- Page Header Banner -->
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <div class="flex items-center gap-2">
            <h1 class="text-2xl font-black tracking-tight text-slate-900">Campaign Operations</h1>
            <span class="px-2.5 py-0.5 rounded-full bg-indigo-50 text-indigo-700 text-xs font-bold">
              {{ totalCount() }} Total
            </span>
          </div>
          <p class="text-xs text-slate-500 mt-1">
            Manage briefs, creator rosters, deliverables, and budgets across active marketing campaigns.
          </p>
        </div>

        <div class="flex items-center gap-3">
          <button
            type="button"
            (click)="loadCampaigns()"
            class="p-2.5 rounded-xl border border-slate-200 text-slate-600 hover:bg-slate-50 hover:text-slate-900 transition"
            title="Refresh list"
          >
            <lucide-icon [img]="RefreshCwIcon" class="w-4 h-4" [class.animate-spin]="isLoading()"></lucide-icon>
          </button>

          <button
            type="button"
            (click)="openCreateModal()"
            class="px-4 py-2.5 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white text-xs font-semibold shadow-md shadow-indigo-600/20 transition flex items-center gap-2"
          >
            <lucide-icon [img]="PlusIcon" class="w-4 h-4"></lucide-icon>
            <span>Create Campaign</span>
          </button>
        </div>
      </div>

      <!-- Filter & Search Toolbar -->
      <div class="p-4 bg-white rounded-2xl border border-slate-200/80 shadow-xs space-y-4">
        <!-- Status Tabs Bar -->
        <div class="flex items-center justify-between gap-2 overflow-x-auto pb-1 border-b border-slate-100">
          <div class="flex items-center gap-1.5 shrink-0">
            @for (tab of statusTabs; track tab.value) {
              <button
                type="button"
                (click)="onStatusTabChange(tab.value)"
                class="px-3.5 py-1.5 rounded-xl text-xs font-bold transition flex items-center gap-1.5"
                [class]="selectedStatus() === tab.value
                  ? 'bg-indigo-600 text-white shadow-xs'
                  : 'text-slate-600 hover:bg-slate-100'"
              >
                <span>{{ tab.label }}</span>
              </button>
            }
          </div>

          <!-- Grid vs Table View Switch -->
          <div class="flex items-center gap-1 bg-slate-100 p-1 rounded-xl shrink-0">
            <button
              type="button"
              (click)="viewMode.set('grid')"
              class="p-1.5 rounded-lg transition"
              [class]="viewMode() === 'grid' ? 'bg-white text-indigo-600 shadow-xs' : 'text-slate-500 hover:text-slate-800'"
              title="Grid View"
            >
              <lucide-icon [img]="LayoutGridIcon" class="w-3.5 h-3.5"></lucide-icon>
            </button>
            <button
              type="button"
              (click)="viewMode.set('table')"
              class="p-1.5 rounded-lg transition"
              [class]="viewMode() === 'table' ? 'bg-white text-indigo-600 shadow-xs' : 'text-slate-500 hover:text-slate-800'"
              title="Table View"
            >
              <lucide-icon [img]="ListIcon" class="w-3.5 h-3.5"></lucide-icon>
            </button>
          </div>
        </div>

        <!-- Search & Client Dropdown Row -->
        <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
          <div class="flex flex-col sm:flex-row items-stretch sm:items-center gap-3 flex-1 max-w-2xl">
            <div class="relative flex-1">
              <lucide-icon
                [img]="SearchIcon"
                class="w-4 h-4 absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-400"
              ></lucide-icon>
              <input
                type="text"
                [(ngModel)]="searchQuery"
                (ngModelChange)="onSearchChange()"
                placeholder="Search campaigns by title..."
                class="w-full pl-9.5 pr-4 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition"
              />
            </div>

            <select
              [(ngModel)]="selectedClientId"
              (ngModelChange)="onClientFilterChange()"
              class="px-3.5 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs text-slate-700 focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition"
            >
              <option value="">All Brand Clients</option>
              @for (c of clientOptions(); track c.id) {
                <option [value]="c.id">{{ c.name }}</option>
              }
            </select>
          </div>

          <div class="text-xs text-slate-500">
            Page {{ currentPage() }} of {{ totalPages() || 1 }}
          </div>
        </div>
      </div>

      <!-- Content Area -->
      @if (isLoading()) {
        <div class="p-16 bg-white rounded-2xl border border-slate-200/80 shadow-xs text-center space-y-3">
          <lucide-icon [img]="Loader2Icon" class="w-8 h-8 text-indigo-600 animate-spin mx-auto"></lucide-icon>
          <p class="text-xs text-slate-500 font-medium">Loading campaign records...</p>
        </div>
      } @else if (campaigns().length === 0) {
        <div class="p-16 bg-white rounded-2xl border border-slate-200/80 shadow-xs text-center space-y-3 max-w-md mx-auto">
          <div class="w-12 h-12 rounded-2xl bg-indigo-50 text-indigo-600 flex items-center justify-center mx-auto">
            <lucide-icon [img]="MegaphoneIcon" class="w-6 h-6"></lucide-icon>
          </div>
          <h3 class="text-sm font-bold text-slate-900">No Campaigns Found</h3>
          <p class="text-xs text-slate-500 leading-relaxed">
            {{ searchQuery || selectedStatus() !== 'ALL' || selectedClientId ? 'No campaigns match the selected filters.' : 'Create your first campaign to begin assigning creators and managing deliverables.' }}
          </p>
          <button
            type="button"
            (click)="openCreateModal()"
            class="mt-2 inline-flex items-center gap-1.5 px-4 py-2 rounded-xl bg-indigo-600 text-white text-xs font-semibold hover:bg-indigo-500 transition"
          >
            <lucide-icon [img]="PlusIcon" class="w-3.5 h-3.5"></lucide-icon>
            <span>Create Campaign</span>
          </button>
        </div>
      } @else {
        <!-- Grid View -->
        @if (viewMode() === 'grid') {
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-5">
            @for (campaign of campaigns(); track campaign.id) {
              <div class="bg-white rounded-3xl border border-slate-200/80 shadow-xs hover:shadow-md transition p-5 flex flex-col justify-between space-y-4 group">
                <!-- Card Header -->
                <div class="space-y-2.5">
                  <div class="flex items-center justify-between gap-2">
                    <span
                      class="px-2.5 py-1 rounded-full text-[11px] font-bold uppercase tracking-wider flex items-center gap-1.5"
                      [ngClass]="getStatusBadgeClass(campaign.status)"
                    >
                      <span class="w-1.5 h-1.5 rounded-full" [ngClass]="getStatusDotClass(campaign.status)"></span>
                      <span>{{ campaign.status }}</span>
                    </span>

                    <div class="flex items-center gap-1 opacity-80 group-hover:opacity-100 transition">
                      <button
                        type="button"
                        (click)="openEditModal(campaign.id)"
                        class="p-1.5 rounded-lg text-slate-400 hover:text-indigo-600 hover:bg-slate-100 transition"
                        title="Edit Campaign"
                      >
                        <lucide-icon [img]="EditIcon" class="w-3.5 h-3.5"></lucide-icon>
                      </button>
                      <button
                        type="button"
                        (click)="confirmDelete(campaign)"
                        class="p-1.5 rounded-lg text-slate-400 hover:text-rose-600 hover:bg-rose-50 transition"
                        title="Delete Campaign"
                      >
                        <lucide-icon [img]="Trash2Icon" class="w-3.5 h-3.5"></lucide-icon>
                      </button>
                    </div>
                  </div>

                  <!-- Title & Link -->
                  <a
                    [routerLink]="['/campaigns', campaign.id]"
                    class="block text-base font-extrabold text-slate-900 group-hover:text-indigo-600 transition line-clamp-2 leading-snug"
                  >
                    {{ campaign.title }}
                  </a>
                </div>

                <!-- Card Metrics -->
                <div class="space-y-3 pt-3 border-t border-slate-100">
                  <div class="flex items-center justify-between text-xs">
                    <span class="text-slate-400 font-medium">Budget</span>
                    <span class="font-extrabold text-slate-900">
                      {{ formatCurrency(campaign.budget) }}
                    </span>
                  </div>

                  <div class="flex items-center justify-between text-xs">
                    <span class="text-slate-400 font-medium">Schedule</span>
                    <span class="font-semibold text-slate-700 flex items-center gap-1">
                      <lucide-icon [img]="CalendarIcon" class="w-3 h-3 text-slate-400"></lucide-icon>
                      <span>{{ campaign.startDate | date:'MMM d' }} - {{ campaign.endDate | date:'MMM d, yyyy' }}</span>
                    </span>
                  </div>

                  <div class="flex items-center justify-between text-xs">
                    <span class="text-slate-400 font-medium">Creators Roster</span>
                    <span class="inline-flex items-center gap-1 px-2 py-0.5 rounded-lg bg-indigo-50 text-indigo-700 font-bold text-[11px]">
                      <lucide-icon [img]="UsersIcon" class="w-3 h-3"></lucide-icon>
                      <span>{{ campaign.creatorsCount }} {{ campaign.creatorsCount === 1 ? 'creator' : 'creators' }}</span>
                    </span>
                  </div>
                </div>

                <!-- Card Footer Action -->
                <div class="pt-2 border-t border-slate-100 flex items-center justify-between gap-2">
                  <!-- Quick Status Selector -->
                  <select
                    [ngModel]="campaign.status"
                    (ngModelChange)="onQuickStatusChange(campaign.id, $event)"
                    class="text-[11px] font-semibold bg-slate-50 border border-slate-200 rounded-lg px-2 py-1 text-slate-600 focus:outline-hidden focus:border-indigo-600"
                  >
                    <option value="Draft">Draft</option>
                    <option value="Active">Active</option>
                    <option value="Completed">Completed</option>
                    <option value="Cancelled">Cancelled</option>
                  </select>

                  <a
                    [routerLink]="['/campaigns', campaign.id]"
                    class="inline-flex items-center gap-1 text-xs font-bold text-indigo-600 hover:text-indigo-700 transition"
                  >
                    <span>Manage</span>
                    <lucide-icon [img]="ArrowUpRightIcon" class="w-3.5 h-3.5"></lucide-icon>
                  </a>
                </div>
              </div>
            }
          </div>
        } @else {
          <!-- Table View -->
          <div class="bg-white rounded-2xl border border-slate-200/80 shadow-xs overflow-hidden">
            <div class="overflow-x-auto">
              <table class="w-full text-left border-collapse">
                <thead>
                  <tr class="border-b border-slate-100 bg-slate-50/50 text-[11px] font-bold text-slate-500 uppercase tracking-wider">
                    <th class="py-3.5 px-6">Campaign Title</th>
                    <th class="py-3.5 px-6">Status</th>
                    <th class="py-3.5 px-6">Budget</th>
                    <th class="py-3.5 px-6">Timeline</th>
                    <th class="py-3.5 px-6">Creators</th>
                    <th class="py-3.5 px-6 text-right">Actions</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-100 text-xs">
                  @for (campaign of campaigns(); track campaign.id) {
                    <tr class="hover:bg-slate-50/75 transition group">
                      <td class="py-4 px-6">
                        <a
                          [routerLink]="['/campaigns', campaign.id]"
                          class="font-bold text-slate-900 group-hover:text-indigo-600 transition"
                        >
                          {{ campaign.title }}
                        </a>
                      </td>

                      <td class="py-4 px-6">
                        <span
                          class="px-2.5 py-1 rounded-full text-[11px] font-bold uppercase tracking-wider inline-flex items-center gap-1.5"
                          [ngClass]="getStatusBadgeClass(campaign.status)"
                        >
                          <span class="w-1.5 h-1.5 rounded-full" [ngClass]="getStatusDotClass(campaign.status)"></span>
                          <span>{{ campaign.status }}</span>
                        </span>
                      </td>

                      <td class="py-4 px-6 font-bold text-slate-900">
                        {{ formatCurrency(campaign.budget) }}
                      </td>

                      <td class="py-4 px-6 text-slate-600">
                        {{ campaign.startDate | date:'MMM d' }} - {{ campaign.endDate | date:'MMM d, yyyy' }}
                      </td>

                      <td class="py-4 px-6">
                        <span class="inline-flex items-center gap-1 px-2.5 py-1 rounded-full bg-slate-100 text-slate-700 font-semibold text-[11px]">
                          <lucide-icon [img]="UsersIcon" class="w-3 h-3 text-slate-500"></lucide-icon>
                          <span>{{ campaign.creatorsCount }}</span>
                        </span>
                      </td>

                      <td class="py-4 px-6 text-right">
                        <div class="flex items-center justify-end gap-1.5">
                          <a
                            [routerLink]="['/campaigns', campaign.id]"
                            class="p-2 rounded-lg text-slate-400 hover:text-indigo-600 hover:bg-indigo-50 transition"
                            title="View Details"
                          >
                            <lucide-icon [img]="EyeIcon" class="w-4 h-4"></lucide-icon>
                          </a>

                          <button
                            type="button"
                            (click)="openEditModal(campaign.id)"
                            class="p-2 rounded-lg text-slate-400 hover:text-indigo-600 hover:bg-indigo-50 transition"
                            title="Edit Campaign"
                          >
                            <lucide-icon [img]="EditIcon" class="w-4 h-4"></lucide-icon>
                          </button>

                          <button
                            type="button"
                            (click)="confirmDelete(campaign)"
                            class="p-2 rounded-lg text-slate-400 hover:text-rose-600 hover:bg-rose-50 transition"
                            title="Delete Campaign"
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
          </div>
        }

        <!-- Pagination Bar -->
        <div class="p-4 bg-white rounded-2xl border border-slate-200/80 shadow-xs flex items-center justify-between">
          <span class="text-xs text-slate-500">
            Showing {{ (currentPage() - 1) * pageSize + 1 }} to {{ Math.min(currentPage() * pageSize, totalCount()) }} of {{ totalCount() }} campaigns
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

      <!-- Delete Confirmation Modal -->
      @if (campaignToDelete()) {
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
              Are you sure you want to delete <span class="font-bold text-slate-900">"{{ campaignToDelete()?.title }}"</span>?
              All deliverable and roster bindings under this campaign will be removed.
            </p>

            <div class="flex items-center justify-end gap-3 pt-2">
              <button
                type="button"
                (click)="campaignToDelete.set(null)"
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

      <!-- Create & Edit Modal -->
      <app-campaign-modal
        [isOpen]="isModalOpen()"
        [campaign]="selectedCampaignForEdit()"
        (close)="closeModal()"
        (saved)="onCampaignSaved()"
      ></app-campaign-modal>
    </div>
  `
})
export class CampaignListComponent implements OnInit {
  private campaignService = inject(CampaignService);
  private clientService = inject(ClientService);

  readonly Math = Math;

  // View state signals
  readonly viewMode = signal<'grid' | 'table'>('grid');
  readonly campaigns = signal<CampaignSummary[]>([]);
  readonly clientOptions = signal<ClientSummary[]>([]);
  readonly totalCount = signal<number>(0);
  readonly totalPages = signal<number>(1);
  readonly currentPage = signal<number>(1);
  readonly pageSize = 9;
  readonly isLoading = signal<boolean>(false);
  readonly isDeleting = signal<boolean>(false);

  // Filters
  searchQuery = '';
  selectedStatus = signal<string>('ALL');
  selectedClientId = '';
  private searchTimeout: any;

  // Modal State Signals
  readonly isModalOpen = signal<boolean>(false);
  readonly selectedCampaignForEdit = signal<Campaign | null>(null);
  readonly campaignToDelete = signal<CampaignSummary | null>(null);

  // Status Tabs
  readonly statusTabs = [
    { label: 'All Campaigns', value: 'ALL' },
    { label: 'Active', value: 'Active' },
    { label: 'Draft', value: 'Draft' },
    { label: 'Completed', value: 'Completed' },
    { label: 'Cancelled', value: 'Cancelled' }
  ];

  // Icons
  readonly MegaphoneIcon = Megaphone;
  readonly SearchIcon = Search;
  readonly PlusIcon = Plus;
  readonly EditIcon = Edit;
  readonly Trash2Icon = Trash2;
  readonly EyeIcon = Eye;
  readonly UsersIcon = Users;
  readonly CalendarIcon = Calendar;
  readonly DollarSignIcon = DollarSign;
  readonly LayoutGridIcon = LayoutGrid;
  readonly ListIcon = List;
  readonly ChevronLeftIcon = ChevronLeft;
  readonly ChevronRightIcon = ChevronRight;
  readonly AlertCircleIcon = AlertCircle;
  readonly Loader2Icon = Loader2;
  readonly RefreshCwIcon = RefreshCw;
  readonly CheckCircle2Icon = CheckCircle2;
  readonly ClockIcon = Clock;
  readonly BanIcon = Ban;
  readonly ArrowUpRightIcon = ArrowUpRight;

  ngOnInit(): void {
    this.loadClients();
    this.loadCampaigns();
  }

  loadClients(): void {
    this.clientService.getClients('', 1, 100).subscribe({
      next: res => {
        if (res.success && res.data) {
          this.clientOptions.set(res.data.items);
        }
      }
    });
  }

  loadCampaigns(): void {
    this.isLoading.set(true);
    this.campaignService
      .getCampaigns(
        this.searchQuery,
        this.selectedClientId || undefined,
        this.selectedStatus() !== 'ALL' ? this.selectedStatus() : undefined,
        this.currentPage(),
        this.pageSize
      )
      .subscribe({
        next: res => {
          this.isLoading.set(false);
          if (res.success && res.data) {
            this.campaigns.set(res.data.items);
            this.totalCount.set(res.data.totalCount);
            this.totalPages.set(res.data.totalPages);
          }
        },
        error: () => {
          this.isLoading.set(false);
          toast.error('Failed to load campaigns');
        }
      });
  }

  onSearchChange(): void {
    clearTimeout(this.searchTimeout);
    this.searchTimeout = setTimeout(() => {
      this.currentPage.set(1);
      this.loadCampaigns();
    }, 300);
  }

  onStatusTabChange(status: string): void {
    this.selectedStatus.set(status);
    this.currentPage.set(1);
    this.loadCampaigns();
  }

  onClientFilterChange(): void {
    this.currentPage.set(1);
    this.loadCampaigns();
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages()) {
      this.currentPage.set(page);
      this.loadCampaigns();
    }
  }

  openCreateModal(): void {
    this.selectedCampaignForEdit.set(null);
    this.isModalOpen.set(true);
  }

  openEditModal(campaignId: string): void {
    this.campaignService.getCampaignById(campaignId).subscribe({
      next: res => {
        if (res.success && res.data) {
          this.selectedCampaignForEdit.set(res.data);
          this.isModalOpen.set(true);
        }
      },
      error: () => toast.error('Failed to fetch campaign details')
    });
  }

  closeModal(): void {
    this.isModalOpen.set(false);
    this.selectedCampaignForEdit.set(null);
  }

  onCampaignSaved(): void {
    this.loadCampaigns();
  }

  onQuickStatusChange(campaignId: string, newStatus: string): void {
    this.campaignService.updateCampaignStatus(campaignId, newStatus).subscribe({
      next: () => {
        toast.success(`Campaign status updated to ${newStatus}`);
        this.loadCampaigns();
      },
      error: err => {
        const msg = err.error?.detail || err.error?.message || 'Failed to update status';
        toast.error(msg);
      }
    });
  }

  confirmDelete(campaign: CampaignSummary): void {
    this.campaignToDelete.set(campaign);
  }

  executeDelete(): void {
    const target = this.campaignToDelete();
    if (!target) return;

    this.isDeleting.set(true);
    this.campaignService.deleteCampaign(target.id).subscribe({
      next: () => {
        this.isDeleting.set(false);
        this.campaignToDelete.set(null);
        toast.success(`Campaign "${target.title}" deleted successfully`);
        this.loadCampaigns();
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
        return 'bg-emerald-50 text-emerald-700 border border-emerald-200/60';
      case 'draft':
        return 'bg-slate-100 text-slate-700 border border-slate-200/60';
      case 'completed':
        return 'bg-indigo-50 text-indigo-700 border border-indigo-200/60';
      case 'cancelled':
        return 'bg-rose-50 text-rose-700 border border-rose-200/60';
      default:
        return 'bg-slate-100 text-slate-700 border border-slate-200/60';
    }
  }

  getStatusDotClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'active':
        return 'bg-emerald-500';
      case 'draft':
        return 'bg-slate-400';
      case 'completed':
        return 'bg-indigo-500';
      case 'cancelled':
        return 'bg-rose-500';
      default:
        return 'bg-slate-400';
    }
  }
}
