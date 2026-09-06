export interface UserDto {
  id: string;
  organizationId: string;
  email: string;
  fullName: string;
  role: 'SuperAdmin' | 'AgencyOwner' | 'CampaignManager' | 'ContentReviewer' | 'Creator';
  isActive: boolean;
  createdAt: string;
}

export interface OrganizationDto {
  id: string;
  name: string;
  slug: string;
  status: string;
  createdAt: string;
}

export interface AuthResultDto {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  user: UserDto;
  organization: OrganizationDto;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterOrganizationRequest {
  organizationName: string;
  slug: string;
  adminFullName: string;
  adminEmail: string;
  password: string;
}
