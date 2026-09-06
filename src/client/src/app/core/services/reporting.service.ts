import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import {
  CampaignMetric,
  RecordDeliverableMetricsRequest,
  CampaignReport
} from '../models/reporting.model';

@Injectable({
  providedIn: 'root'
})
export class ReportingService {
  private http = inject(HttpClient);
  private baseUrl = '/api/v1';

  // Record Deliverable Metrics (PUT /api/v1/deliverables/{deliverableId}/metrics)
  recordDeliverableMetrics(
    deliverableId: string,
    request: RecordDeliverableMetricsRequest
  ): Observable<ApiResponse<CampaignMetric>> {
    return this.http.put<ApiResponse<CampaignMetric>>(
      `${this.baseUrl}/deliverables/${deliverableId}/metrics`,
      request
    );
  }

  // Get Deliverable Metrics (GET /api/v1/deliverables/{deliverableId}/metrics)
  getDeliverableMetrics(deliverableId: string): Observable<ApiResponse<CampaignMetric>> {
    return this.http.get<ApiResponse<CampaignMetric>>(
      `${this.baseUrl}/deliverables/${deliverableId}/metrics`
    );
  }

  // Generate Campaign Report Asynchronously (POST /api/v1/campaigns/{campaignId}/reports/generate)
  generateCampaignReport(campaignId: string): Observable<ApiResponse<CampaignReport>> {
    return this.http.post<ApiResponse<CampaignReport>>(
      `${this.baseUrl}/campaigns/${campaignId}/reports/generate`,
      {}
    );
  }

  // Get Campaign Reports (GET /api/v1/campaigns/{campaignId}/reports)
  getCampaignReports(campaignId: string): Observable<ApiResponse<CampaignReport[]>> {
    return this.http.get<ApiResponse<CampaignReport[]>>(
      `${this.baseUrl}/campaigns/${campaignId}/reports`
    );
  }

  // Get Report By ID (GET /api/v1/reports/{id})
  getReportById(id: string): Observable<ApiResponse<CampaignReport>> {
    return this.http.get<ApiResponse<CampaignReport>>(`${this.baseUrl}/reports/${id}`);
  }

  // Get Report MinIO Pre-Signed Download URL (GET /api/v1/reports/{id}/download)
  getReportDownloadUrl(id: string): Observable<ApiResponse<string>> {
    return this.http.get<ApiResponse<string>>(`${this.baseUrl}/reports/${id}/download`);
  }
}
