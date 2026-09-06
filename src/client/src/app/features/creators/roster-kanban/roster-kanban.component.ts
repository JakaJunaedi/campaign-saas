import { Component, OnInit, inject, signal, effect, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  DragDropModule,
  CdkDragDrop,
  moveItemInArray,
  transferArrayItem
} from '@angular/cdk/drag-drop';
import {
  LucideAngularModule,
  FolderKanban,
  Users,
  Plus,
  Trash2,
  Edit2,
  DollarSign,
  Loader2,
  Sparkles,
  CheckCircle2,
  Clock,
  Send,
  FileCheck,
  Megaphone,
  RefreshCw,
  ExternalLink
} from 'lucide-angular';
import { toast } from 'ngx-sonner';
import { CampaignService } from '../../../core/services/campaign.service';
import { CreatorService } from '../../../core/services/creator.service';
import { CampaignCreator, CampaignSummary } from '../../../core/models/campaign.model';
import { CreatorSummary } from '../../../core/models/creator.model';
import { AddToRosterModalComponent } from '../add-to-roster-modal/add-to-roster-modal.component';

export interface KanbanColumn {
  id: string;
  title: string;
  status: string;
  color: string;
  badgeClass: string;
  items: EnrichedRosterItem[];
}

export interface EnrichedRosterItem extends CampaignCreator {
  creatorName?: string;
  creatorNiche?: string;
  creatorFollowers?: number;
}

