import { Component, OnInit, OnDestroy, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  LucideAngularModule,
  BarChart3,
  FileText,
  Download,
  Plus,
  RefreshCw,
  Loader2,
  TrendingUp,
  Eye,
  Heart,
  Share2,
  CheckCircle2,
  AlertCircle,
  Clock,
  ExternalLink,
  Coins,
  Sparkles,
  Calendar,
  Layers,
  Users
} from 'lucide-angular';
import { toast } from 'ngx-sonner';
import { ReportingService } from '../../core/services/reporting.service';
import { CampaignService } from '../../core/services/campaign.service';
import { DeliverableService } from '../../core/services/deliverable.service';
import { CreatorService } from '../../core/services/creator.service';
import { Campaign, CampaignSummary, CampaignCreator } from '../../core/models/campaign.model';
import { Deliverable } from '../../core/models/deliverable.model';
import { CreatorSummary } from '../../core/models/creator.model';
import { CampaignMetric, CampaignReport } from '../../core/models/reporting.model';
import { DeliverableMetricsModalComponent } from './deliverable-metrics-modal/deliverable-metrics-modal.component';

@Component({
  selector: 'app-reports-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LucideAngularModule,
    DeliverableMetricsModalComponent
  ],
  template: `
    <div class="space-y-6">
      <!-- Header -->
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <div class="flex items-center gap-2">
            <h1 class="text-2xl font-black tracking-tight text-slate-900">Campaign Analytics & Reports</h1>
            <span class="px-2.5 py-0.5 rounded-full bg-emerald-50 text-emerald-700 text-xs font-bold">
              PDF Engine Ready
            </span>
          </div>
          <p class="text-xs text-slate-500 mt-1">
            Manual performance metrics aggregation and asynchronous PDF export generation with jsreport & MinIO.
          </p>
        </div>

        <!-- Header Actions -->
        <div class="flex items-center gap-3">
          <button
            type="button"
            (click)="refreshAll()"
            [disabled]="!selectedCampaignId"
            class="p-2.5 rounded-xl border border-slate-200 text-slate-600 hover:bg-slate-50 disabled:opacity-40 transition"
            title="Refresh Analytics"
          >
            <lucide-icon [img]="RefreshCwIcon" class="w-4 h-4" [class.animate-spin]="isLoading()"></lucide-icon>
          </button>

          <button
            type="button"
            (click)="triggerGenerateReport()"
            [disabled]="!selectedCampaignId || isGeneratingPdf()"
            class="px-4 py-2.5 rounded-xl bg-gradient-to-r from-emerald-600 to-teal-600 hover:from-emerald-500 hover:to-teal-500 disabled:from-slate-300 disabled:to-slate-300 text-white text-xs font-semibold shadow-md shadow-emerald-600/20 transition flex items-center gap-2"
          >
            @if (isGeneratingPdf()) {
              <lucide-icon [img]="Loader2Icon" class="w-4 h-4 animate-spin"></lucide-icon>
              <span>Queueing Report...</span>
            } @else {
              <lucide-icon [img]="FileTextIcon" class="w-4 h-4"></lucide-icon>
              <span>Generate PDF Report</span>
            }
          </button>
        </div>
      </div>

      <!-- Campaign Selector Bar -->
      <div class="p-4 bg-white rounded-2xl border border-slate-200/80 shadow-xs flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div class="flex items-center gap-3 flex-1 max-w-xl">
          <div class="w-8 h-8 rounded-xl bg-emerald-50 text-emerald-600 flex items-center justify-center font-bold shrink-0">
            <lucide-icon [img]="BarChart3Icon" class="w-4 h-4"></lucide-icon>
          </div>
          <select
            [(ngModel)]="selectedCampaignId"
            (ngModelChange)="onCampaignChange()"
            class="w-full px-3.5 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs font-semibold text-slate-800 focus:outline-hidden focus:ring-2 focus:ring-emerald-500/20 focus:border-emerald-600 transition cursor-pointer"
          >
            <option value="" disabled selected>Select campaign for analytics...</option>
            @for (c of campaigns(); track c.id) {
              <option [value]="c.id">{{ c.title }} ({{ c.status }})</option>
            }
          </select>
        </div>

        @if (currentCampaign()) {
          <div class="flex items-center gap-3 text-xs text-slate-500">
            <span class="font-bold text-slate-700">Budget: {{ formatCurrency(currentCampaign()!.budget) }}</span>
            <span class="text-slate-300">•</span>
            <span>{{ currentCampaign()!.startDate | date:'MMM d' }} - {{ currentCampaign()!.endDate | date:'MMM d, yyyy' }}</span>
          </div>
        }
      </div>

      @if (isLoading()) {
        <div class="p-16 bg-white rounded-2xl border border-slate-200/80 shadow-xs text-center space-y-3">
          <lucide-icon [img]="Loader2Icon" class="w-8 h-8 text-emerald-600 animate-spin mx-auto"></lucide-icon>
          <p class="text-xs text-slate-500 font-medium">Aggregating campaign analytics & reports...</p>
        </div>
      } @else if (!selectedCampaignId) {
        <div class="p-16 bg-white rounded-2xl border border-slate-200/80 shadow-xs text-center space-y-3 max-w-md mx-auto">
          <div class="w-12 h-12 rounded-2xl bg-emerald-50 text-emerald-600 flex items-center justify-center mx-auto">
            <lucide-icon [img]="BarChart3Icon" class="w-6 h-6"></lucide-icon>
          </div>
          <h3 class="text-sm font-bold text-slate-900">Select a Campaign</h3>
          <p class="text-xs text-slate-500">
            Choose a campaign above to view total reach, engagements, deliverable performance breakdown, and generate executive PDF summaries.
          </p>
        </div>
      } @else {
        <!-- Summary KPI Cards Grid -->
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
          <!-- Card 1: Total Reach & Impressions -->
          <div class="p-5 rounded-2xl bg-white border border-slate-200/80 shadow-xs space-y-3">
            <div class="flex items-center justify-between">
              <span class="text-xs font-bold text-slate-500 uppercase tracking-wider">Total Reach</span>
              <div class="w-8 h-8 rounded-xl bg-indigo-50 text-indigo-600 flex items-center justify-center">
                <lucide-icon [img]="EyeIcon" class="w-4 h-4"></lucide-icon>
              </div>
            </div>
            <div>
              <h3 class="text-2xl font-black text-slate-900 tracking-tight">
                {{ summaryMetrics().totalReach | number }}
              </h3>
              <p class="text-[11px] text-slate-400 mt-0.5">
                {{ summaryMetrics().totalImpressions | number }} total impressions
              </p>
            </div>
          </div>

          <!-- Card 2: Total Video Views -->
          <div class="p-5 rounded-2xl bg-white border border-slate-200/80 shadow-xs space-y-3">
            <div class="flex items-center justify-between">
              <span class="text-xs font-bold text-slate-500 uppercase tracking-wider">Total Video Views</span>
              <div class="w-8 h-8 rounded-xl bg-emerald-50 text-emerald-600 flex items-center justify-center">
                <lucide-icon [img]="TrendingUpIcon" class="w-4 h-4"></lucide-icon>
              </div>
            </div>
            <div>
              <h3 class="text-2xl font-black text-slate-900 tracking-tight">
                {{ summaryMetrics().totalViews | number }}
              </h3>
              <p class="text-[11px] text-emerald-600 font-semibold mt-0.5">
                Cost Per View: {{ formatCurrency(summaryMetrics().cpv) }}
              </p>
            </div>
          </div>

          <!-- Card 3: Total Engagement -->
          <div class="p-5 rounded-2xl bg-white border border-slate-200/80 shadow-xs space-y-3">
            <div class="flex items-center justify-between">
              <span class="text-xs font-bold text-slate-500 uppercase tracking-wider">Total Engagement</span>
              <div class="w-8 h-8 rounded-xl bg-rose-50 text-rose-600 flex items-center justify-center">
                <lucide-icon [img]="HeartIcon" class="w-4 h-4"></lucide-icon>
              </div>
            </div>
            <div>
              <h3 class="text-2xl font-black text-slate-900 tracking-tight">
                {{ summaryMetrics().totalEngagement | number }}
              </h3>
              <p class="text-[11px] text-rose-600 font-semibold mt-0.5">
                Cost Per Eng.: {{ formatCurrency(summaryMetrics().cpe) }}
              </p>
            </div>
          </div>

          <!-- Card 4: Avg Engagement Rate -->
          <div class="p-5 rounded-2xl bg-white border border-slate-200/80 shadow-xs space-y-3">
            <div class="flex items-center justify-between">
              <span class="text-xs font-bold text-slate-500 uppercase tracking-wider">Avg Engagement Rate</span>
              <div class="w-8 h-8 rounded-xl bg-amber-50 text-amber-600 flex items-center justify-center">
                <lucide-icon [img]="SparklesIcon" class="w-4 h-4"></lucide-icon>
              </div>
            </div>
            <div>
              <h3 class="text-2xl font-black text-slate-900 tracking-tight">
                {{ summaryMetrics().avgEngagementRate | number:'1.2-2' }}%
              </h3>
              <p class="text-[11px] text-slate-400 mt-0.5">
                Across {{ deliverablesWithMetrics().length }} deliverable items
              </p>
            </div>
          </div>
        </div>

        <!-- Deliverables Performance Breakdown Table -->
        <div class="bg-white rounded-2xl border border-slate-200/80 shadow-xs overflow-hidden space-y-4 p-6">
          <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-2">
            <div>
              <h3 class="text-base font-bold text-slate-900">Deliverable Performance Breakdown</h3>
              <p class="text-xs text-slate-500">
                Log manual verified analytics for each creator submission.
              </p>
            </div>
          </div>

          @if (deliverablesWithMetrics().length === 0) {
            <div class="p-8 rounded-xl bg-slate-50 border border-slate-200/60 text-center space-y-2">
              <lucide-icon [img]="BarChart3Icon" class="w-6 h-6 text-slate-400 mx-auto"></lucide-icon>
              <p class="text-xs text-slate-600 font-medium">No deliverables found for this campaign.</p>
              <p class="text-[11px] text-slate-400">Add deliverables in the Deliverables module first.</p>
            </div>
          } @else {
            <div class="overflow-x-auto -mx-6 -mb-6">
              <table class="w-full text-left border-collapse">
                <thead>
                  <tr class="border-b border-slate-100 bg-slate-50/50 text-[11px] font-bold text-slate-500 uppercase tracking-wider">
                    <th class="py-3 px-6">Deliverable / Creator</th>
                    <th class="py-3 px-4">Platform & Type</th>
                    <th class="py-3 px-4 text-right">Reach</th>
                    <th class="py-3 px-4 text-right">Views</th>
                    <th class="py-3 px-4 text-right">Likes</th>
                    <th class="py-3 px-4 text-right">Comments</th>
                    <th class="py-3 px-4 text-right">Shares / Clicks</th>
                    <th class="py-3 px-4 text-right">Eng. Rate</th>
                    <th class="py-3 px-6 text-right">Action</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-100 text-xs">
                  @for (item of deliverablesWithMetrics(); track item.deliverable.id) {
                    <tr class="hover:bg-slate-50/75 transition">
                      <!-- Title & Creator -->
                      <td class="py-3.5 px-6">
                        <div class="space-y-0.5">
                          <p class="font-bold text-slate-900">{{ item.deliverable.title }}</p>
                          <p class="text-[11px] text-slate-500 font-medium">
                            {{ getCreatorNameForRoster(item.deliverable.campaignCreatorId) }}
                          </p>
                        </div>
                      </td>

                      <!-- Platform & Type -->
                      <td class="py-3.5 px-4">
                        <span class="inline-flex items-center gap-1.5 px-2.5 py-0.5 rounded-full bg-slate-100 text-slate-700 font-semibold text-[10px]">
                          {{ item.deliverable.platform }} • {{ item.deliverable.contentType }}
                        </span>
                      </td>

                      <!-- Reach -->
                      <td class="py-3.5 px-4 text-right font-semibold text-slate-800">
                        {{ item.metric ? (item.metric.reach | number) : '-' }}
                      </td>

                      <!-- Views -->
                      <td class="py-3.5 px-4 text-right font-semibold text-slate-800">
                        {{ item.metric ? (item.metric.views | number) : '-' }}
                      </td>

                      <!-- Likes -->
                      <td class="py-3.5 px-4 text-right font-semibold text-slate-800">
                        {{ item.metric ? (item.metric.likes | number) : '-' }}
                      </td>

                      <!-- Comments -->
                      <td class="py-3.5 px-4 text-right font-semibold text-slate-800">
                        {{ item.metric ? (item.metric.comments | number) : '-' }}
                      </td>

                      <!-- Shares / Clicks -->
                      <td class="py-3.5 px-4 text-right font-semibold text-slate-800">
                        @if (item.metric) {
                          <span>{{ item.metric.shares | number }}s / {{ item.metric.clicks | number }}c</span>
                        } @else {
                          <span>-</span>
                        }
                      </td>

                      <!-- Engagement Rate -->
                      <td class="py-3.5 px-4 text-right">
                        @if (item.metric) {
                          <span class="px-2 py-0.5 rounded-md bg-emerald-50 text-emerald-700 text-[11px] font-bold">
                            {{ item.metric.engagementRate | number:'1.2-2' }}%
                          </span>
                        } @else {
                          <span class="text-slate-400">-</span>
                        }
                      </td>

                      <!-- Action Button -->
                      <td class="py-3.5 px-6 text-right">
                        <button
                          type="button"
                          (click)="openMetricsModal(item.deliverable, item.metric)"
                          class="px-3 py-1.5 rounded-xl border border-slate-200 text-slate-700 hover:bg-slate-100 text-xs font-semibold transition"
                        >
                          {{ item.metric ? 'Edit Metrics' : 'Log Metrics' }}
                        </button>
                      </td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          }
        </div>

        <!-- Generated PDF Reports Archive -->
        <div class="bg-white rounded-2xl border border-slate-200/80 shadow-xs p-6 space-y-4">
          <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-2">
            <div>
              <h3 class="text-base font-bold text-slate-900">Exported PDF Reports</h3>
              <p class="text-xs text-slate-500">
                Asynchronous PDF generation history stored in MinIO object storage.
              </p>
            </div>

            <button
              type="button"
              (click)="loadReports()"
              class="p-2 rounded-xl border border-slate-200 text-slate-600 hover:bg-slate-50 text-xs font-semibold transition flex items-center gap-1.5 shrink-0 self-start sm:self-auto"
            >
              <lucide-icon [img]="RefreshCwIcon" class="w-3.5 h-3.5" [class.animate-spin]="isLoadingReports()"></lucide-icon>
              <span>Refresh Reports</span>
            </button>
          </div>

          @if (reports().length === 0) {
            <div class="p-8 rounded-xl bg-slate-50 border border-slate-200/60 text-center space-y-2">
              <lucide-icon [img]="FileTextIcon" class="w-6 h-6 text-slate-400 mx-auto"></lucide-icon>
              <p class="text-xs text-slate-600 font-medium">No PDF reports generated yet for this campaign.</p>
              <p class="text-[11px] text-slate-400">Click "Generate PDF Report" above to trigger asynchronous PDF generation.</p>
            </div>
          } @else {
            <div class="overflow-x-auto -mx-6 -mb-6">
              <table class="w-full text-left border-collapse">
                <thead>
                  <tr class="border-b border-slate-100 bg-slate-50/50 text-[11px] font-bold text-slate-500 uppercase tracking-wider">
                    <th class="py-3 px-6">Report ID</th>
                    <th class="py-3 px-4">Requested At</th>
                    <th class="py-3 px-4">Completed At</th>
                    <th class="py-3 px-4">Status</th>
                    <th class="py-3 px-6 text-right">Download</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-100 text-xs">
                  @for (report of reports(); track report.id) {
                    <tr class="hover:bg-slate-50/75 transition">
                      <!-- Report ID -->
                      <td class="py-3.5 px-6 font-mono text-slate-700 text-[11px]">
                        #{{ report.id.substring(0, 8) }}
                      </td>

                      <!-- Requested At -->
                      <td class="py-3.5 px-4 text-slate-600">
                        {{ report.requestedAt | date:'medium' }}
                      </td>

                      <!-- Completed At -->
                      <td class="py-3.5 px-4 text-slate-600">
                        {{ report.completedAt ? (report.completedAt | date:'medium') : '-' }}
                      </td>

                      <!-- Status -->
                      <td class="py-3.5 px-4">
                        @if (report.status === 'Completed') {
                          <span class="px-2.5 py-0.5 rounded-full bg-emerald-50 text-emerald-700 border border-emerald-200/60 font-bold text-[10px] inline-flex items-center gap-1">
                            <lucide-icon [img]="CheckCircle2Icon" class="w-3 h-3"></lucide-icon>
                            <span>Completed</span>
                          </span>
                        } @else if (report.status === 'Pending' || report.status === 'Generating') {
                          <span class="px-2.5 py-0.5 rounded-full bg-amber-50 text-amber-700 border border-amber-200/60 font-bold text-[10px] inline-flex items-center gap-1 animate-pulse">
                            <lucide-icon [img]="Loader2Icon" class="w-3 h-3 animate-spin"></lucide-icon>
                            <span>{{ report.status }}...</span>
                          </span>
                        } @else if (report.status === 'Failed') {
                          <span
                            class="px-2.5 py-0.5 rounded-full bg-rose-50 text-rose-700 border border-rose-200/60 font-bold text-[10px] inline-flex items-center gap-1"
                            [title]="report.errorMessage || 'Generation failed'"
                          >
                            <lucide-icon [img]="AlertCircleIcon" class="w-3 h-3"></lucide-icon>
                            <span>Failed</span>
                          </span>
                        }
                      </td>

                      <!-- Download Button -->
                      <td class="py-3.5 px-6 text-right">
                        @if (report.status === 'Completed') {
                          <button
                            type="button"
                            (click)="downloadReport(report)"
                            [disabled]="isDownloadingReport() === report.id"
                            class="px-3 py-1.5 rounded-xl bg-emerald-50 text-emerald-700 hover:bg-emerald-100 font-bold text-xs transition inline-flex items-center gap-1.5"
                          >
                            @if (isDownloadingReport() === report.id) {
                              <lucide-icon [img]="Loader2Icon" class="w-3.5 h-3.5 animate-spin"></lucide-icon>
                              <span>Downloading...</span>
                            } @else {
                              <lucide-icon [img]="DownloadIcon" class="w-3.5 h-3.5"></lucide-icon>
                              <span>Download PDF</span>
                            }
                          </button>
                        } @else {
                          <span class="text-slate-400 text-xs">-</span>
                        }
                      </td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          }
        </div>
      }

      <!-- Deliverable Metrics Modal -->
      <app-deliverable-metrics-modal
        [isOpen]="isMetricsModalOpen()"
        [deliverable]="selectedDeliverableForModal()"
        [existingMetric]="selectedMetricForModal()"
        (close)="closeMetricsModal()"
        (saved)="onMetricSaved($event)"
      ></app-deliverable-metrics-modal>
    </div>
  `
})
export class ReportsDashboardComponent implements OnInit, OnDestroy {
  private reportingService = inject(ReportingService);
  private campaignService = inject(CampaignService);
  private deliverableService = inject(DeliverableService);
  private creatorService = inject(CreatorService);

