export interface AdminOverview {
  totalTenants: number;
  totalUsers: number;
  activeTenants: number;
  systemStatus: string;
  timestamp: string;
}

export interface AdminOrganizationItem {
  id: string;
  name: string;
  slug: string;
  status: string;
  usersCount: number;
  createdAt: string;
}

export interface UpdateOrganizationStatusRequest {
  status: string;
}