@Component({
  selector: 'app-roster-kanban',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DragDropModule,
    LucideAngularModule,
    AddToRosterModalComponent
  ],
  template: `
    <div class="space-y-6">
      <!-- Toolbar: Campaign Selector & Quick Actions -->
      <div class="p-4 bg-white rounded-2xl border border-slate-200/80 shadow-xs flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div class="flex flex-col sm:flex-row sm:items-center gap-3 flex-1">
          <div class="flex items-center gap-2 shrink-0">
            <div class="w-9 h-9 rounded-xl bg-violet-50 text-violet-600 flex items-center justify-center font-bold">
              <lucide-icon [img]="FolderKanbanIcon" class="w-4 h-4"></lucide-icon>
            </div>
            <span class="text-xs font-bold text-slate-700">Active Campaign:</span>
          </div>

          <select
            [(ngModel)]="selectedCampaignId"
            (ngModelChange)="onCampaignChange()"
            class="px-3.5 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs font-semibold text-slate-800 focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition max-w-md"
          >
            <option value="" disabled selected>Select campaign to view roster...</option>
            @for (camp of campaigns(); track camp.id) {
              <option [value]="camp.id">{{ camp.title }} ({{ camp.status }})</option>
            }
          </select>
        </div>

        <div class="flex items-center gap-2 shrink-0">
          <button
            type="button"
            (click)="loadRosterData()"
            [disabled]="!selectedCampaignId"
            class="p-2 rounded-xl border border-slate-200 text-slate-600 hover:bg-slate-50 disabled:opacity-40 transition"
            title="Refresh Board"
          >
            <lucide-icon [img]="RefreshCwIcon" class="w-4 h-4" [class.animate-spin]="isLoading()"></lucide-icon>
          </button>

          <button
            type="button"
            (click)="isAddRosterModalOpen.set(true)"
            [disabled]="!selectedCampaignId"
            class="px-4 py-2 rounded-xl bg-indigo-600 hover:bg-indigo-500 disabled:bg-slate-200 disabled:text-slate-400 text-white text-xs font-semibold shadow-xs transition flex items-center gap-2"
          >
            <lucide-icon [img]="PlusIcon" class="w-3.5 h-3.5"></lucide-icon>
            <span>Assign Creator</span>
          </button>
        </div>
      </div>

      <!-- Kanban Board Area -->
      @if (isLoading()) {
        <div class="p-16 bg-white rounded-2xl border border-slate-200/80 shadow-xs text-center space-y-3">
          <lucide-icon [img]="Loader2Icon" class="w-8 h-8 text-indigo-600 animate-spin mx-auto"></lucide-icon>
          <p class="text-xs text-slate-500 font-medium">Loading roster board...</p>
        </div>
      } @else if (!selectedCampaignId) {
        <div class="p-16 bg-white rounded-2xl border border-slate-200/80 shadow-xs text-center space-y-3 max-w-md mx-auto">
          <div class="w-12 h-12 rounded-2xl bg-violet-50 text-violet-600 flex items-center justify-center mx-auto">
            <lucide-icon [img]="FolderKanbanIcon" class="w-6 h-6"></lucide-icon>
          </div>
          <h3 class="text-sm font-bold text-slate-900">Select a Campaign</h3>
          <p class="text-xs text-slate-500">
            Please choose an active campaign from the dropdown above to manage its influencer roster stages.
          </p>
        </div>
      } @else {
        <!-- Drag & Drop Columns Grid -->
        <div
          cdkDropListGroup
          class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-4 overflow-x-auto pb-4"
        >
          @for (col of columns(); track col.id) {
            <div class="bg-slate-50/80 rounded-3xl border border-slate-200/70 p-3.5 flex flex-col min-h-[500px]">
              <!-- Column Header -->
              <div class="flex items-center justify-between pb-3 mb-3 border-b border-slate-200/60 px-1">
                <div class="flex items-center gap-2">
                  <span class="w-2.5 h-2.5 rounded-full" [ngClass]="col.color"></span>
                  <h4 class="text-xs font-bold text-slate-800">{{ col.title }}</h4>
                </div>
                <span class="px-2 py-0.5 rounded-full bg-white border border-slate-200 text-[10px] font-bold text-slate-600 shadow-2xs">
                  {{ col.items.length }}
                </span>
              </div>

              <!-- Droppable Card Container -->
              <div
                cdkDropList
                [id]="col.status"
                [cdkDropListData]="col.items"
                (cdkDropListDropped)="onCardDrop($event, col.status)"
                class="flex-1 space-y-3 min-h-[400px] transition-colors rounded-2xl p-1"
                [class.bg-indigo-50/40]="col.items.length === 0"
              >
                @if (col.items.length === 0) {
                  <div class="h-24 border-2 border-dashed border-slate-200 rounded-2xl flex items-center justify-center text-[11px] text-slate-400 font-medium">
                    Drag card here
                  </div>
                }

                @for (item of col.items; track item.id) {
                  <div
                    cdkDrag
                    class="bg-white rounded-2xl p-4 border border-slate-200/80 shadow-xs hover:shadow-md transition cursor-grab active:cursor-grabbing space-y-3 group"
                  >
                    <!-- Card Top -->
                    <div class="flex items-start justify-between gap-2">
                      <div class="flex items-center gap-2.5">
                        <div class="w-8 h-8 rounded-xl bg-indigo-50 text-indigo-700 font-black text-xs flex items-center justify-center shrink-0 border border-indigo-100">
                          {{ (item.creatorName || 'K').charAt(0).toUpperCase() }}
                        </div>
                        <div class="truncate max-w-[130px]">
                          <p class="text-xs font-bold text-slate-900 truncate">
                            {{ item.creatorName || 'Creator #' + item.creatorId.substring(0, 6) }}
                          </p>
                          @if (item.creatorNiche) {
                            <span class="text-[10px] text-indigo-600 font-semibold block truncate">
                              {{ item.creatorNiche }}
                            </span>
                          }
                        </div>
                      </div>

                      <button
                        type="button"
                        (click)="removeCreator(item.creatorId)"
                        class="opacity-0 group-hover:opacity-100 text-slate-400 hover:text-rose-600 p-1 rounded-lg transition"
                        title="Remove from roster"
                      >
                        <lucide-icon [img]="Trash2Icon" class="w-3.5 h-3.5"></lucide-icon>
                      </button>
                    </div>

                    <!-- Card Fee & Metrics -->
                    <div class="pt-2 border-t border-slate-100 flex items-center justify-between text-xs">
                      <span class="text-[10px] text-slate-400 font-medium">Agreed Fee</span>
                      <span class="font-extrabold text-emerald-600 text-xs">
                        {{ formatCurrency(item.agreedRate) }}
                      </span>
                    </div>
                  </div>
                }
              </div>
            </div>
          }
        </div>
      }

      <!-- Add Creator to Roster Modal -->
      <app-add-to-roster-modal
        [isOpen]="isAddRosterModalOpen()"
        [preselectedCampaignId]="selectedCampaignId"
        (close)="isAddRosterModalOpen.set(false)"
        (enrolled)="onCreatorEnrolled()"
      ></app-add-to-roster-modal>
    </div>
  `
})
export class RosterKanbanComponent implements OnInit {
  @Input() initialCampaignId?: string;

  private campaignService = inject(CampaignService);
  private creatorService = inject(CreatorService);

  readonly campaigns = signal<CampaignSummary[]>([]);
  readonly creatorsMap = signal<Map<string, CreatorSummary>>(new Map());
  readonly isLoading = signal<boolean>(false);
  readonly isAddRosterModalOpen = signal<boolean>(false);

  selectedCampaignId = '';

  // Kanban Columns
  readonly columns = signal<KanbanColumn[]>([
    {
      id: 'col-invited',
      title: 'Invited / Outreach',
      status: 'Invited',
      color: 'bg-slate-400',
      badgeClass: 'bg-slate-100 text-slate-700',
      items: []
    },
    {
      id: 'col-confirmed',
      title: 'Confirmed',
      status: 'Confirmed',
      color: 'bg-blue-500',
      badgeClass: 'bg-blue-50 text-blue-700',
      items: []
    },
    {
      id: 'col-draft',
      title: 'Draft Submitted',
      status: 'DraftSubmitted',
      color: 'bg-amber-500',
      badgeClass: 'bg-amber-50 text-amber-700',
      items: []
    },
    {
      id: 'col-approved',
      title: 'Approved',
      status: 'Approved',
      color: 'bg-indigo-500',
      badgeClass: 'bg-indigo-50 text-indigo-700',
      items: []
    },
    {
      id: 'col-completed',
      title: 'Live & Done',
      status: 'Completed',
      color: 'bg-emerald-500',
      badgeClass: 'bg-emerald-50 text-emerald-700',
      items: []
    }
  ]);