  readonly campaigns = signal<CampaignSummary[]>([]);
  readonly currentCampaign = signal<Campaign | null>(null);
  selectedCampaignId = '';

  readonly deliverablesWithMetrics = signal<{
    deliverable: Deliverable;
    metric: CampaignMetric | null;
  }[]>([]);

  readonly rosterCreatorsMap = signal<Map<string, CampaignCreator>>(new Map());
  readonly creatorsMap = signal<Map<string, CreatorSummary>>(new Map());

  readonly reports = signal<CampaignReport[]>([]);

  readonly isLoading = signal<boolean>(false);
  readonly isLoadingReports = signal<boolean>(false);
  readonly isGeneratingPdf = signal<boolean>(false);
  readonly isDownloadingReport = signal<string | null>(null);

  readonly isMetricsModalOpen = signal<boolean>(false);
  readonly selectedDeliverableForModal = signal<Deliverable | null>(null);
  readonly selectedMetricForModal = signal<CampaignMetric | null>(null);

  private pollingTimer: any = null;

  // Icons
  readonly BarChart3Icon = BarChart3;
  readonly FileTextIcon = FileText;
  readonly DownloadIcon = Download;
  readonly PlusIcon = Plus;
  readonly RefreshCwIcon = RefreshCw;
  readonly Loader2Icon = Loader2;
  readonly TrendingUpIcon = TrendingUp;
  readonly EyeIcon = Eye;
  readonly HeartIcon = Heart;
  readonly Share2Icon = Share2;
  readonly CheckCircle2Icon = CheckCircle2;
  readonly AlertCircleIcon = AlertCircle;
  readonly ClockIcon = Clock;
  readonly ExternalLinkIcon = ExternalLink;
  readonly CoinsIcon = Coins;
  readonly SparklesIcon = Sparkles;
  readonly CalendarIcon = Calendar;
  readonly LayersIcon = Layers;
  readonly UsersIcon = Users;

