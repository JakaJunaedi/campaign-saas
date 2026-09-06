import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { AuditLog, AuditPagedResult } from '../models/audit.model';

@Injectable({
  providedIn: 'root'
})
export class AuditService {
  private http = inject(HttpClient);
  private baseUrl = '/api/v1/audit-logs';

  getAuditLogs(
    module?: string,
    action?: string,
    entityId?: string,
    page = 1,
    pageSize = 50
  ): Observable<ApiResponse<AuditPagedResult<AuditLog>>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (module && module !== 'ALL') {
      params = params.set('module', module);
    }
    if (action && action !== 'ALL') {
      params = params.set('action', action);
    }
    if (entityId) {
      params = params.set('entityId', entityId);
    }

    return this.http.get<ApiResponse<AuditPagedResult<AuditLog>>>(this.baseUrl, { params });
  }
}
