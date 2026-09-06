export type CampaignStatus = 'Draft' | 'Active' | 'Completed' | 'Cancelled';

export interface Campaign {
  id: string;
  organizationId: string;
  clientId: string;
  title: string;
  description?: string;
  budget: number;
  startDate: string;
  endDate: string;
  status: CampaignStatus | string;
  createdAt: string;
  updatedAt?: string;
}

export interface CampaignSummary {
  id: string;
  organizationId: string;
  clientId: string;
  title: string;
  budget: number;
  startDate: string;
  endDate: string;
  status: CampaignStatus | string;
  creatorsCount: number;
  createdAt: string;
}

export interface CampaignCreator {
  id: string;
  organizationId: string;
  campaignId: string;
  creatorId: string;
  status: string;
  agreedRate: number;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateCampaignRequest {
  clientId: string;
  title: string;
  description?: string;
  budget: number;
  startDate: string;
  endDate: string;
}

export interface UpdateCampaignRequest {
  title: string;
  description?: string;
  budget: number;
  startDate: string;
  endDate: string;
}

export interface UpdateCampaignStatusRequest {
  status: string;
}

export interface AddCreatorToRosterRequest {
  creatorId: string;
  agreedRate: number;
}

export interface UpdateRosterStatusRequest {
  status: string;
  agreedRate?: number;
}

export type { PagedResult } from './api-response.model';

