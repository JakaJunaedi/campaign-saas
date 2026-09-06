import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { LucideAngularModule, Lock, Mail, ArrowRight, Loader2 } from 'lucide-angular';

@Component({
  selector: 'app-login',
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
      <div class="absolute -top-40 -left-40 w-96 h-96 bg-indigo-600/20 rounded-full blur-3xl pointer-events-none"></div>
      <div class="absolute -bottom-40 -right-40 w-96 h-96 bg-violet-600/20 rounded-full blur-3xl pointer-events-none"></div>

      <div class="sm:mx-auto sm:w-full sm:max-w-md relative z-10">
        <!-- Logo -->
        <div class="flex justify-center">
          <div class="w-12 h-12 rounded-2xl bg-gradient-to-tr from-indigo-500 to-violet-500 flex items-center justify-center text-white text-xl font-extrabold shadow-lg shadow-indigo-500/30">
            CS
          </div>
        </div>
        <h2 class="mt-4 text-center text-2xl font-bold tracking-tight text-white">
          Sign in to your agency
        </h2>
        <p class="mt-1 text-center text-sm text-slate-400">
          Campaign & Influencer Operations Platform
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

          <form [formGroup]="loginForm" (ngSubmit)="onSubmit()" class="space-y-4">
            <!-- Email Field -->
            <div>
              <label for="email" class="block text-xs font-medium text-slate-300 mb-1.5">
                Work Email
              </label>
              <div class="relative">
                <div class="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none text-slate-500">
                  <lucide-icon [img]="MailIcon" class="w-4 h-4"></lucide-icon>
                </div>
                <input
                  id="email"
                  type="email"
                  formControlName="email"
                  autocomplete="email"
                  placeholder="admin@agency.com"
                  class="block w-full pl-10 pr-3.5 py-2.5 bg-slate-950/60 border border-slate-700/80 rounded-xl text-white placeholder-slate-500 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition"
                  [class.border-rose-500]="isFieldInvalid('email')"
                />
              </div>
              @if (isFieldInvalid('email')) {
                <p class="mt-1 text-[11px] text-rose-400">Please enter a valid email address.</p>
              }
            </div>

            <!-- Password Field -->
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
                  autocomplete="current-password"
                  placeholder="••••••••"
                  class="block w-full pl-10 pr-3.5 py-2.5 bg-slate-950/60 border border-slate-700/80 rounded-xl text-white placeholder-slate-500 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition"
                  [class.border-rose-500]="isFieldInvalid('password')"
                />
              </div>
              @if (isFieldInvalid('password')) {
                <p class="mt-1 text-[11px] text-rose-400">Password is required.</p>
              }
            </div>

            <!-- Submit Button -->
            <div class="pt-2">
              <button
                type="submit"
                [disabled]="loginForm.invalid || isSubmitting()"
                class="w-full flex items-center justify-center gap-2 py-2.5 px-4 rounded-xl text-sm font-semibold text-white bg-indigo-600 hover:bg-indigo-500 active:bg-indigo-700 disabled:opacity-50 disabled:cursor-not-allowed shadow-lg shadow-indigo-600/25 transition"
              >
                @if (isSubmitting()) {
                  <lucide-icon [img]="Loader2Icon" class="w-4 h-4 animate-spin"></lucide-icon>
                  <span>Signing In...</span>
                } @else {
                  <span>Sign In</span>
                  <lucide-icon [img]="ArrowRightIcon" class="w-4 h-4"></lucide-icon>
                }
              </button>
            </div>
          </form>

          <!-- Register Prompt -->
          <div class="mt-6 pt-5 border-t border-slate-800 text-center">
            <p class="text-xs text-slate-400">
              Need to create a new agency organization?
              <a routerLink="/register" class="font-semibold text-indigo-400 hover:text-indigo-300 transition ml-1">
                Register Workspace
              </a>
            </p>
          </div>
        </div>
      </div>
    </div>
  `
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private notificationService = inject(NotificationService);
  private router = inject(Router);

  readonly isSubmitting = signal<boolean>(false);
  readonly errorMessage = signal<string | null>(null);

  readonly MailIcon = Mail;
  readonly LockIcon = Lock;
  readonly ArrowRightIcon = ArrowRight;
  readonly Loader2Icon = Loader2;

  readonly loginForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  isFieldInvalid(fieldName: string): boolean {
    const field = this.loginForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  onSubmit() {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    this.authService.login(this.loginForm.value).subscribe({
      next: (res) => {
        this.isSubmitting.set(false);
        this.notificationService.showSuccess('Welcome back!', `Signed in as ${res.data.user.fullName}`);
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        const detail = err.error?.detail || err.error?.title || 'Invalid email or password. Please try again.';
        this.errorMessage.set(detail);
      }
    });
  }
}
