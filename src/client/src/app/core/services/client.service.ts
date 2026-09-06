import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { Client, ClientSummary, CreateClientRequest, UpdateClientRequest, PagedResult } from '../models/client.model';

@Injectable({
  providedIn: 'root'
})
export class ClientService {
  private http = inject(HttpClient);
  private readonly baseUrl = '/api/v1/clients';

  getClients(search?: string, page = 1, pageSize = 10): Observable<ApiResponse<PagedResult<ClientSummary>>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (search && search.trim()) {
      params = params.set('search', search.trim());
    }

    return this.http.get<ApiResponse<PagedResult<ClientSummary>>>(this.baseUrl, { params });
  }

  getClientById(id: string): Observable<ApiResponse<Client>> {
    return this.http.get<ApiResponse<Client>>(`${this.baseUrl}/${id}`);
  }

  createClient(request: CreateClientRequest): Observable<ApiResponse<Client>> {
    return this.http.post<ApiResponse<Client>>(this.baseUrl, request);
  }

  updateClient(id: string, request: UpdateClientRequest): Observable<ApiResponse<Client>> {
    return this.http.put<ApiResponse<Client>>(`${this.baseUrl}/${id}`, request);
  }

  deleteClient(id: string): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.baseUrl}/${id}`);
  }
}
