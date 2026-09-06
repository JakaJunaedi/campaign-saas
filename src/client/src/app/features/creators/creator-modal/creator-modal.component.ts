import { Component, EventEmitter, Input, Output, inject, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import {
  LucideAngularModule,
  X,
  Plus,
  Trash2,
  Users,
  Mail,
  Phone,
  Globe,
  Tag,
  Loader2,
  Save,
  Share2
} from 'lucide-angular';
import { toast } from 'ngx-sonner';
import { CreatorService } from '../../../core/services/creator.service';
import { Creator, CreateCreatorRequest, UpdateCreatorRequest, SocialAccount } from '../../../core/models/creator.model';

@Component({
  selector: 'app-creator-modal',
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
                <lucide-icon [img]="UsersIcon" class="w-5 h-5"></lucide-icon>
              </div>
              <div>
                <h3 class="text-lg font-bold text-slate-900">
                  {{ creator ? 'Edit Creator Profile' : 'Add Creator to CRM' }}
                </h3>
                <p class="text-xs text-slate-500">
                  {{ creator ? 'Update creator niche, contact info, and social channels.' : 'Register an influencer into your agencys talent CRM database.' }}
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
          <form [formGroup]="creatorForm" (ngSubmit)="onSubmit()" class="p-6 space-y-6 max-h-[75vh] overflow-y-auto">
            <!-- Basic Creator Info -->
            <div class="space-y-4">
              <h4 class="text-xs font-bold text-slate-700 uppercase tracking-wider">Creator Information</h4>
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div class="space-y-1.5">
                  <label class="block text-xs font-semibold text-slate-700">
                    Full Name / Alias <span class="text-rose-500">*</span>
                  </label>
                  <input
                    type="text"
                    formControlName="fullName"
                    placeholder="e.g. Jessica Iskandar, Raditya Dika"
                    class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-sm focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition"
                    [class.border-rose-300]="isFieldInvalid('fullName')"
                  />
                  @if (isFieldInvalid('fullName')) {
                    <p class="text-[11px] text-rose-500 font-medium">Creator name is required.</p>
                  }
                </div>

                <div class="space-y-1.5">
                  <label class="block text-xs font-semibold text-slate-700">
                    Primary Niche / Category <span class="text-rose-500">*</span>
                  </label>
                  <select
                    formControlName="niche"
                    class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-xs bg-white focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition"
                    [class.border-rose-300]="isFieldInvalid('niche')"
                  >
                    <option value="" disabled selected>Select category...</option>
                    @for (n of nicheOptions; track n) {
                      <option [value]="n">{{ n }}</option>
                    }
                  </select>
                  @if (isFieldInvalid('niche')) {
                    <p class="text-[11px] text-rose-500 font-medium">Please select a primary niche.</p>
                  }
                </div>

                <div class="space-y-1.5">
                  <label class="block text-xs font-semibold text-slate-700">Email Address</label>
                  <input
                    type="email"
                    formControlName="email"
                    placeholder="e.g. contact@creator.com"
                    class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-xs focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition"
                  />
                </div>

                <div class="space-y-1.5">
                  <label class="block text-xs font-semibold text-slate-700">WhatsApp / Phone</label>
                  <input
                    type="text"
                    formControlName="phoneNumber"
                    placeholder="e.g. +62 812-9876-5432"
                    class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-xs focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition"
                  />
                </div>
              </div>
            </div>

            <!-- Social Accounts FormArray -->
            <div class="space-y-4 pt-2 border-t border-slate-100">
              <div class="flex items-center justify-between">
                <div>
                  <h4 class="text-xs font-bold text-slate-700 uppercase tracking-wider">Social Media Handles & Reach</h4>
                  <p class="text-[11px] text-slate-400">Add TikTok, Instagram, YouTube accounts with follower numbers.</p>
                </div>
                <button
                  type="button"
                  (click)="addSocialAccount()"
                  class="inline-flex items-center gap-1.5 px-3 py-1.5 rounded-xl bg-indigo-50 hover:bg-indigo-100 text-indigo-700 text-xs font-semibold transition"
                >
                  <lucide-icon [img]="PlusIcon" class="w-3.5 h-3.5"></lucide-icon>
                  <span>Add Account</span>
                </button>
              </div>

              <div formArrayName="socialAccounts" class="space-y-3">
                @for (accountGroup of socialAccountsFormArray.controls; track $index) {
                  <div
                    [formGroupName]="$index"
                    class="p-4 rounded-2xl bg-slate-50 border border-slate-200/80 relative space-y-3"
                  >
                    <div class="flex items-center justify-between">
                      <span class="text-xs font-bold text-indigo-700">Channel #{{ $index + 1 }}</span>
                      @if (socialAccountsFormArray.length > 1) {
                        <button
                          type="button"
                          (click)="removeSocialAccount($index)"
                          class="text-slate-400 hover:text-rose-600 p-1 rounded-lg transition"
                          title="Remove channel"
                        >
                          <lucide-icon [img]="Trash2Icon" class="w-4 h-4"></lucide-icon>
                        </button>
                      }
                    </div>

                    <div class="grid grid-cols-1 sm:grid-cols-4 gap-3">
                      <!-- Platform -->
                      <div class="space-y-1">
                        <label class="block text-[11px] font-semibold text-slate-600">Platform *</label>
                        <select
                          formControlName="platform"
                          class="w-full px-3 py-2 rounded-xl bg-white border border-slate-200 text-xs focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600"
                        >
                          <option value="TikTok">TikTok</option>
                          <option value="Instagram">Instagram</option>
                          <option value="YouTube">YouTube</option>
                          <option value="Twitter">Twitter / X</option>
                        </select>
                      </div>

                      <!-- Handle -->
                      <div class="space-y-1">
                        <label class="block text-[11px] font-semibold text-slate-600">Handle / Username *</label>
                        <input
                          type="text"
                          formControlName="handle"
                          placeholder="e.g. @jessica_iskandar"
                          class="w-full px-3 py-2 rounded-xl bg-white border border-slate-200 text-xs focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600"
                        />
                      </div>

                      <!-- Profile URL -->
                      <div class="space-y-1">
                        <label class="block text-[11px] font-semibold text-slate-600">Profile URL *</label>
                        <input
                          type="url"
                          formControlName="profileUrl"
                          placeholder="https://tiktok.com/@..."
                          class="w-full px-3 py-2 rounded-xl bg-white border border-slate-200 text-xs focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600"
                        />
                      </div>

                      <!-- Followers Count -->
                      <div class="space-y-1">
                        <label class="block text-[11px] font-semibold text-slate-600">Followers *</label>
                        <input
                          type="number"
                          formControlName="followerCount"
                          placeholder="500000"
                          min="0"
                          class="w-full px-3 py-2 rounded-xl bg-white border border-slate-200 text-xs focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600"
                        />
                      </div>
                    </div>
                  </div>
                }
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
                [disabled]="isSubmitting() || creatorForm.invalid"
                class="px-5 py-2.5 rounded-xl bg-indigo-600 hover:bg-indigo-500 disabled:bg-indigo-300 text-white text-xs font-semibold shadow-md shadow-indigo-600/20 transition flex items-center gap-2"
              >
                @if (isSubmitting()) {
                  <lucide-icon [img]="Loader2Icon" class="w-4 h-4 animate-spin"></lucide-icon>
                  <span>Saving...</span>
                } @else {
                  <lucide-icon [img]="SaveIcon" class="w-4 h-4"></lucide-icon>
                  <span>{{ creator ? 'Update Creator' : 'Save to CRM' }}</span>
                }
              </button>
            </div>
          </form>
        </div>
      </div>
    }
  `
})
export class CreatorModalComponent {
  @Input() isOpen = false;
  @Input() creator: Creator | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<Creator>();

  private fb = inject(FormBuilder);
  private creatorService = inject(CreatorService);

  readonly isSubmitting = signal<boolean>(false);

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

  readonly XIcon = X;
  readonly PlusIcon = Plus;
  readonly Trash2Icon = Trash2;
  readonly UsersIcon = Users;
  readonly MailIcon = Mail;
  readonly PhoneIcon = Phone;
  readonly GlobeIcon = Globe;
  readonly TagIcon = Tag;
  readonly Loader2Icon = Loader2;
  readonly SaveIcon = Save;
  readonly Share2Icon = Share2;

  creatorForm: FormGroup = this.fb.group({
    fullName: ['', [Validators.required, Validators.maxLength(200)]],
    niche: ['', [Validators.required]],
    email: ['', [Validators.email]],
    phoneNumber: [''],
    socialAccounts: this.fb.array([])
  });

  get socialAccountsFormArray(): FormArray {
    return this.creatorForm.get('socialAccounts') as FormArray;
  }

  constructor() {
    effect(() => {
      if (this.isOpen) {
        this.populateForm();
      }
    });
  }

  private populateForm(): void {
    this.socialAccountsFormArray.clear();

    if (this.creator) {
      this.creatorForm.patchValue({
        fullName: this.creator.fullName,
        niche: this.creator.niche,
        email: this.creator.email || '',
        phoneNumber: this.creator.phoneNumber || ''
      });

      if (this.creator.socialAccounts && this.creator.socialAccounts.length > 0) {
        for (const acc of this.creator.socialAccounts) {
          this.socialAccountsFormArray.push(this.createSocialAccountGroup(acc));
        }
      } else {
        this.socialAccountsFormArray.push(this.createSocialAccountGroup());
      }
    } else {
      this.creatorForm.reset({
        fullName: '',
        niche: '',
        email: '',
        phoneNumber: ''
      });
      this.socialAccountsFormArray.push(this.createSocialAccountGroup());
    }
  }

  createSocialAccountGroup(account?: SocialAccount): FormGroup {
    return this.fb.group({
      platform: [account?.platform || 'Instagram', [Validators.required]],
      handle: [account?.handle || '', [Validators.required]],
      profileUrl: [account?.profileUrl || '', [Validators.required]],
      followerCount: [account?.followerCount || 0, [Validators.required, Validators.min(0)]]
    });
  }

  addSocialAccount(): void {
    this.socialAccountsFormArray.push(this.createSocialAccountGroup());
  }

  removeSocialAccount(index: number): void {
    if (this.socialAccountsFormArray.length > 1) {
      this.socialAccountsFormArray.removeAt(index);
    }
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.creatorForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  onCancel(): void {
    this.creatorForm.reset();
    this.close.emit();
  }

  onSubmit(): void {
    if (this.creatorForm.invalid) {
      this.creatorForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    const formVal = this.creatorForm.value;

    const accountsPayload: SocialAccount[] = (formVal.socialAccounts || []).filter(
      (a: SocialAccount) => a.platform && a.handle?.trim() && a.profileUrl?.trim()
    );

    if (this.creator) {
      const updateReq: UpdateCreatorRequest = {
        fullName: formVal.fullName.trim(),
        niche: formVal.niche,
        email: formVal.email?.trim() || undefined,
        phoneNumber: formVal.phoneNumber?.trim() || undefined,
        socialAccounts: accountsPayload
      };

      this.creatorService.updateCreator(this.creator.id, updateReq).subscribe({
        next: res => {
          this.isSubmitting.set(false);
          toast.success('Creator profile updated');
          this.saved.emit(res.data);
          this.onCancel();
        },
        error: err => {
          this.isSubmitting.set(false);
          const msg = err.error?.detail || err.error?.message || 'Failed to update creator';
          toast.error(msg);
        }
      });
    } else {
      const createReq: CreateCreatorRequest = {
        fullName: formVal.fullName.trim(),
        niche: formVal.niche,
        email: formVal.email?.trim() || undefined,
        phoneNumber: formVal.phoneNumber?.trim() || undefined,
        socialAccounts: accountsPayload
      };

      this.creatorService.createCreator(createReq).subscribe({
        next: res => {
          this.isSubmitting.set(false);
          toast.success('Creator registered to CRM');
          this.saved.emit(res.data);
          this.onCancel();
        },
        error: err => {
          this.isSubmitting.set(false);
          const msg = err.error?.detail || err.error?.message || 'Failed to create creator';
          toast.error(msg);
        }
      });
    }
  }
}
