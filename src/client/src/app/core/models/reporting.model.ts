export interface CampaignMetric {
  id: string;
  organizationId: string;
  deliverableId: string;
  reach: number;
  impressions: number;
  views: number;
  likes: number;
  comments: number;
  shares: number;
  clicks: number;
  totalEngagement: number;
  engagementRate: number;
  recordedAt: string;
}

export interface RecordDeliverableMetricsRequest {
  reach: number;
  impressions: number;
  views: number;
  likes: number;
  comments: number;
  shares: number;
  clicks: number;
}

export type ReportStatus = 'Pending' | 'Generating' | 'Completed' | 'Failed';

export interface CampaignReport {
  id: string;
  organizationId: string;
  campaignId: string;
  status: ReportStatus;
  fileObjectKey?: string;
  downloadUrl?: string;
  errorMessage?: string;
  requestedAt: string;
  completedAt?: string;
}

export interface OrganizationInfo {
  name: string;
  logoUrl?: string;
}

export interface CampaignInfo {
  title: string;
  clientName: string;
  startDate: string;
  endDate: string;
  totalBudget: number;
  currency: string;
}

export interface SummaryMetricsInfo {
  totalCreators: number;
  totalDeliverables: number;
  totalReach: number;
  totalImpressions: number;
  totalViews: number;
  totalEngagement: number;
  averageEngagementRate: number;
  costPerEngagement: number;
  costPerView: number;
}

export interface DeliverableMetricItem {
  creatorName: string;
  platform: string;
  contentType: string;
  liveUrl?: string;
  reach: number;
  views: number;
  likes: number;
  comments: number;
  shares: number;
  totalEngagement: number;
  engagementRate: number;
}

export interface CampaignReportPayload {
  organization: OrganizationInfo;
  campaign: CampaignInfo;
  summaryMetrics: SummaryMetricsInfo;
  deliverables: DeliverableMetricItem[];
}
