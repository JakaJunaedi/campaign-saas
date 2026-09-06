import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse, PagedResult } from '../models/api-response.model';
import { AdminOverview, AdminOrganizationItem, UpdateOrganizationStatusRequest } from '../models/admin.model';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private http = inject(HttpClient);
  private baseUrl = '/api/v1/admin';

  getOverview(): Observable<ApiResponse<AdminOverview>> {
    return this.http.get<ApiResponse<AdminOverview>>(`${this.baseUrl}/overview`);
  }

  getOrganizations(
    search?: string,
    status?: string,
    page: number = 1,
    pageSize: number = 10
  ): Observable<ApiResponse<PagedResult<AdminOrganizationItem>>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (search) params = params.set('search', search);
    if (status) params = params.set('status', status);

    return this.http.get<ApiResponse<PagedResult<AdminOrganizationItem>>>(
      `${this.baseUrl}/organizations`,
      { params }
    );
  }

  updateOrganizationStatus(
    id: string,
    request: UpdateOrganizationStatusRequest
  ): Observable<ApiResponse<AdminOrganizationItem>> {
    return this.http.patch<ApiResponse<AdminOrganizationItem>>(
      `${this.baseUrl}/organizations/${id}/status`,
      request
    );
  }
}
