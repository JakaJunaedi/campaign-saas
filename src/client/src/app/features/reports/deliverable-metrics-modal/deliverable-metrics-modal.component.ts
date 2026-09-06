import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  LucideAngularModule,
  BarChart3,
  X,
  Loader2,
  TrendingUp,
  Eye,
  Heart,
  MessageSquare,
  Share2,
  MousePointerClick,
  Sparkles
} from 'lucide-angular';
import { toast } from 'ngx-sonner';
import { ReportingService } from '../../../core/services/reporting.service';
import { Deliverable } from '../../../core/models/deliverable.model';
import { CampaignMetric, RecordDeliverableMetricsRequest } from '../../../core/models/reporting.model';

@Component({
  selector: 'app-deliverable-metrics-modal',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LucideAngularModule
  ],
  template: `
    @if (isOpen) {
      <div class="fixed inset-0 z-50 overflow-y-auto bg-slate-900/60 backdrop-blur-xs flex items-center justify-center p-4 animate-in fade-in duration-200">
        <div
          class="bg-white rounded-3xl shadow-2xl border border-slate-100 w-full max-w-xl overflow-hidden animate-in zoom-in-95 duration-150"
          (click)="$event.stopPropagation()"
        >
          <!-- Header -->
          <div class="px-6 py-5 border-b border-slate-100 flex items-center justify-between bg-slate-50/50">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-2xl bg-emerald-50 text-emerald-600 flex items-center justify-center">
                <lucide-icon [img]="BarChart3Icon" class="w-5 h-5"></lucide-icon>
              </div>
              <div>
                <h3 class="text-base font-bold text-slate-900">Record Deliverable Performance</h3>
                <p class="text-xs text-slate-500">
                  {{ deliverable?.title || 'Deliverable Metrics' }} • {{ deliverable?.platform }} ({{ deliverable?.contentType }})
                </p>
              </div>
            </div>

            <button
              type="button"
              (click)="close.emit()"
              class="p-2 rounded-xl text-slate-400 hover:text-slate-600 hover:bg-slate-100 transition"
            >
              <lucide-icon [img]="XIcon" class="w-4 h-4"></lucide-icon>
            </button>
          </div>

          <!-- Form Body -->
          <form (ngSubmit)="onSubmit()" class="p-6 space-y-5">
            <!-- Realtime Calculated KPI Preview Bar -->
            <div class="grid grid-cols-2 gap-3 p-4 rounded-2xl bg-gradient-to-tr from-emerald-900 to-teal-900 text-white shadow-inner">
              <div class="space-y-1">
                <span class="text-[10px] font-bold text-emerald-300 uppercase tracking-wider">Calculated Total Engagement</span>
                <p class="text-xl font-black text-white">
                  {{ computedTotalEngagement() | number }}
                </p>
                <p class="text-[10px] text-emerald-200">Likes + Comments + Shares + Clicks</p>
              </div>

              <div class="space-y-1">
                <span class="text-[10px] font-bold text-emerald-300 uppercase tracking-wider">Calculated Engagement Rate</span>
                <p class="text-xl font-black text-emerald-300">
                  {{ computedEngagementRate() | number:'1.2-2' }}%
                </p>
                <p class="text-[10px] text-emerald-200">Total Engagements / Reach</p>
              </div>
            </div>

            <!-- Reach & Impressions -->
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5 flex items-center gap-1.5">
                  <lucide-icon [img]="EyeIcon" class="w-3.5 h-3.5 text-slate-400"></lucide-icon>
                  <span>Reach *</span>
                </label>
                <input
                  type="number"
                  min="0"
                  [(ngModel)]="metricsForm.reach"
                  name="reach"
                  required
                  class="w-full px-3.5 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-xs font-semibold text-slate-800 focus:outline-hidden focus:ring-2 focus:ring-emerald-500/20 focus:border-emerald-600 transition"
                  placeholder="e.g. 50000"
                />
              </div>

              <div>
                <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5 flex items-center gap-1.5">
                  <lucide-icon [img]="EyeIcon" class="w-3.5 h-3.5 text-slate-400"></lucide-icon>
                  <span>Impressions *</span>
                </label>
                <input
                  type="number"
                  min="0"
                  [(ngModel)]="metricsForm.impressions"
                  name="impressions"
                  required
                  class="w-full px-3.5 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-xs font-semibold text-slate-800 focus:outline-hidden focus:ring-2 focus:ring-emerald-500/20 focus:border-emerald-600 transition"
                  placeholder="e.g. 75000"
                />
              </div>
            </div>

            <!-- Video Views & Clicks -->
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5 flex items-center gap-1.5">
                  <lucide-icon [img]="TrendingUpIcon" class="w-3.5 h-3.5 text-slate-400"></lucide-icon>
                  <span>Video / Content Views</span>
                </label>
                <input
                  type="number"
                  min="0"
                  [(ngModel)]="metricsForm.views"
                  name="views"
                  class="w-full px-3.5 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-xs font-semibold text-slate-800 focus:outline-hidden focus:ring-2 focus:ring-emerald-500/20 focus:border-emerald-600 transition"
                  placeholder="e.g. 42000"
                />
              </div>

              <div>
                <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5 flex items-center gap-1.5">
                  <lucide-icon [img]="MousePointerClickIcon" class="w-3.5 h-3.5 text-slate-400"></lucide-icon>
                  <span>Link Clicks</span>
                </label>
                <input
                  type="number"
                  min="0"
                  [(ngModel)]="metricsForm.clicks"
                  name="clicks"
                  class="w-full px-3.5 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-xs font-semibold text-slate-800 focus:outline-hidden focus:ring-2 focus:ring-emerald-500/20 focus:border-emerald-600 transition"
                  placeholder="e.g. 1500"
                />
              </div>
            </div>

            <!-- Social Engagements (Likes, Comments, Shares) -->
            <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
              <div>
                <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5 flex items-center gap-1.5">
                  <lucide-icon [img]="HeartIcon" class="w-3.5 h-3.5 text-rose-500"></lucide-icon>
                  <span>Likes</span>
                </label>
                <input
                  type="number"
                  min="0"
                  [(ngModel)]="metricsForm.likes"
                  name="likes"
                  class="w-full px-3.5 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-xs font-semibold text-slate-800 focus:outline-hidden focus:ring-2 focus:ring-emerald-500/20 focus:border-emerald-600 transition"
                  placeholder="e.g. 3200"
                />
              </div>

              <div>
                <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5 flex items-center gap-1.5">
                  <lucide-icon [img]="MessageSquareIcon" class="w-3.5 h-3.5 text-indigo-500"></lucide-icon>
                  <span>Comments</span>
                </label>
                <input
                  type="number"
                  min="0"
                  [(ngModel)]="metricsForm.comments"
                  name="comments"
                  class="w-full px-3.5 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-xs font-semibold text-slate-800 focus:outline-hidden focus:ring-2 focus:ring-emerald-500/20 focus:border-emerald-600 transition"
                  placeholder="e.g. 240"
                />
              </div>

              <div>
                <label class="block text-xs font-bold text-slate-700 uppercase tracking-wider mb-1.5 flex items-center gap-1.5">
                  <lucide-icon [img]="Share2Icon" class="w-3.5 h-3.5 text-cyan-500"></lucide-icon>
                  <span>Shares / Saves</span>
                </label>
                <input
                  type="number"
                  min="0"
                  [(ngModel)]="metricsForm.shares"
                  name="shares"
                  class="w-full px-3.5 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-xs font-semibold text-slate-800 focus:outline-hidden focus:ring-2 focus:ring-emerald-500/20 focus:border-emerald-600 transition"
                  placeholder="e.g. 580"
                />
              </div>
            </div>

            <!-- Footer Actions -->
            <div class="flex items-center justify-end gap-3 pt-4 border-t border-slate-100">
              <button
                type="button"
                (click)="close.emit()"
                [disabled]="isSaving()"
                class="px-4 py-2.5 rounded-xl border border-slate-200 text-slate-700 text-xs font-semibold hover:bg-slate-50 transition"
              >
                Cancel
              </button>

              <button
                type="submit"
                [disabled]="isSaving()"
                class="px-5 py-2.5 rounded-xl bg-emerald-600 hover:bg-emerald-500 disabled:bg-slate-200 disabled:text-slate-400 text-white text-xs font-semibold shadow-md shadow-emerald-600/20 transition flex items-center gap-2"
              >
                @if (isSaving()) {
                  <lucide-icon [img]="Loader2Icon" class="w-4 h-4 animate-spin"></lucide-icon>
                  <span>Saving Metrics...</span>
                } @else {
                  <lucide-icon [img]="BarChart3Icon" class="w-4 h-4"></lucide-icon>
                  <span>Save Performance Data</span>
                }
              </button>
            </div>
          </form>
        </div>
      </div>
    }
  `
})
export class DeliverableMetricsModalComponent implements OnChanges {
  private reportingService = inject(ReportingService);

