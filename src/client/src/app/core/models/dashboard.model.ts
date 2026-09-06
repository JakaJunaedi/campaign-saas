export interface DashboardRecentCampaignItem {
  id: string;
  title: string;
  clientName: string;
  startDate: string;
  endDate: string;
  budget: number;
  status: string;
  creatorsCount: number;
}

export interface DashboardActionItem {
  deliverableId: string;
  campaignId: string;
  title: string;
  platform: string;
  contentType: string;
  status: string;
  dueDate: string;
}

export interface DashboardOverview {
  activeCampaignsCount: number;
  totalCampaignsCount: number;
  totalClientsCount: number;
  totalCreatorsCount: number;
  pendingReviewsCount: number;
  completedDeliverablesCount: number;
  totalBudgetManaged: number;
  totalReportsCount: number;
  recentCampaigns: DashboardRecentCampaignItem[];
  actionItems: DashboardActionItem[];
}
