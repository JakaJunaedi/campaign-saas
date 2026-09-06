export interface ClientContact {
  name: string;
  email: string;
  phoneNumber?: string;
  position?: string;
}

export interface Client {
  id: string;
  organizationId: string;
  name: string;
  companyName?: string;
  contacts: ClientContact[];
  createdAt: string;
  updatedAt?: string;
}

export interface ClientSummary {
  id: string;
  organizationId: string;
  name: string;
  companyName?: string;
  contactsCount: number;
  createdAt: string;
}

export interface CreateClientRequest {
  name: string;
  companyName?: string;
  contacts?: ClientContact[];
}

export interface UpdateClientRequest {
  name: string;
  companyName?: string;
  contacts?: ClientContact[];
}

export type { PagedResult } from './api-response.model';