  ngOnInit(): void {
    this.loadCampaigns();
    this.loadCreators();
  }

  ngOnDestroy(): void {
    this.stopPolling();
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
    if (!this.selectedCampaignId) return;

    this.loadCampaignDetails();
    this.loadDeliverablesAndMetrics();
    this.loadReports();
  }

  private loadCampaignDetails(): void {
    this.campaignService.getCampaignById(this.selectedCampaignId).subscribe({
      next: res => {
        if (res.success && res.data) {
          this.currentCampaign.set(res.data);
        }
      }
    });

    this.campaignService.getCampaignRoster(this.selectedCampaignId).subscribe({
      next: res => {
        if (res.success && res.data) {
          const map = new Map<string, CampaignCreator>();
          for (const rc of res.data) {
            map.set(rc.id, rc);
          }
          this.rosterCreatorsMap.set(map);
        }
      }
    });
  }

  private loadDeliverablesAndMetrics(): void {
    this.isLoading.set(true);
    this.deliverableService.getDeliverablesByCampaign(this.selectedCampaignId).subscribe({
      next: res => {
        if (res.success && res.data) {
          const deliverableList = res.data;
          const results: { deliverable: Deliverable; metric: CampaignMetric | null }[] = [];

          let pendingCalls = deliverableList.length;
          if (pendingCalls === 0) {
            this.deliverablesWithMetrics.set([]);
            this.isLoading.set(false);
            return;
          }

          deliverableList.forEach(del => {
            this.reportingService.getDeliverableMetrics(del.id).subscribe({
              next: metricRes => {
                results.push({
                  deliverable: del,
                  metric: metricRes.success ? metricRes.data : null
                });
                pendingCalls--;
                if (pendingCalls === 0) {
                  this.deliverablesWithMetrics.set(results);
                  this.isLoading.set(false);
                }
              },
              error: () => {
                results.push({
                  deliverable: del,
                  metric: null
                });
                pendingCalls--;
                if (pendingCalls === 0) {
                  this.deliverablesWithMetrics.set(results);
                  this.isLoading.set(false);
                }
              }
            });
          });
        } else {
          this.deliverablesWithMetrics.set([]);
          this.isLoading.set(false);
        }
      },
      error: () => {
        this.isLoading.set(false);
        toast.error('Failed to load campaign deliverables');
      }
    });
  }

