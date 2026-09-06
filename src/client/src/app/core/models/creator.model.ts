export interface SocialAccount {
  platform: string;
  handle: string;
  profileUrl: string;
  followerCount: number;
}

export interface Creator {
  id: string;
  organizationId: string;
  fullName: string;
  email?: string;
  phoneNumber?: string;
  niche: string;
  status: string;
  socialAccounts: SocialAccount[];
  createdAt: string;
  updatedAt?: string;
}

export interface CreatorSummary {
  id: string;
  organizationId: string;
  fullName: string;
  email?: string;
  phoneNumber?: string;
  niche: string;
  status: string;
  socialAccountsCount: number;
  totalFollowers: number;
  createdAt: string;
}

export interface CreateCreatorRequest {
  fullName: string;
  niche: string;
  email?: string;
  phoneNumber?: string;
  socialAccounts?: SocialAccount[];
}

export interface UpdateCreatorRequest {
  fullName: string;
  niche: string;
  email?: string;
  phoneNumber?: string;
  socialAccounts?: SocialAccount[];
}

export interface UpdateCreatorStatusRequest {
  status: string;
}

export type { PagedResult } from './api-response.model';
