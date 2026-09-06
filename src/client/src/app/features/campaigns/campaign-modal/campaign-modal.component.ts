import { Component, EventEmitter, Input, Output, OnInit, inject, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import {
  LucideAngularModule,
  X,
  Megaphone,
  Building2,
  Calendar,
  DollarSign,
  FileText,
  Loader2,
  Save
} from 'lucide-angular';
import { toast } from 'ngx-sonner';
import { CampaignService } from '../../../core/services/campaign.service';
import { ClientService } from '../../../core/services/client.service';
import { Campaign, CreateCampaignRequest, UpdateCampaignRequest } from '../../../core/models/campaign.model';
import { ClientSummary } from '../../../core/models/client.model';

@Component({
  selector: 'app-campaign-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule],
  template: `
    @if (isOpen) {
      <div class="fixed inset-0 z-50 overflow-y-auto bg-slate-900/60 backdrop-blur-xs flex items-center justify-center p-4 sm:p-6 animate-in fade-in duration-200">
        <div
          class="bg-white rounded-3xl shadow-2xl border border-slate-100 w-full max-w-2xl overflow-hidden transform transition-all"
          (click)="$event.stopPropagation()"
        >
          <!-- Modal Header -->
          <div class="px-6 py-5 border-b border-slate-100 flex items-center justify-between bg-slate-50/50">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-2xl bg-indigo-50 text-indigo-600 flex items-center justify-center font-bold">
                <lucide-icon [img]="MegaphoneIcon" class="w-5 h-5"></lucide-icon>
              </div>
              <div>
                <h3 class="text-lg font-bold text-slate-900">
                  {{ campaign ? 'Edit Campaign Brief' : 'Create New Campaign' }}
                </h3>
                <p class="text-xs text-slate-500">
                  {{ campaign ? 'Update campaign schedule, budget, and brief details.' : 'Define campaign brief, assigned brand client, dates, and budget.' }}
                </p>
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

          <!-- Modal Body & Form -->
          <form [formGroup]="campaignForm" (ngSubmit)="onSubmit()" class="p-6 space-y-5 max-h-[75vh] overflow-y-auto">
            <!-- Client Selection (Only required for new campaign) -->
            @if (!campaign) {
              <div class="space-y-1.5">
                <label class="block text-xs font-semibold text-slate-700">
                  Select Brand Client <span class="text-rose-500">*</span>
                </label>
                <div class="relative">
                  <select
                    formControlName="clientId"
                    class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-xs bg-white focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition"
                    [class.border-rose-300]="isFieldInvalid('clientId')"
                  >
                    <option value="" disabled selected>Select a client brand...</option>
                    @for (client of clients(); track client.id) {
                      <option [value]="client.id">{{ client.name }} {{ client.companyName ? '(' + client.companyName + ')' : '' }}</option>
                    }
                  </select>
                </div>
                @if (isFieldInvalid('clientId')) {
                  <p class="text-[11px] text-rose-500 font-medium">Please select a client for this campaign.</p>
                }
              </div>
            }

            <!-- Campaign Title -->
            <div class="space-y-1.5">
              <label class="block text-xs font-semibold text-slate-700">
                Campaign Title <span class="text-rose-500">*</span>
              </label>
              <input
                type="text"
                formControlName="title"
                placeholder="e.g. Ramadan Special Sale 2026, Q2 Product Launch"
                class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-sm focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition"
                [class.border-rose-300]="isFieldInvalid('title')"
              />
              @if (isFieldInvalid('title')) {
                <p class="text-[11px] text-rose-500 font-medium">Campaign title is required.</p>
              }
            </div>

            <!-- Campaign Description -->
            <div class="space-y-1.5">
              <label class="block text-xs font-semibold text-slate-700">Campaign Brief & Objectives</label>
              <textarea
                formControlName="description"
                rows="3"
                placeholder="Key deliverables, campaign storyline, target audience, brand dos & don'ts..."
                class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-xs focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition"
              ></textarea>
            </div>

            <!-- Budget & Schedule Row -->
            <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
              <!-- Budget -->
              <div class="space-y-1.5">
                <label class="block text-xs font-semibold text-slate-700">
                  Total Budget <span class="text-rose-500">*</span>
                </label>
                <div class="relative">
                  <span class="absolute left-3 top-1/2 -translate-y-1/2 text-xs font-semibold text-slate-400">Rp</span>
                  <input
                    type="number"
                    formControlName="budget"
                    placeholder="50000000"
                    min="0"
                    class="w-full pl-8 pr-3.5 py-2.5 rounded-xl border border-slate-200 text-xs focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition"
                    [class.border-rose-300]="isFieldInvalid('budget')"
                  />
                </div>
                @if (isFieldInvalid('budget')) {
                  <p class="text-[11px] text-rose-500 font-medium">Valid budget is required.</p>
                }
              </div>

              <!-- Start Date -->
              <div class="space-y-1.5">
                <label class="block text-xs font-semibold text-slate-700">
                  Start Date <span class="text-rose-500">*</span>
                </label>
                <input
                  type="date"
                  formControlName="startDate"
                  class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-xs focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition"
                  [class.border-rose-300]="isFieldInvalid('startDate')"
                />
              </div>

              <!-- End Date -->
              <div class="space-y-1.5">
                <label class="block text-xs font-semibold text-slate-700">
                  End Date <span class="text-rose-500">*</span>
                </label>
                <input
                  type="date"
                  formControlName="endDate"
                  class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-xs focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition"
                  [class.border-rose-300]="isFieldInvalid('endDate')"
                />
              </div>
            </div>

            <!-- Form Action Footer -->
            <div class="pt-4 border-t border-slate-100 flex items-center justify-end gap-3">
              <button
                type="button"
                (click)="onCancel()"
                [disabled]="isSubmitting()"
                class="px-4 py-2.5 rounded-xl border border-slate-200 text-slate-700 hover:bg-slate-50 text-xs font-semibold transition"
              >
                Cancel
              </button>

              <button
                type="submit"
                [disabled]="isSubmitting() || campaignForm.invalid"
                class="px-5 py-2.5 rounded-xl bg-indigo-600 hover:bg-indigo-500 disabled:bg-indigo-300 text-white text-xs font-semibold shadow-md shadow-indigo-600/20 transition flex items-center gap-2"
              >
                @if (isSubmitting()) {
                  <lucide-icon [img]="Loader2Icon" class="w-4 h-4 animate-spin"></lucide-icon>
                  <span>Saving...</span>
                } @else {
                  <lucide-icon [img]="SaveIcon" class="w-4 h-4"></lucide-icon>
                  <span>{{ campaign ? 'Update Campaign' : 'Create Campaign' }}</span>
                }
              </button>
            </div>
          </form>
        </div>
      </div>
    }
  `
})
export class CampaignModalComponent implements OnInit {
  @Input() isOpen = false;
  @Input() campaign: Campaign | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<Campaign>();

  private fb = inject(FormBuilder);
  private campaignService = inject(CampaignService);
  private clientService = inject(ClientService);

  readonly clients = signal<ClientSummary[]>([]);
  readonly isSubmitting = signal<boolean>(false);

  readonly XIcon = X;
  readonly MegaphoneIcon = Megaphone;
  readonly Building2Icon = Building2;
  readonly CalendarIcon = Calendar;
  readonly DollarSignIcon = DollarSign;
  readonly FileTextIcon = FileText;
  readonly Loader2Icon = Loader2;
  readonly SaveIcon = Save;

  campaignForm: FormGroup = this.fb.group({
    clientId: ['', [Validators.required]],
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: [''],
    budget: [0, [Validators.required, Validators.min(0)]],
    startDate: ['', [Validators.required]],
    endDate: ['', [Validators.required]]
  });

  constructor() {
    effect(() => {
      if (this.isOpen) {
        this.populateForm();
        this.loadClientsList();
      }
    });
  }

  ngOnInit(): void {
    this.loadClientsList();
  }

  private loadClientsList(): void {
    this.clientService.getClients('', 1, 100).subscribe({
      next: res => {
        if (res.success && res.data) {
          this.clients.set(res.data.items);
        }
      }
    });
  }

  private populateForm(): void {
    if (this.campaign) {
      this.campaignForm.patchValue({
        clientId: this.campaign.clientId,
        title: this.campaign.title,
        description: this.campaign.description || '',
        budget: this.campaign.budget,
        startDate: this.campaign.startDate,
        endDate: this.campaign.endDate
      });
      this.campaignForm.get('clientId')?.disable();
    } else {
      const today = new Date().toISOString().split('T')[0];
      const nextMonth = new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0];

      this.campaignForm.enable();
      this.campaignForm.reset({
        clientId: '',
        title: '',
        description: '',
        budget: 0,
        startDate: today,
        endDate: nextMonth
      });
    }
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.campaignForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  onCancel(): void {
    this.campaignForm.reset();
    this.close.emit();
  }

  onSubmit(): void {
    if (this.campaignForm.invalid) {
      this.campaignForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    const formVal = this.campaignForm.getRawValue();

    if (this.campaign) {
      const updateReq: UpdateCampaignRequest = {
        title: formVal.title.trim(),
        description: formVal.description?.trim() || undefined,
        budget: Number(formVal.budget),
        startDate: formVal.startDate,
        endDate: formVal.endDate
      };

      this.campaignService.updateCampaign(this.campaign.id, updateReq).subscribe({
        next: res => {
          this.isSubmitting.set(false);
          toast.success('Campaign updated successfully');
          this.saved.emit(res.data);
          this.onCancel();
        },
        error: err => {
          this.isSubmitting.set(false);
          const msg = err.error?.detail || err.error?.message || 'Failed to update campaign';
          toast.error(msg);
        }
      });
    } else {
      const createReq: CreateCampaignRequest = {
        clientId: formVal.clientId,
        title: formVal.title.trim(),
        description: formVal.description?.trim() || undefined,
        budget: Number(formVal.budget),
        startDate: formVal.startDate,
        endDate: formVal.endDate
      };

      this.campaignService.createCampaign(createReq).subscribe({
        next: res => {
          this.isSubmitting.set(false);
          toast.success('Campaign created successfully');
          this.saved.emit(res.data);
          this.onCancel();
        },
        error: err => {
          this.isSubmitting.set(false);
          const msg = err.error?.detail || err.error?.message || 'Failed to create campaign';
          toast.error(msg);
        }
      });
    }
  }
}
