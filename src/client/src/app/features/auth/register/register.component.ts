import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { LucideAngularModule, Building2, Globe, User, Mail, Lock, ArrowRight, Loader2 } from 'lucide-angular';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    LucideAngularModule
  ],
  template: `
    <div class="min-h-screen bg-slate-950 flex flex-col justify-center py-12 sm:px-6 lg:px-8 relative overflow-hidden">
      <!-- Background Ambient Glows -->
      <div class="absolute -top-40 -left-40 w-96 h-96 bg-violet-600/20 rounded-full blur-3xl pointer-events-none"></div>
      <div class="absolute -bottom-40 -right-40 w-96 h-96 bg-indigo-600/20 rounded-full blur-3xl pointer-events-none"></div>

      <div class="sm:mx-auto sm:w-full sm:max-w-md relative z-10">
        <!-- Logo -->
        <div class="flex justify-center">
          <div class="w-12 h-12 rounded-2xl bg-gradient-to-tr from-indigo-500 to-violet-500 flex items-center justify-center text-white text-xl font-extrabold shadow-lg shadow-indigo-500/30">
            CS
          </div>
        </div>
        <h2 class="mt-4 text-center text-2xl font-bold tracking-tight text-white">
          Create Agency Workspace
        </h2>
        <p class="mt-1 text-center text-sm text-slate-400">
          Setup multi-tenant organization & admin account
        </p>
      </div>

      <div class="mt-8 sm:mx-auto sm:w-full sm:max-w-md relative z-10 px-4">
        <div class="bg-slate-900/80 backdrop-blur-xl py-8 px-6 sm:px-10 rounded-2xl shadow-2xl border border-slate-800">
          <!-- Error Alert Banner -->
          @if (errorMessage()) {
            <div class="mb-5 p-3.5 rounded-xl bg-rose-500/10 border border-rose-500/20 text-rose-300 text-xs flex items-center gap-2">
              <span class="w-1.5 h-1.5 rounded-full bg-rose-400"></span>
              <span>{{ errorMessage() }}</span>
            </div>
          }

          <form [formGroup]="registerForm" (ngSubmit)="onSubmit()" class="space-y-4">
            <!-- Organization Name -->
            <div>
              <label for="orgName" class="block text-xs font-medium text-slate-300 mb-1.5">
                Agency / Organization Name
              </label>
              <div class="relative">
                <div class="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none text-slate-500">
                  <lucide-icon [img]="Building2Icon" class="w-4 h-4"></lucide-icon>
                </div>
                <input
                  id="orgName"
                  type="text"
                  formControlName="organizationName"
                  (input)="onOrgNameChange()"
                  placeholder="Apex Influencer Agency"
                  class="block w-full pl-10 pr-3.5 py-2.5 bg-slate-950/60 border border-slate-700/80 rounded-xl text-white placeholder-slate-500 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition"
                  [class.border-rose-500]="isFieldInvalid('organizationName')"
                />
              </div>
              @if (isFieldInvalid('organizationName')) {
                <p class="mt-1 text-[11px] text-rose-400">Organization name is required.</p>
              }
            </div>

            <!-- Organization Slug -->
            <div>
              <label for="slug" class="block text-xs font-medium text-slate-300 mb-1.5">
                Workspace URL Slug
              </label>
              <div class="relative">
                <div class="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none text-slate-500">
                  <lucide-icon [img]="GlobeIcon" class="w-4 h-4"></lucide-icon>
                </div>
                <input
                  id="slug"
                  type="text"
                  formControlName="slug"
                  placeholder="apex-influencer"
                  class="block w-full pl-10 pr-3.5 py-2.5 bg-slate-950/60 border border-slate-700/80 rounded-xl text-white placeholder-slate-500 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition"
                  [class.border-rose-500]="isFieldInvalid('slug')"
                />
              </div>
              <p class="mt-1 text-[10px] text-slate-500">Unique identifier for tenant isolation</p>
            </div>

            <div class="pt-2 border-t border-slate-800"></div>

            <!-- Admin Full Name -->
            <div>
              <label for="adminName" class="block text-xs font-medium text-slate-300 mb-1.5">
                Admin Full Name
              </label>
              <div class="relative">
                <div class="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none text-slate-500">
                  <lucide-icon [img]="UserIcon" class="w-4 h-4"></lucide-icon>
                </div>
                <input
                  id="adminName"
                  type="text"
                  formControlName="adminFullName"
                  placeholder="Sarah Jenkins"
                  class="block w-full pl-10 pr-3.5 py-2.5 bg-slate-950/60 border border-slate-700/80 rounded-xl text-white placeholder-slate-500 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition"
                  [class.border-rose-500]="isFieldInvalid('adminFullName')"
                />
              </div>
              @if (isFieldInvalid('adminFullName')) {
                <p class="mt-1 text-[11px] text-rose-400">Admin name is required.</p>
              }
            </div>

            <!-- Admin Email -->
            <div>
              <label for="adminEmail" class="block text-xs font-medium text-slate-300 mb-1.5">
                Admin Work Email
              </label>
              <div class="relative">
                <div class="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none text-slate-500">
                  <lucide-icon [img]="MailIcon" class="w-4 h-4"></lucide-icon>
                </div>
                <input
                  id="adminEmail"
                  type="email"
                  formControlName="adminEmail"
                  autocomplete="email"
                  placeholder="sarah@apexagency.com"
                  class="block w-full pl-10 pr-3.5 py-2.5 bg-slate-950/60 border border-slate-700/80 rounded-xl text-white placeholder-slate-500 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition"
                  [class.border-rose-500]="isFieldInvalid('adminEmail')"
                />
              </div>
              @if (isFieldInvalid('adminEmail')) {
                <p class="mt-1 text-[11px] text-rose-400">Please enter a valid email address.</p>
              }
            </div>

            <!-- Password -->
            <div>
              <label for="password" class="block text-xs font-medium text-slate-300 mb-1.5">
                Password
              </label>
              <div class="relative">
                <div class="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none text-slate-500">
                  <lucide-icon [img]="LockIcon" class="w-4 h-4"></lucide-icon>
                </div>
                <input
                  id="password"
                  type="password"
                  formControlName="password"
                  placeholder="Minimum 6 characters"
                  class="block w-full pl-10 pr-3.5 py-2.5 bg-slate-950/60 border border-slate-700/80 rounded-xl text-white placeholder-slate-500 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition"
                  [class.border-rose-500]="isFieldInvalid('password')"
                />
              </div>
              @if (isFieldInvalid('password')) {
                <p class="mt-1 text-[11px] text-rose-400">Password must be at least 6 characters.</p>
              }
            </div>

            <!-- Submit Button -->
            <div class="pt-2">
              <button
                type="submit"
                [disabled]="registerForm.invalid || isSubmitting()"
                class="w-full flex items-center justify-center gap-2 py-2.5 px-4 rounded-xl text-sm font-semibold text-white bg-gradient-to-r from-indigo-600 to-violet-600 hover:from-indigo-500 hover:to-violet-500 active:from-indigo-700 active:to-violet-700 disabled:opacity-50 disabled:cursor-not-allowed shadow-lg shadow-indigo-600/25 transition"
              >
                @if (isSubmitting()) {
                  <lucide-icon [img]="Loader2Icon" class="w-4 h-4 animate-spin"></lucide-icon>
                  <span>Creating Workspace...</span>
                } @else {
                  <span>Create Agency Workspace</span>
                  <lucide-icon [img]="ArrowRightIcon" class="w-4 h-4"></lucide-icon>
                }
              </button>
            </div>
          </form>

          <!-- Login Prompt -->
          <div class="mt-6 pt-5 border-t border-slate-800 text-center">
            <p class="text-xs text-slate-400">
              Already have an agency account?
              <a routerLink="/login" class="font-semibold text-indigo-400 hover:text-indigo-300 transition ml-1">
                Sign In
              </a>
            </p>
          </div>
        </div>
      </div>
    </div>
  `
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private notificationService = inject(NotificationService);
  private router = inject(Router);

  readonly isSubmitting = signal<boolean>(false);
  readonly errorMessage = signal<string | null>(null);

  readonly Building2Icon = Building2;
  readonly GlobeIcon = Globe;
  readonly UserIcon = User;
  readonly MailIcon = Mail;
  readonly LockIcon = Lock;
  readonly ArrowRightIcon = ArrowRight;
  readonly Loader2Icon = Loader2;

  readonly registerForm: FormGroup = this.fb.group({
    organizationName: ['', [Validators.required, Validators.maxLength(200)]],
    slug: ['', [Validators.required, Validators.maxLength(100), Validators.pattern(/^[a-z0-9-]+$/)]],
    adminFullName: ['', [Validators.required, Validators.maxLength(150)]],
    adminEmail: ['', [Validators.required, Validators.email, Validators.maxLength(255)]],
    password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(100)]]
  });

  onOrgNameChange() {
    const orgName = this.registerForm.get('organizationName')?.value || '';
    const slugControl = this.registerForm.get('slug');

    if (!slugControl?.dirty) {
      const generatedSlug = orgName
        .toLowerCase()
        .trim()
        .replace(/[^a-z0-9\s-]/g, '')
        .replace(/\s+/g, '-');
      slugControl?.setValue(generatedSlug);
    }
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.registerForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  onSubmit() {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    this.authService.registerOrganization(this.registerForm.value).subscribe({
      next: (res) => {
        this.isSubmitting.set(false);
        this.notificationService.showSuccess('Workspace Created!', `Organization ${res.data.organization.name} is ready.`);
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        const detail = err.error?.detail || err.error?.title || 'Failed to create organization. Please try again.';
        this.errorMessage.set(detail);
      }
    });
  }
}
