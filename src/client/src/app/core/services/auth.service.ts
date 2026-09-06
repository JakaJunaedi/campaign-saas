import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, catchError, throwError } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { AuthResultDto, LoginRequest, RegisterOrganizationRequest, UserDto, OrganizationDto } from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  private readonly TOKEN_KEY = 'campaign_saas_access_token';
  private readonly REFRESH_TOKEN_KEY = 'campaign_saas_refresh_token';
  private readonly USER_KEY = 'campaign_saas_user';
  private readonly ORG_KEY = 'campaign_saas_org';

  readonly currentUser = signal<UserDto | null>(this.getStoredUser());
  readonly currentOrganization = signal<OrganizationDto | null>(this.getStoredOrg());
  readonly isLoading = signal<boolean>(false);

  readonly isAuthenticated = computed(() => !!this.currentUser() && !!this.getAccessToken());
  readonly userRole = computed(() => this.currentUser()?.role ?? null);
  readonly userFullName = computed(() => this.currentUser()?.fullName ?? 'User');
  readonly orgName = computed(() => this.currentOrganization()?.name ?? 'Campaign SaaS');

  login(request: LoginRequest): Observable<ApiResponse<AuthResultDto>> {
    this.isLoading.set(true);
    return this.http.post<ApiResponse<AuthResultDto>>('/api/v1/auth/login', request).pipe(
      tap(response => {
        this.isLoading.set(false);
        if (response.success && response.data) {
          this.setSession(response.data);
        }
      }),
      catchError(err => {
        this.isLoading.set(false);
        return throwError(() => err);
      })
    );
  }

  registerOrganization(request: RegisterOrganizationRequest): Observable<ApiResponse<AuthResultDto>> {
    this.isLoading.set(true);
    return this.http.post<ApiResponse<AuthResultDto>>('/api/v1/auth/register-organization', request).pipe(
      tap(response => {
        this.isLoading.set(false);
        if (response.success && response.data) {
          this.setSession(response.data);
        }
      }),
      catchError(err => {
        this.isLoading.set(false);
        return throwError(() => err);
      })
    );
  }

  refreshToken(): Observable<ApiResponse<AuthResultDto>> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      this.logout();
      return throwError(() => new Error('No refresh token available'));
    }

    return this.http.post<ApiResponse<AuthResultDto>>('/api/v1/auth/refresh', { refreshToken }).pipe(
      tap(response => {
        if (response.success && response.data) {
          this.setSession(response.data);
        }
      }),
      catchError(err => {
        this.logout();
        return throwError(() => err);
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    localStorage.removeItem(this.ORG_KEY);

    this.currentUser.set(null);
    this.currentOrganization.set(null);
    this.router.navigate(['/login']);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  private setSession(authResult: AuthResultDto): void {
    localStorage.setItem(this.TOKEN_KEY, authResult.accessToken);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, authResult.refreshToken);
    localStorage.setItem(this.USER_KEY, JSON.stringify(authResult.user));
    localStorage.setItem(this.ORG_KEY, JSON.stringify(authResult.organization));

    this.currentUser.set(authResult.user);
    this.currentOrganization.set(authResult.organization);
  }

  private getStoredUser(): UserDto | null {
    const userJson = localStorage.getItem(this.USER_KEY);
    if (!userJson) return null;
    try {
      return JSON.parse(userJson) as UserDto;
    } catch {
      return null;
    }
  }

  private getStoredOrg(): OrganizationDto | null {
    const orgJson = localStorage.getItem(this.ORG_KEY);
    if (!orgJson) return null;
    try {
      return JSON.parse(orgJson) as OrganizationDto;
    } catch {
      return null;
    }
  }
}