  loadReports(): void {
    if (!this.selectedCampaignId) return;

    this.isLoadingReports.set(true);
    this.reportingService.getCampaignReports(this.selectedCampaignId).subscribe({
      next: res => {
        this.isLoadingReports.set(false);
        if (res.success && res.data) {
          this.reports.set(res.data);
          this.checkAndManagePolling(res.data);
        }
      },
      error: () => {
        this.isLoadingReports.set(false);
      }
    });
  }

  private checkAndManagePolling(reportsList: CampaignReport[]): void {
    const hasActiveJob = reportsList.some(
      r => r.status === 'Pending' || r.status === 'Generating'
    );

    if (hasActiveJob && !this.pollingTimer) {
      this.pollingTimer = setInterval(() => {
        if (this.selectedCampaignId) {
          this.reportingService.getCampaignReports(this.selectedCampaignId).subscribe({
            next: res => {
              if (res.success && res.data) {
                this.reports.set(res.data);
                const stillActive = res.data.some(
                  r => r.status === 'Pending' || r.status === 'Generating'
                );
                if (!stillActive) {
                  this.stopPolling();
                  toast.success('PDF report generation complete!');
                }
              }
            }
          });
        }
      }, 5000);
    } else if (!hasActiveJob && this.pollingTimer) {
      this.stopPolling();
    }
  }

