import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import {
  Campaign,
  CampaignSummary,
  CampaignCreator,
  CreateCampaignRequest,
  UpdateCampaignRequest,
  UpdateCampaignStatusRequest,
  AddCreatorToRosterRequest,
  UpdateRosterStatusRequest,
  PagedResult
} from '../models/campaign.model';

@Injectable({
  providedIn: 'root'
})
export class CampaignService {
  private http = inject(HttpClient);
  private readonly baseUrl = '/api/v1/campaigns';

  getCampaigns(
    search?: string,
    clientId?: string,
    status?: string,
    page = 1,
    pageSize = 10
  ): Observable<ApiResponse<PagedResult<CampaignSummary>>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (search && search.trim()) {
      params = params.set('search', search.trim());
    }
    if (clientId) {
      params = params.set('clientId', clientId);
    }
    if (status && status !== 'ALL') {
      params = params.set('status', status);
    }

    return this.http.get<ApiResponse<PagedResult<CampaignSummary>>>(this.baseUrl, { params });
  }

  getCampaignById(id: string): Observable<ApiResponse<Campaign>> {
    return this.http.get<ApiResponse<Campaign>>(`${this.baseUrl}/${id}`);
  }

  createCampaign(request: CreateCampaignRequest): Observable<ApiResponse<Campaign>> {
    return this.http.post<ApiResponse<Campaign>>(this.baseUrl, request);
  }

  updateCampaign(id: string, request: UpdateCampaignRequest): Observable<ApiResponse<Campaign>> {
    return this.http.put<ApiResponse<Campaign>>(`${this.baseUrl}/${id}`, request);
  }

  updateCampaignStatus(id: string, status: string): Observable<ApiResponse<Campaign>> {
    const request: UpdateCampaignStatusRequest = { status };
    return this.http.patch<ApiResponse<Campaign>>(`${this.baseUrl}/${id}/status`, request);
  }

  deleteCampaign(id: string): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.baseUrl}/${id}`);
  }

  // Roster endpoints
  getCampaignRoster(campaignId: string): Observable<ApiResponse<CampaignCreator[]>> {
    return this.http.get<ApiResponse<CampaignCreator[]>>(`${this.baseUrl}/${campaignId}/roster`);
  }

  addCreatorToRoster(campaignId: string, request: AddCreatorToRosterRequest): Observable<ApiResponse<CampaignCreator>> {
    return this.http.post<ApiResponse<CampaignCreator>>(`${this.baseUrl}/${campaignId}/roster`, request);
  }

  updateRosterStatus(
    campaignId: string,
    creatorId: string,
    request: UpdateRosterStatusRequest
  ): Observable<ApiResponse<CampaignCreator>> {
    return this.http.patch<ApiResponse<CampaignCreator>>(
      `${this.baseUrl}/${campaignId}/roster/${creatorId}/status`,
      request
    );
  }

  removeCreatorFromRoster(campaignId: string, creatorId: string): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.baseUrl}/${campaignId}/roster/${creatorId}`);
  }
}
