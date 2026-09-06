import { Component, EventEmitter, Input, Output, OnInit, inject, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import {
  LucideAngularModule,
  X,
  FileCheck2,
  Calendar,
  Layers,
  FileText,
  Loader2,
  Save,
  Users,
  Share2
} from 'lucide-angular';
import { toast } from 'ngx-sonner';
import { DeliverableService } from '../../../core/services/deliverable.service';
import { CampaignService } from '../../../core/services/campaign.service';
import { CreatorService } from '../../../core/services/creator.service';
import { Deliverable, CreateDeliverableRequest, UpdateDeliverableRequest } from '../../../core/models/deliverable.model';
import { CampaignCreator } from '../../../core/models/campaign.model';
import { CreatorSummary } from '../../../core/models/creator.model';

@Component({
  selector: 'app-deliverable-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule],
  template: `
    @if (isOpen) {
      <div class="fixed inset-0 z-50 overflow-y-auto bg-slate-900/60 backdrop-blur-xs flex items-center justify-center p-4 sm:p-6 animate-in fade-in duration-200">
        <div
          class="bg-white rounded-3xl shadow-2xl border border-slate-100 w-full max-w-xl overflow-hidden transform transition-all"
          (click)="$event.stopPropagation()"
        >
          <!-- Header -->
          <div class="px-6 py-5 border-b border-slate-100 flex items-center justify-between bg-slate-50/50">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-2xl bg-amber-50 text-amber-600 flex items-center justify-center font-bold">
                <lucide-icon [img]="FileCheck2Icon" class="w-5 h-5"></lucide-icon>
              </div>
              <div>
                <h3 class="text-lg font-bold text-slate-900">
                  {{ deliverable ? 'Edit Deliverable Brief' : 'Create Deliverable Item' }}
                </h3>
                <p class="text-xs text-slate-500">
                  {{ deliverable ? 'Update deliverable scope, content type, and deadline.' : 'Assign content deliverable to a creator on this campaign.' }}
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

          <!-- Form -->
          <form [formGroup]="deliverableForm" (ngSubmit)="onSubmit()" class="p-6 space-y-4 max-h-[75vh] overflow-y-auto">
            <!-- Creator Selection (only on creation) -->
            @if (!deliverable) {
              <div class="space-y-1.5">
                <label class="block text-xs font-semibold text-slate-700">
                  Assigned Creator <span class="text-rose-500">*</span>
                </label>
                <select
                  formControlName="campaignCreatorId"
                  class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-xs bg-white focus:outline-hidden focus:ring-2 focus:ring-amber-500/20 focus:border-amber-600 transition"
                  [class.border-rose-300]="isFieldInvalid('campaignCreatorId')"
                >
                  <option value="" disabled selected>Select enrolled creator...</option>
                  @for (c of rosterCreators; track c.id) {
                    <option [value]="c.id">
                      {{ getCreatorName(c.creatorId) }} (Status: {{ c.status }})
                    </option>
                  }
                </select>
                @if (isFieldInvalid('campaignCreatorId')) {
                  <p class="text-[11px] text-rose-500 font-medium">Please select a creator from the roster.</p>
                }
              </div>
            }

            <!-- Deliverable Title -->
            <div class="space-y-1.5">
              <label class="block text-xs font-semibold text-slate-700">
                Deliverable Title <span class="text-rose-500">*</span>
              </label>
              <input
                type="text"
                formControlName="title"
                placeholder="e.g. TikTok 60s Unboxing & Review, IG Carousel 5 Slides"
                class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-sm focus:outline-hidden focus:ring-2 focus:ring-amber-500/20 focus:border-amber-600 transition"
                [class.border-rose-300]="isFieldInvalid('title')"
              />
              @if (isFieldInvalid('title')) {
                <p class="text-[11px] text-rose-500 font-medium">Title is required.</p>
              }
            </div>

            <!-- Platform & Content Type Row -->
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <!-- Platform -->
              <div class="space-y-1.5">
                <label class="block text-xs font-semibold text-slate-700">
                  Platform <span class="text-rose-500">*</span>
                </label>
                <select
                  formControlName="platform"
                  class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-xs bg-white focus:outline-hidden focus:ring-2 focus:ring-amber-500/20 focus:border-amber-600 transition"
                >
                  <option value="Instagram">Instagram</option>
                  <option value="TikTok">TikTok</option>
                  <option value="YouTube">YouTube</option>
                  <option value="Twitter">Twitter / X</option>
                </select>
              </div>

              <!-- Content Type -->
              <div class="space-y-1.5">
                <label class="block text-xs font-semibold text-slate-700">
                  Content Format <span class="text-rose-500">*</span>
                </label>
                <select
                  formControlName="contentType"
                  class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-xs bg-white focus:outline-hidden focus:ring-2 focus:ring-amber-500/20 focus:border-amber-600 transition"
                >
                  <option value="Reels">Reels / Short Video</option>
                  <option value="TikTok Video">TikTok Video</option>
                  <option value="Story">Story / Highlights</option>
                  <option value="Post / Carousel">Feed Post / Carousel</option>
                  <option value="YouTube Video">YouTube Main Video</option>
                </select>
              </div>
            </div>

            <!-- Due Date -->
            <div class="space-y-1.5">
              <label class="block text-xs font-semibold text-slate-700">
                Submission Due Date <span class="text-rose-500">*</span>
              </label>
              <input
                type="date"
                formControlName="dueDate"
                class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-xs focus:outline-hidden focus:ring-2 focus:ring-amber-500/20 focus:border-amber-600 transition"
                [class.border-rose-300]="isFieldInvalid('dueDate')"
              />
              @if (isFieldInvalid('dueDate')) {
                <p class="text-[11px] text-rose-500 font-medium">Due date is required.</p>
              }
            </div>

            <!-- Brief Notes -->
            <div class="space-y-1.5">
              <label class="block text-xs font-semibold text-slate-700">Specific Brief Notes & Guidelines</label>
              <textarea
                formControlName="briefNotes"
                rows="3"
                placeholder="Key message points, brand mention tags (@brand), mandatory hashtags (#ad), CTA..."
                class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-xs focus:outline-hidden focus:ring-2 focus:ring-amber-500/20 focus:border-amber-600 transition"
              ></textarea>
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
                [disabled]="isSubmitting() || deliverableForm.invalid"
                class="px-5 py-2.5 rounded-xl bg-amber-600 hover:bg-amber-500 disabled:bg-amber-300 text-white text-xs font-semibold shadow-md shadow-amber-600/20 transition flex items-center gap-2"
              >
                @if (isSubmitting()) {
                  <lucide-icon [img]="Loader2Icon" class="w-4 h-4 animate-spin"></lucide-icon>
                  <span>Saving...</span>
                } @else {
                  <lucide-icon [img]="SaveIcon" class="w-4 h-4"></lucide-icon>
                  <span>{{ deliverable ? 'Update Deliverable' : 'Create Deliverable' }}</span>
                }
              </button>
            </div>
          </form>
        </div>
      </div>
    }
  `
})
export class DeliverableModalComponent {
  @Input() isOpen = false;
  @Input() campaignId!: string;
  @Input() rosterCreators: CampaignCreator[] = [];
  @Input() deliverable: Deliverable | null = null;
  @Input() creatorsMap = new Map<string, CreatorSummary>();
  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<Deliverable>();

  private fb = inject(FormBuilder);
  private deliverableService = inject(DeliverableService);

  readonly isSubmitting = signal<boolean>(false);

  readonly XIcon = X;
  readonly FileCheck2Icon = FileCheck2;
  readonly CalendarIcon = Calendar;
  readonly LayersIcon = Layers;
  readonly FileTextIcon = FileText;
  readonly Loader2Icon = Loader2;
  readonly SaveIcon = Save;
  readonly UsersIcon = Users;
  readonly Share2Icon = Share2;

  deliverableForm: FormGroup = this.fb.group({
    campaignCreatorId: ['', [Validators.required]],
    title: ['', [Validators.required, Validators.maxLength(200)]],
    platform: ['Instagram', [Validators.required]],
    contentType: ['Reels', [Validators.required]],
    dueDate: ['', [Validators.required]],
    briefNotes: ['']
  });

  constructor() {
    effect(() => {
      if (this.isOpen) {
        this.populateForm();
      }
    });
  }

  private populateForm(): void {
    if (this.deliverable) {
      this.deliverableForm.patchValue({
        campaignCreatorId: this.deliverable.campaignCreatorId,
        title: this.deliverable.title,
        platform: this.deliverable.platform,
        contentType: this.deliverable.contentType,
        dueDate: this.deliverable.dueDate,
        briefNotes: this.deliverable.briefNotes || ''
      });
      this.deliverableForm.get('campaignCreatorId')?.disable();
    } else {
      const nextWeek = new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString().split('T')[0];
      this.deliverableForm.enable();
      this.deliverableForm.reset({
        campaignCreatorId: this.rosterCreators.length > 0 ? this.rosterCreators[0].id : '',
        title: '',
        platform: 'Instagram',
        contentType: 'Reels',
        dueDate: nextWeek,
        briefNotes: ''
      });
    }
  }

  getCreatorName(creatorId: string): string {
    const found = this.creatorsMap.get(creatorId);
    return found ? `${found.fullName} (${found.niche})` : `Creator #${creatorId.substring(0, 6)}`;
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.deliverableForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  onCancel(): void {
    this.deliverableForm.reset();
    this.close.emit();
  }

  onSubmit(): void {
    if (this.deliverableForm.invalid) {
      this.deliverableForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    const formVal = this.deliverableForm.getRawValue();

    if (this.deliverable) {
      const updateReq: UpdateDeliverableRequest = {
        title: formVal.title.trim(),
        platform: formVal.platform,
        contentType: formVal.contentType,
        dueDate: formVal.dueDate,
        briefNotes: formVal.briefNotes?.trim() || undefined
      };

      this.deliverableService.updateDeliverable(this.deliverable.id, updateReq).subscribe({
        next: res => {
          this.isSubmitting.set(false);
          toast.success('Deliverable updated successfully');
          this.saved.emit(res.data);
          this.onCancel();
        },
        error: err => {
          this.isSubmitting.set(false);
          const msg = err.error?.detail || err.error?.message || 'Failed to update deliverable';
          toast.error(msg);
        }
      });
    } else {
      const createReq: CreateDeliverableRequest = {
        campaignCreatorId: formVal.campaignCreatorId,
        title: formVal.title.trim(),
        platform: formVal.platform,
        contentType: formVal.contentType,
        dueDate: formVal.dueDate,
        briefNotes: formVal.briefNotes?.trim() || undefined
      };

      this.deliverableService.createDeliverable(this.campaignId, createReq).subscribe({
        next: res => {
          this.isSubmitting.set(false);
          toast.success('Deliverable created successfully');
          this.saved.emit(res.data);
          this.onCancel();
        },
        error: err => {
          this.isSubmitting.set(false);
          const msg = err.error?.detail || err.error?.message || 'Failed to create deliverable';
          toast.error(msg);
        }
      });
    }
  }
}