  private stopPolling(): void {
    if (this.pollingTimer) {
      clearInterval(this.pollingTimer);
      this.pollingTimer = null;
    }
  }

  triggerGenerateReport(): void {
    if (!this.selectedCampaignId) return;

    this.isGeneratingPdf.set(true);
    this.reportingService.generateCampaignReport(this.selectedCampaignId).subscribe({
      next: res => {
        this.isGeneratingPdf.set(false);
        toast.success('PDF report generation queued (202 Accepted)! Polling status...');
        this.loadReports();
      },
      error: err => {
        this.isGeneratingPdf.set(false);
        const msg = err.error?.detail || err.error?.message || 'Failed to queue report generation';
        toast.error(msg);
      }
    });
  }

  downloadReport(report: CampaignReport): void {
    if (report.downloadUrl) {
      window.open(report.downloadUrl, '_blank');
      return;
    }

    this.isDownloadingReport.set(report.id);
    this.reportingService.getReportDownloadUrl(report.id).subscribe({
      next: res => {
        this.isDownloadingReport.set(null);
        if (res.success && res.data) {
          window.open(res.data, '_blank');
        } else {
          toast.error('Download URL not available.');
        }
      },
      error: () => {
        this.isDownloadingReport.set(null);
        toast.error('Failed to get download URL.');
      }
    });
  }