  // Icons
  readonly FolderKanbanIcon = FolderKanban;
  readonly UsersIcon = Users;
  readonly PlusIcon = Plus;
  readonly Trash2Icon = Trash2;
  readonly Edit2Icon = Edit2;
  readonly DollarSignIcon = DollarSign;
  readonly Loader2Icon = Loader2;
  readonly SparklesIcon = Sparkles;
  readonly CheckCircle2Icon = CheckCircle2;
  readonly ClockIcon = Clock;
  readonly SendIcon = Send;
  readonly FileCheckIcon = FileCheck;
  readonly MegaphoneIcon = Megaphone;
  readonly RefreshCwIcon = RefreshCw;
  readonly ExternalLinkIcon = ExternalLink;

  ngOnInit(): void {
    this.loadCampaignsList();
    this.loadCreatorsCache();
  }

  private loadCampaignsList(): void {
    this.campaignService.getCampaigns('', undefined, undefined, 1, 100).subscribe({
      next: res => {
        if (res.success && res.data) {
          this.campaigns.set(res.data.items);
          if (this.initialCampaignId) {
            this.selectedCampaignId = this.initialCampaignId;
            this.loadRosterData();
          } else if (res.data.items.length > 0) {
            this.selectedCampaignId = res.data.items[0].id;
            this.loadRosterData();
          }
        }
      }
    });
  }

  private loadCreatorsCache(): void {
    this.creatorService.getCreators('', undefined, undefined, undefined, 1, 100).subscribe({
      next: res => {
        if (res.success && res.data) {
          const map = new Map<string, CreatorSummary>();
          for (const c of res.data.items) {
            map.set(c.id, c);
          }
          this.creatorsMap.set(map);
          this.reEnrichColumns();
        }
      }
    });
  }

  onCampaignChange(): void {
    this.loadRosterData();
  }

  loadRosterData(): void {
    if (!this.selectedCampaignId) return;

    this.isLoading.set(true);
    this.campaignService.getCampaignRoster(this.selectedCampaignId).subscribe({
      next: res => {
        this.isLoading.set(false);
        if (res.success && res.data) {
          this.distributeIntoColumns(res.data);
        }
      },
      error: () => {
        this.isLoading.set(false);
        toast.error('Failed to load campaign roster');
      }
    });
  }

  private distributeIntoColumns(items: CampaignCreator[]): void {
    const map = this.creatorsMap();

    const newCols = this.columns().map(col => {
      const filtered = items
        .filter(item => {
          if (col.status === 'Completed') {
            return item.status === 'Completed' || item.status === 'Published';
          }
          return item.status === col.status;
        })
        .map(item => {
          const creator = map.get(item.creatorId);
          return {
            ...item,
            creatorName: creator?.fullName,
            creatorNiche: creator?.niche,
            creatorFollowers: creator?.totalFollowers
          } as EnrichedRosterItem;
        });

      return {
        ...col,
        items: filtered
      };
    });

    this.columns.set(newCols);
  }

  private reEnrichColumns(): void {
    const map = this.creatorsMap();
    const currentCols = this.columns().map(col => ({
      ...col,
      items: col.items.map(item => {
        const creator = map.get(item.creatorId);
        return {
          ...item,
          creatorName: creator?.fullName || item.creatorName,
          creatorNiche: creator?.niche || item.creatorNiche,
          creatorFollowers: creator?.totalFollowers || item.creatorFollowers
        };
      })
    }));
    this.columns.set(currentCols);
  }

  onCardDrop(event: CdkDragDrop<EnrichedRosterItem[]>, newStatus: string): void {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      const item = event.previousContainer.data[event.previousIndex];
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex
      );

      // Backend API call to update status
      this.campaignService
        .updateRosterStatus(this.selectedCampaignId, item.creatorId, { status: newStatus })
        .subscribe({
          next: () => {
            item.status = newStatus;
            toast.success(`Roster stage updated to ${newStatus}`);
          },
          error: err => {
            const msg = err.error?.detail || err.error?.message || 'Failed to update roster stage';
            toast.error(msg);
            this.loadRosterData(); // Revert
          }
        });
    }
  }

  removeCreator(creatorId: string): void {
    if (!this.selectedCampaignId) return;

    this.campaignService.removeCreatorFromRoster(this.selectedCampaignId, creatorId).subscribe({
      next: () => {
        toast.success('Creator removed from roster');
        this.loadRosterData();
      },
      error: err => {
        const msg = err.error?.detail || err.error?.message || 'Failed to remove creator';
        toast.error(msg);
      }
    });
  }

  onCreatorEnrolled(): void {
    this.loadRosterData();
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('id-ID', {
      style: 'currency',
      currency: 'IDR',
      maximumFractionDigits: 0
    }).format(amount);
  }
}
