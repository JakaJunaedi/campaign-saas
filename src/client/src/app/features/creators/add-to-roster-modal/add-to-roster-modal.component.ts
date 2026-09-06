import { Component, EventEmitter, Input, Output, OnInit, inject, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import {
  LucideAngularModule,
  X,
  UserPlus,
  Megaphone,
  DollarSign,
  Loader2,
  Save,
  Users
} from 'lucide-angular';
import { toast } from 'ngx-sonner';
import { CampaignService } from '../../../core/services/campaign.service';
import { CreatorService } from '../../../core/services/creator.service';
import { CampaignSummary } from '../../../core/models/campaign.model';
import { Creator, CreatorSummary } from '../../../core/models/creator.model';

@Component({
  selector: 'app-add-to-roster-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule],
  template: `
    @if (isOpen) {
      <div class="fixed inset-0 z-50 overflow-y-auto bg-slate-900/60 backdrop-blur-xs flex items-center justify-center p-4 sm:p-6 animate-in fade-in duration-200">
        <div
          class="bg-white rounded-3xl shadow-2xl border border-slate-100 w-full max-w-lg overflow-hidden transform transition-all"
          (click)="$event.stopPropagation()"
        >
          <!-- Header -->
          <div class="px-6 py-5 border-b border-slate-100 flex items-center justify-between bg-slate-50/50">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-2xl bg-indigo-50 text-indigo-600 flex items-center justify-center font-bold">
                <lucide-icon [img]="UserPlusIcon" class="w-5 h-5"></lucide-icon>
              </div>
              <div>
                <h3 class="text-lg font-bold text-slate-900">Enroll Creator to Roster</h3>
                <p class="text-xs text-slate-500">Assign influencer to a campaign roster board.</p>
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

          <!-- Form -->
          <form [formGroup]="rosterForm" (ngSubmit)="onSubmit()" class="p-6 space-y-4">
            <!-- Campaign Selection -->
            <div class="space-y-1.5">
              <label class="block text-xs font-semibold text-slate-700">
                Target Campaign <span class="text-rose-500">*</span>
              </label>
              <select
                formControlName="campaignId"
                class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-xs bg-white focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition"
                [class.border-rose-300]="isFieldInvalid('campaignId')"
              >
                <option value="" disabled selected>Select active campaign...</option>
                @for (c of campaigns(); track c.id) {
                  <option [value]="c.id">{{ c.title }} ({{ c.status }})</option>
                }
              </select>
              @if (isFieldInvalid('campaignId')) {
                <p class="text-[11px] text-rose-500 font-medium">Please select a campaign.</p>
              }
            </div>

            <!-- Creator Selection -->
            <div class="space-y-1.5">
              <label class="block text-xs font-semibold text-slate-700">
                Creator / KOL <span class="text-rose-500">*</span>
              </label>
              @if (preselectedCreator) {
                <div class="p-3 rounded-xl bg-slate-50 border border-slate-200 text-xs flex items-center justify-between">
                  <span class="font-bold text-slate-900">{{ preselectedCreator.fullName }}</span>
                  <span class="text-indigo-600 font-semibold">{{ preselectedCreator.niche }}</span>
                </div>
              } @else {
                <select
                  formControlName="creatorId"
                  class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-xs bg-white focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition"
                  [class.border-rose-300]="isFieldInvalid('creatorId')"
                >
                  <option value="" disabled selected>Select creator from CRM...</option>
                  @for (cr of creators(); track cr.id) {
                    <option [value]="cr.id">{{ cr.fullName }} — {{ cr.niche }}</option>
                  }
                </select>
              }
              @if (isFieldInvalid('creatorId')) {
                <p class="text-[11px] text-rose-500 font-medium">Please select a creator.</p>
              }
            </div>

            <!-- Agreed Rate (IDR) -->
            <div class="space-y-1.5">
              <label class="block text-xs font-semibold text-slate-700">
                Agreed Rate / Fee <span class="text-rose-500">*</span>
              </label>
              <div class="relative">
                <span class="absolute left-3 top-1/2 -translate-y-1/2 text-xs font-semibold text-slate-400">Rp</span>
                <input
                  type="number"
                  formControlName="agreedRate"
                  placeholder="5000000"
                  min="0"
                  class="w-full pl-8 pr-3.5 py-2.5 rounded-xl border border-slate-200 text-xs focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition"
                  [class.border-rose-300]="isFieldInvalid('agreedRate')"
                />
              </div>
              <p class="text-[11px] text-slate-400">Rate agreed with creator for deliverables under this campaign.</p>
            </div>

            <!-- Footer Actions -->
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
                [disabled]="isSubmitting() || rosterForm.invalid"
                class="px-5 py-2.5 rounded-xl bg-indigo-600 hover:bg-indigo-500 disabled:bg-indigo-300 text-white text-xs font-semibold shadow-md shadow-indigo-600/20 transition flex items-center gap-2"
              >
                @if (isSubmitting()) {
                  <lucide-icon [img]="Loader2Icon" class="w-4 h-4 animate-spin"></lucide-icon>
                  <span>Enrolling...</span>
                } @else {
                  <lucide-icon [img]="SaveIcon" class="w-4 h-4"></lucide-icon>
                  <span>Add to Roster</span>
                }
              </button>
            </div>
          </form>
        </div>
      </div>
    }
  `
})
export class AddToRosterModalComponent implements OnInit {
  @Input() isOpen = false;
  @Input() preselectedCreator: Creator | CreatorSummary | null = null;
  @Input() preselectedCampaignId?: string;
  @Output() close = new EventEmitter<void>();
  @Output() enrolled = new EventEmitter<void>();

  private fb = inject(FormBuilder);
  private campaignService = inject(CampaignService);
  private creatorService = inject(CreatorService);

  readonly campaigns = signal<CampaignSummary[]>([]);
  readonly creators = signal<CreatorSummary[]>([]);
  readonly isSubmitting = signal<boolean>(false);

  readonly UserPlusIcon = UserPlus;
  readonly MegaphoneIcon = Megaphone;
  readonly DollarSignIcon = DollarSign;
  readonly XIcon = X;
  readonly Loader2Icon = Loader2;
  readonly SaveIcon = Save;
  readonly UsersIcon = Users;

  rosterForm: FormGroup = this.fb.group({
    campaignId: ['', [Validators.required]],
    creatorId: ['', [Validators.required]],
    agreedRate: [0, [Validators.required, Validators.min(0)]]
  });

  constructor() {
    effect(() => {
      if (this.isOpen) {
        this.populate();
        this.loadCampaigns();
        if (!this.preselectedCreator) {
          this.loadCreators();
        }
      }
    });
  }

  ngOnInit(): void {
    this.loadCampaigns();
  }

  private loadCampaigns(): void {
    this.campaignService.getCampaigns('', undefined, undefined, 1, 100).subscribe({
      next: res => {
        if (res.success && res.data) {
          this.campaigns.set(res.data.items);
        }
      }
    });
  }

  private loadCreators(): void {
    this.creatorService.getCreators('', undefined, undefined, undefined, 1, 100).subscribe({
      next: res => {
        if (res.success && res.data) {
          this.creators.set(res.data.items);
        }
      }
    });
  }

  private populate(): void {
    this.rosterForm.reset({
      campaignId: this.preselectedCampaignId || '',
      creatorId: this.preselectedCreator?.id || '',
      agreedRate: 0
    });
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.rosterForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  onCancel(): void {
    this.rosterForm.reset();
    this.close.emit();
  }

  onSubmit(): void {
    if (this.rosterForm.invalid) {
      this.rosterForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    const formVal = this.rosterForm.value;

    const campaignId = formVal.campaignId;
    const req = {
      creatorId: formVal.creatorId,
      agreedRate: Number(formVal.agreedRate)
    };

    this.campaignService.addCreatorToRoster(campaignId, req).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        toast.success('Creator enrolled into campaign roster');
        this.enrolled.emit();
        this.onCancel();
      },
      error: err => {
        this.isSubmitting.set(false);
        const msg = err.error?.detail || err.error?.message || 'Failed to add creator to roster';
        toast.error(msg);
      }
    });
  }
}