  @Input() isOpen = false;
  @Input() deliverable: Deliverable | null = null;
  @Input() existingMetric: CampaignMetric | null = null;

  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<CampaignMetric>();

  readonly isSaving = signal<boolean>(false);

  metricsForm: RecordDeliverableMetricsRequest = {
    reach: 0,
    impressions: 0,
    views: 0,
    likes: 0,
    comments: 0,
    shares: 0,
    clicks: 0
  };

  // Icons
  readonly BarChart3Icon = BarChart3;
  readonly XIcon = X;
  readonly Loader2Icon = Loader2;
  readonly TrendingUpIcon = TrendingUp;
  readonly EyeIcon = Eye;
  readonly HeartIcon = Heart;
  readonly MessageSquareIcon = MessageSquare;
  readonly Share2Icon = Share2;
  readonly MousePointerClickIcon = MousePointerClick;
  readonly SparklesIcon = Sparkles;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['isOpen'] && this.isOpen) {
      if (this.existingMetric) {
        this.metricsForm = {
          reach: this.existingMetric.reach || 0,
          impressions: this.existingMetric.impressions || 0,
          views: this.existingMetric.views || 0,
          likes: this.existingMetric.likes || 0,
          comments: this.existingMetric.comments || 0,
          shares: this.existingMetric.shares || 0,
          clicks: this.existingMetric.clicks || 0
        };
      } else if (this.deliverable) {
        // Fetch latest metrics if any
        this.reportingService.getDeliverableMetrics(this.deliverable.id).subscribe({
          next: res => {
            if (res.success && res.data) {
              this.metricsForm = {
                reach: res.data.reach || 0,
                impressions: res.data.impressions || 0,
                views: res.data.views || 0,
                likes: res.data.likes || 0,
                comments: res.data.comments || 0,
                shares: res.data.shares || 0,
                clicks: res.data.clicks || 0
              };
            }
          }
        });
      }
    }
  }

  computedTotalEngagement(): number {
    return (
      (this.metricsForm.likes || 0) +
      (this.metricsForm.comments || 0) +
      (this.metricsForm.shares || 0) +
      (this.metricsForm.clicks || 0)
    );
  }

  computedEngagementRate(): number {
    const reach = this.metricsForm.reach || 0;
    if (reach <= 0) return 0;
    const total = this.computedTotalEngagement();
    return (total / reach) * 100;
  }

  onSubmit(): void {
    if (!this.deliverable) return;

    this.isSaving.set(true);
    this.reportingService
      .recordDeliverableMetrics(this.deliverable.id, this.metricsForm)
      .subscribe({
        next: res => {
          this.isSaving.set(false);
          if (res.success && res.data) {
            toast.success('Performance metrics recorded successfully!');
            this.saved.emit(res.data);
            this.close.emit();
          }
        },
        error: err => {
          this.isSaving.set(false);
          const msg = err.error?.detail || err.error?.message || 'Failed to record metrics';
          toast.error(msg);
        }
      });
  }
}
