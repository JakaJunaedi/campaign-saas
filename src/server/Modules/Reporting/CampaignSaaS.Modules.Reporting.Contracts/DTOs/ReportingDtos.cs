namespace CampaignSaaS.Modules.Reporting.Contracts.DTOs;

public record CampaignMetricDto(
    Guid Id,
    Guid OrganizationId,
    Guid DeliverableId,
    long Reach,
    long Impressions,
    long Views,
    long Likes,
    long Comments,
    long Shares,
    long Clicks,
    long TotalEngagement,
    double EngagementRate,
    DateTimeOffset RecordedAt);

public record CampaignReportDto(
    Guid Id,
    Guid OrganizationId,
    Guid CampaignId,
    string Status,
    string? FileObjectKey,
    string? DownloadUrl,
    string? ErrorMessage,
    DateTimeOffset RequestedAt,
    DateTimeOffset? CompletedAt);

public record CampaignReportPayloadDto(
    OrganizationInfo Organization,
    CampaignInfo Campaign,
    SummaryMetricsInfo SummaryMetrics,
    IReadOnlyList<DeliverableMetricItem> Deliverables);

public record OrganizationInfo(string Name, string? LogoUrl);

public record CampaignInfo(
    string Title,
    string ClientName,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal TotalBudget,
    string Currency);

public record SummaryMetricsInfo(
    int TotalCreators,
    int TotalDeliverables,
    long TotalReach,
    long TotalImpressions,
    long TotalViews,
    long TotalEngagement,
    double AverageEngagementRate,
    decimal CostPerEngagement,
    decimal CostPerView);

public record DeliverableMetricItem(
    string CreatorName,
    string Platform,
    string ContentType,
    string? LiveUrl,
    long Reach,
    long Views,
    long Likes,
    long Comments,
    long Shares,
    long TotalEngagement,
    double EngagementRate);
