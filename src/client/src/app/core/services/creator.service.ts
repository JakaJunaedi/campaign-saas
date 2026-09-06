import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import {
  Creator,
  CreatorSummary,
  CreateCreatorRequest,
  UpdateCreatorRequest,
  UpdateCreatorStatusRequest,
  PagedResult
} from '../models/creator.model';

@Injectable({
  providedIn: 'root'
})
export class CreatorService {
  private http = inject(HttpClient);
  private readonly baseUrl = '/api/v1/creators';

  getCreators(
    search?: string,
    niche?: string,
    platform?: string,
    status?: string,
    page = 1,
    pageSize = 10
  ): Observable<ApiResponse<PagedResult<CreatorSummary>>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (search && search.trim()) {
      params = params.set('search', search.trim());
    }
    if (niche && niche !== 'ALL') {
      params = params.set('niche', niche);
    }
    if (platform && platform !== 'ALL') {
      params = params.set('platform', platform);
    }
    if (status && status !== 'ALL') {
      params = params.set('status', status);
    }

    return this.http.get<ApiResponse<PagedResult<CreatorSummary>>>(this.baseUrl, { params });
  }

  getCreatorById(id: string): Observable<ApiResponse<Creator>> {
    return this.http.get<ApiResponse<Creator>>(`${this.baseUrl}/${id}`);
  }

  createCreator(request: CreateCreatorRequest): Observable<ApiResponse<Creator>> {
    return this.http.post<ApiResponse<Creator>>(this.baseUrl, request);
  }

  updateCreator(id: string, request: UpdateCreatorRequest): Observable<ApiResponse<Creator>> {
    return this.http.put<ApiResponse<Creator>>(`${this.baseUrl}/${id}`, request);
  }

  updateCreatorStatus(id: string, status: string): Observable<ApiResponse<Creator>> {
    const request: UpdateCreatorStatusRequest = { status };
    return this.http.patch<ApiResponse<Creator>>(`${this.baseUrl}/${id}/status`, request);
  }

  deleteCreator(id: string): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.baseUrl}/${id}`);
  }
}