  summaryMetrics = computed(() => {
    const list = this.deliverablesWithMetrics();
    const budget = this.currentCampaign()?.budget || 0;

    let totalReach = 0;
    let totalImpressions = 0;
    let totalViews = 0;
    let totalEngagement = 0;
    let metricsCount = 0;
    let totalEngagementRateSum = 0;

    for (const item of list) {
      if (item.metric) {
        totalReach += item.metric.reach || 0;
        totalImpressions += item.metric.impressions || 0;
        totalViews += item.metric.views || 0;
        totalEngagement += item.metric.totalEngagement || 0;
        totalEngagementRateSum += item.metric.engagementRate || 0;
        metricsCount++;
      }
    }

    const avgEngagementRate = metricsCount > 0 ? totalEngagementRateSum / metricsCount : 0;
    const cpv = totalViews > 0 ? budget / totalViews : 0;
    const cpe = totalEngagement > 0 ? budget / totalEngagement : 0;

    return {
      totalReach,
      totalImpressions,
      totalViews,
      totalEngagement,
      avgEngagementRate,
      cpv,
      cpe
    };
  });

  refreshAll(): void {
    this.loadCampaignDetails();
    this.loadDeliverablesAndMetrics();
    this.loadReports();
  }

  getCreatorNameForRoster(campaignCreatorId: string): string {
    const rosterItem = this.rosterCreatorsMap().get(campaignCreatorId);
    if (!rosterItem) return 'Creator';
    const creator = this.creatorsMap().get(rosterItem.creatorId);
    return creator ? creator.fullName : `Creator #${rosterItem.creatorId.substring(0, 6)}`;
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('id-ID', {
      style: 'currency',
      currency: 'IDR',
      maximumFractionDigits: 0
    }).format(amount);
  }

  openMetricsModal(del: Deliverable, metric: CampaignMetric | null): void {
    this.selectedDeliverableForModal.set(del);
    this.selectedMetricForModal.set(metric);
    this.isMetricsModalOpen.set(true);
  }

  closeMetricsModal(): void {
    this.isMetricsModalOpen.set(false);
    this.selectedDeliverableForModal.set(null);
    this.selectedMetricForModal.set(null);
  }

  onMetricSaved(savedMetric: CampaignMetric): void {
    this.loadDeliverablesAndMetrics();
  }
}
